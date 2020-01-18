using AlertMe.Controllers;
using AlertMe.Data;
using AlertMe.Models;
using AlertMe.ViewModels;
using Clarifai.API;
using Clarifai.DTOs.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Alert_Me.Controllers
{

    public class HomeController : Controller
    {

        private readonly UserManager<User> _userManager;

        readonly ApplicationDbContext applicationDbContext = GetApplicationDbContext.GetApplication();

        UserAlertToGeoJsonAdapter alertToGeoJson = new UserAlertToGeoJsonAdapter();

        public HomeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public int GeoJsonLocation { get; private set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public IActionResult Index()
        {
            List<Alert> alerts = new List<Alert> {
                new Alert()
            };

            List<Feature> features = new List<Feature>();

            foreach (Alert alert in applicationDbContext.Alerts)
            {

                features.Add(alertToGeoJson.Convert(new UsersAlerts
                {
                    AlertLevel = AlertLevel.AlertLocation,
                    Longitude = alert.Longitude,
                    Latitude = alert.Latitude
                }));

            }

            GeoJson geoJson = new GeoJson
            {
                features = features
            };

            alerts.ElementAt(0).UserAlertStrings = geoJson;

            var JsonFormat = JsonConvert.SerializeObject(alerts);

            return View(new SummaryViewModel { GeoJson = JsonFormat});
        }

        [Authorize]
        public async Task<IActionResult> Alerts()
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            List<Alert> alerts = new List<Alert>();
            
            var _alerts = (from row in applicationDbContext.Alerts
                           where row.CreatedAt > DateTime.Now.AddDays(-14) orderby row.CreatedAt descending
                           select row);

            var getSubscribedAlerts = GetSubscribedAlerts(user);
           
            foreach (Alert alert in _alerts)
            {
                alert.UserAlertStrings = GetGeoJson(alert);
                alert.CreatedBy = GetUser(alert.CreatedById);

                if (getSubscribedAlerts.Contains(alert))
                {
                    alerts.Insert(0, alert);
                }
                else
                {
                    alerts.Add(alert);
                }
            }

            var JsonFormat = JsonConvert.SerializeObject(alerts);

            return View(
                new JsonAlertViewModel {
                    Alerts = alerts,
                    JsonFormat = JsonFormat,
                    User = user,
                    NotificationsNumber = GetNotificationNumber(user)
                });
        }

        [HttpPost]
        public string AddAlert([FromBody] JObject jObject)
        {
            UsersAlerts userAlerts = new UsersAlerts
            {
                UserId = jObject.GetValue("UserId").Value<string>(),
                AlertId = jObject.GetValue("AlertId").Value<int>(),
                Latitude = jObject.GetValue("Latitude").Value<double>(),
                Longitude = jObject.GetValue("Longitude").Value<double>(),
                AlertLevel = (AlertLevel)jObject.GetValue("AlertLevel").Value<int>()
            };

            applicationDbContext.UserAlerts.Add(userAlerts);

            User user = GetUser(userAlerts.UserId);

            var updatedAlert = (from alert in applicationDbContext.Alerts where alert.Id == userAlerts.AlertId
                                select alert).FirstOrDefault();

            switch (userAlerts.AlertLevel)
            {
                case AlertLevel.Critical:
                    updatedAlert.Critical++;
                    AddNotification(user, updatedAlert, "Marked Critical");
                    break;
                case AlertLevel.Warning:
                    updatedAlert.Warning++;
                    AddNotification(user, updatedAlert, "Marked Warning");
                    break;
                case AlertLevel.FalseAlarm:
                    updatedAlert.FalseAlarm++;
                    AddNotification(user, updatedAlert, "Marked False Alarm");
                    break;
            }
            updatedAlert.LastCheck = DateTime.Now;

            applicationDbContext.SaveChanges();

            var geoJson = GetGeoJson(updatedAlert);

            updatedAlert.UserAlertStrings = geoJson;

            return JsonConvert.SerializeObject(updatedAlert);
        }


        public async Task<ActionResult> CreateAlert(Alert alert)
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            alert.CreatedById = user.Id;
            alert.CreatedAt = DateTime.Now;
            alert.LastCheck = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return View("NewAlert", alert);
            }

            if (alert.Image != null && alert.Image.Length > 0)

            {
                byte[] img = null;
                using (var file = alert.Image.OpenReadStream())
                using (var memory = new MemoryStream())
                {
                    file.CopyTo(memory);
                    img = memory.ToArray();
                }

                if (!await ValidateImage(img))
                {
                    alert.ParsedImage = "Error In appropraite image";
                    return View("NewAlert", alert);
                }
                alert.Picture = img;
            }

            applicationDbContext.Alerts.Add(alert);

            if (HasSubscribers(alert.CreatedById))
            {
                AlertForSubscribed alertFor = new AlertForSubscribed
                {
                    AlertId = alert.Id,
                    CreatedBy = alert.CreatedById
                };
                var Follows = GetFollowers(user.Id);
                foreach (Follow follow in Follows)
                {
                    follow.UnSeen = true;
                }
                applicationDbContext.Follows.UpdateRange(Follows);
                applicationDbContext.AlertForSubscribers.Add(alertFor);
            }

            applicationDbContext.SaveChanges();

            return RedirectToAction("Alerts");
        }


        [HttpPost]
        public string SortAlerts([FromBody] JObject jObject)
        {
            IQueryable<Alert> alerts = null ;
            List<Alert> readyAlerts = new List<Alert>();

            var key = jObject.GetValue("SortBy").Value<string>();
            var latitude = jObject.GetValue("Latitude").Value<double>();
            var longitude = jObject.GetValue("Longitude").Value<double>();

            if (key.Equals("CREATED"))
            {
                alerts = (from row in applicationDbContext.Alerts orderby row.CreatedAt descending select row);
            }
            else if (key.Equals("POPULAR"))
            {
                alerts = (from row in applicationDbContext.Alerts
                          orderby row.Critical + row.FalseAlarm + row.Warning + row.NumberOfComments descending
                          select row);
            }
            else if (key.Equals("NERBY"))
            {
                alerts = (from row in applicationDbContext.Alerts
                          orderby CalculateDistance(row.Longitude, longitude, row.Latitude, latitude)
                          select row);
            }
            else if (key.Equals("CHECKED"))
            {
                alerts = (from row in applicationDbContext.Alerts orderby row.LastCheck descending select row);
            }

            foreach(var alert in alerts)
            {

                alert.UserAlertStrings = GetGeoJson(alert);

                readyAlerts.Add(alert);
            }
 
            return JsonConvert.SerializeObject(alerts);
        }

        [Route("/Home/EditAlert/{alertId}")]
        public async Task<ActionResult> EditAlertAsync(int? alertId)
        {
            
            if(alertId == null)
            {
                return RedirectToAction("Alerts");
            }
            User user = await _userManager.GetUserAsync(HttpContext.User);
            Alert alert = GetAlert((int)alertId);
            if (!user.Id.Equals(alert.CreatedById))
            {
                return RedirectToAction("Alerts");
            }

            return View("NewAlert", alert);
        }

        [Route("/Home/OpenNotification/{notifId}")]
        public async Task<ActionResult> OpenNotification(int? notifId)
        {
            if(notifId == null)
            {
                return RedirectToAction("Alerts");
            }

            var notification = (from row in applicationDbContext.Notifications where 
                                row.Id == notifId select row).FirstOrDefault();
            notification.Seen = true;
            
            applicationDbContext.Notifications.Remove(notification);
            applicationDbContext.SaveChanges();

            JsonAlertViewModel json = await GetAlertJsonById(notification.AlertId);
            return View("Alerts", json);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateAlert(Alert alert)
        {
            if (!ModelState.IsValid)
            {
                return View("NewAlert", alert);
            }

            User user = await _userManager.GetUserAsync(HttpContext.User);
            Alert alertToUpdate = GetAlert(alert.Id);

            alertToUpdate.Tags = alert.Tags;
            alertToUpdate.Title = alert.Title;
            alertToUpdate.Longitude = alert.Longitude;
            alertToUpdate.Latitude = alert.Latitude;
            alertToUpdate.Description = alert.Description;

            applicationDbContext.Alerts.Update(alertToUpdate);
            applicationDbContext.SaveChanges();

            return RedirectToAction("Alerts");
        }

        [Route("/Home/FilterByTagAsync/{tag}")]
        public async Task<ActionResult> FilterByTagAsync(string tag)
        {
            if (tag == null)
            {
                return RedirectToAction("Alerts");
            }
            JsonAlertViewModel json = await GetJsonAlertViewAsync(tag);
            return View("Alerts", json);
        }

        [HttpGet("/Home/SearchAlertAsync/{tag ?}")]
        public async Task<ActionResult> SearchAlertAsync(string tag)
        {
            if (tag == null)
            {
                return RedirectToAction("Alerts");
            }
            JsonAlertViewModel json = await GetJsonAlertViewAsync(tag);
            return View("Alerts", json);
        }

        [Route("/Home/AlertsByUserAsync/{Id}")]
        public async Task<ActionResult> AlertsByUserAsync(string Id)
        {
            IQueryable<Alert> alerts = (from row in applicationDbContext.Alerts
                                        where row.CreatedById.Equals(Id)
                                        select row);
            JsonAlertViewModel json = await GetJsonAlert(alerts);
            return View("Alerts", json);
        }

        [HttpPost]
        public async Task<string> Subscribe([FromBody] JObject jObject)
        {
            string userId = jObject.GetValue("UserId").Value<string>();
            User user = GetUser(userId);
            User loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            Follow follow = new Follow
            {
                SubscriberId = loggedInUser.Id,
                SubscribedToId = user.Id
            };

            user.NumberOfSubscribers++;

            applicationDbContext.Follows.Add(follow);
            applicationDbContext.Users.Update(user);
            applicationDbContext.SaveChanges();
            

            return JsonConvert.SerializeObject("UnSubscribe");
        }

        [HttpPost]
        public async Task<string> UnSubscribe([FromBody] JObject jObject)
        {
            string userId = jObject.GetValue("UserId").Value<string>();
            User user = GetUser(userId);
            User loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            var subs = IsSubscribedTo(loggedInUser.Id, user.Id);

            applicationDbContext.Follows.Remove(subs);
            user.NumberOfSubscribers--;
            applicationDbContext.Users.Update(user);
            applicationDbContext.SaveChanges();
            return JsonConvert.SerializeObject("Subscribe");
        }

        [HttpPost]
        public async Task<ActionResult> ViewProfile(IFormCollection formFields)
        {
            string userId = formFields["id"];
            User user = GetUser(userId);
            return await RedirectToProfile(user);
        }

        [HttpGet("/Home/ViewProfileGet/{userId ?}")]
        public async Task<ActionResult> ViewProfileGet(string userId)
        {
            if (userId == null) return RedirectToAction("Alerts");
            User user = GetUser(userId);
            return await RedirectToProfile(user);
        }


        [HttpPost]
        public string GetAlert([FromBody] JObject jObject)
        {
            var alertId = jObject.GetValue("AlertId").Value<int>();
            Alert alert = GetAlert(alertId);

            var alerts = GetAlertsBySimilarity(alert);

            foreach (var _alert in alerts)
            {
                _alert.UserAlertStrings = GetGeoJson(_alert);
                _alert.CreatedBy = GetUser(_alert.CreatedById);
            }
            
            return JsonConvert.SerializeObject(alerts);
        }

        [HttpPost]
        public async Task<string> AddCommentAsync([FromBody] JObject jObject)
        {
            int AlertId = jObject.GetValue("AlertId").Value<int>();
            string Comment = jObject.GetValue("Comment").Value<string>().Trim();

            Alert alert = (from row in applicationDbContext.Alerts where AlertId == row.Id select row).FirstOrDefault();
            alert.NumberOfComments++;

            User user = await _userManager.GetUserAsync(HttpContext.User);
            Comment comment = new Comment
            {
                AlertId = AlertId,
                CommentContent = Comment,
                CommentedAt = DateTime.Now,
                CommentedBy = user.Id
            };

            if (!user.Id.Equals(alert.CreatedById))
            {
                AddNotification(user, alert, "commented");
            }
            

            if (comment.CommentContent != null && comment.CommentContent != "")
            {
                applicationDbContext.Alerts.Update(alert);
                applicationDbContext.Comments.Add(comment);
                applicationDbContext.SaveChanges();
            }

            return JsonConvert.SerializeObject(GetComments(AlertId));
        }

        [HttpPost]
        public string GetComments([FromBody] JObject jObject)
        {
            int AlertId = jObject.GetValue("AlertId").Value<int>();
            
            return JsonConvert.SerializeObject(GetComments(AlertId));
        }

        [HttpPost]
        public string FetchAlertImage([FromBody] JObject jObject)
        {
            int AlertId = jObject.GetValue("AlertId").Value<int>();

            return JsonConvert.SerializeObject(Convert.ToBase64String(GetAlert(AlertId).Picture));
        }

        [HttpPost]
        public async Task<string> DeleteAlertAsync([FromBody] JObject jObject)
        {
            int AlertId = jObject.GetValue("AlertId").Value<int>();

            Alert alert = GetAlert(AlertId);

            User user = await _userManager.GetUserAsync(HttpContext.User);
            if (alert.CreatedById.Equals(user.Id))
            {
                applicationDbContext.Alerts.Remove(alert);
                applicationDbContext.SaveChanges();
            }

            return JsonConvert.SerializeObject("success");
        } 
        
        [HttpPost]
        public async Task<string> DeleteCommentAsync([FromBody] JObject jObject)
        {
            int commentId = jObject.GetValue("CommentId").Value<int>();
            int AlertId = jObject.GetValue("AlertId").Value<int>();

            Alert alert = GetAlert(AlertId);

            Comment comment = (from row in applicationDbContext.Comments where row.Id == commentId select row).FirstOrDefault();
            User user = await _userManager.GetUserAsync(HttpContext.User);
            if (comment.CommentedBy.Equals(user.Id))
            {
                applicationDbContext.Comments.Remove(comment);
                alert.NumberOfComments--;
                applicationDbContext.Alerts.Update(alert);
                applicationDbContext.SaveChanges();
            }

            return JsonConvert.SerializeObject(GetComments(AlertId));
        }

        [HttpPost]
        public async Task<string> FetchNotificationAsync()
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            var notifications = (from notif in applicationDbContext.Notifications
                                 where notif.UserId.Equals(user.Id) && !notif.Seen
                                 select notif).ToList();

            return JsonConvert.SerializeObject(notifications);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult NewAlert()
        {
            return View(new Alert());
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public void AddNotification(User user, Alert alert, string content)
        {
            if (!user.Id.Equals(alert.CreatedById))
            {
                Notification existingNotif = (from notif in applicationDbContext.Notifications
                                              where (notif.AlertId == alert.Id) select notif).FirstOrDefault();
                if (existingNotif != null && user != GetUser(existingNotif.UserId))
                {
                    existingNotif.Seen = false;
                    applicationDbContext.Notifications.Update(existingNotif);
                }
                else
                {
                    Notification notification = new Notification
                    {
                        AlertId = alert.Id,
                        UserId = alert.CreatedById,
                        Seen = false,
                        NotifactionContent = $" {user.FirstName} {content} on your alert."
                    };

                    applicationDbContext.Notifications.Add(notification);
                }
            
                applicationDbContext.SaveChanges();
            }
        }

        public int GetNotificationNumber(User user)
        {
            return (from row in applicationDbContext.Notifications
                    where row.UserId.Equals(user.Id) && !row.Seen select row).Count();
        }
        
        public GeoJson GetGeoJson(Alert alert)
        {
            List<Feature> features = new List<Feature>();

            var userAlerts = (from uAlert in applicationDbContext.UserAlerts where uAlert.AlertId == alert.Id select uAlert);

            features.Add(alertToGeoJson.Convert(new UsersAlerts
            {
                AlertLevel = AlertLevel.AlertLocation,
                Longitude = alert.Longitude,
                Latitude = alert.Latitude
            }));

            foreach (var userAlert in userAlerts)
            {
                features.Add(alertToGeoJson.Convert(userAlert));
            }


            GeoJson geoJson = new GeoJson
            {
                features = features
            };

            return geoJson;

        }

        public double CalculateDistance(double x_1, double x_2, double y_1, double y_2)
        {
            return Math.Sqrt(Math.Pow((x_2 - x_1), 2.0) + Math.Pow((y_2 - y_1), 2.0));
        }

        public async Task<JsonAlertViewModel> GetJsonAlertViewAsync(string tag)
        {
            IQueryable<Alert> alerts = (from row in applicationDbContext.Alerts
                                        where row.Tags.Contains(tag)
                                        select row);
            return await GetJsonAlert(alerts);
        }

        public async Task<JsonAlertViewModel> GetAlertJsonById(int alertId)
        {
            IQueryable<Alert> alerts = (from row in applicationDbContext.Alerts
                                        where row.Id == alertId select row);
            return await GetJsonAlert(alerts);
        }

        public User GetUser(string id)
        {
            return (from row in applicationDbContext.Users where row.Id.Equals(id) select row).FirstOrDefault();
        }

        public Alert GetAlert(int AlertId)
        {
            return (from row in applicationDbContext.Alerts where row.Id == AlertId select row).FirstOrDefault();
        }

        public bool HasSubscribers(string userId)
        {
            var subscriber = (from row in applicationDbContext.Follows
                              where row.SubscribedToId.Equals(userId) select row).FirstOrDefault();
            return subscriber != null;
        }

        public Follow IsSubscribedTo(string subscriber, string subscribedTo)
        {
            var sub = (from row in applicationDbContext.Follows where row.SubscribedToId.Equals(subscribedTo)
                       && row.SubscriberId.Equals(subscriber) select row).FirstOrDefault();

            return sub;
        }

        public IQueryable<Follow> GetFollowers(string subscribedTo)
        {
            return (from row in applicationDbContext.Follows
                    where row.SubscribedToId.Equals(subscribedTo)
                    select row);
        }

        public IQueryable<Follow> GetFollows(string subscriberId)
        {
            return from row in applicationDbContext.Follows
                   where row.SubscriberId.Equals(subscriberId)
                   select row;
        }

        private Follow GetFollow(int id)
        {
            return (from row in applicationDbContext.Follows
                    where id == row.Id select row).FirstOrDefault();
        
        }

        private async Task<ActionResult> RedirectToProfile(User user)
        {
            User loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewProfileViewModel viewProfileView = new ViewProfileViewModel
            {
                User = user,
                IsLoggedInUser = (user.Id.Equals(loggedInUser.Id)),
                LoggedInUserSubscribed = IsSubscribedTo(loggedInUser.Id, user.Id) != null
            };
            return View("ViewProfile", viewProfileView);
        }

        public HashSet<Alert> GetSubscribedAlerts(User user)
        {
            HashSet<Alert> alertsFromSubscription = new HashSet<Alert>();

            List<string> follows = new List<string>();
            var subs = GetFollows(user.Id);

            foreach(var _follow in subs)
            {
                if (_follow.UnSeen)
                {
                    _follow.UnSeen = false;
                    follows.Add(_follow.SubscribedToId);
                }
            }

            if (follows.Count == 0) return alertsFromSubscription;

            var alerts = from row in applicationDbContext.AlertForSubscribers
                         where follows.Contains(row.CreatedBy)
                         select row.AlertId;

            foreach(var alertId in alerts)
            {
                alertsFromSubscription.Add(GetAlert(alertId));
            }

            applicationDbContext.Follows.UpdateRange(subs);
            applicationDbContext.SaveChanges();

            return alertsFromSubscription;
        }

        public List<Comment> GetComments(int AlertId)
        {
            var comments = (from row in applicationDbContext.Comments where row.AlertId == AlertId select row).ToList();

            foreach (var comment in comments)
            {
                var user = GetUser(comment.CommentedBy);
                comment.User = user;
                if (user.Picture != null)
                {
                    comment.PicOfCommento = Convert.ToBase64String(user.Picture);
                }

            }
            return comments;
        }

        public async Task<bool> ValidateImage(byte[] image)
        {
            var Client = new ClarifaiClient("f5e76daef9a448c5949b9746828444e4");

            var response = await Client.PublicModels.GeneralModel.Predict(
                    new ClarifaiFileImage(image))
                .ExecuteAsync();

            if (response.IsSuccessful)
            {
                foreach (var concept in response.Get().Data)
                {
                    if(concept.Name.Equals("sexy") || concept.Name.Equals("beach") || concept.Name.Equals("bikini") || concept.Name.Equals("desire")
                       || concept.Name.Equals("romance") || concept.Name.Equals("brunette"))
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }

        public async Task<JsonAlertViewModel> GetJsonAlert(IQueryable<Alert> alerts)
        {
            List<Alert> readyAlerts = new List<Alert>();


            foreach (var alert in alerts)
            {

                alert.UserAlertStrings = GetGeoJson(alert);
                alert.CreatedBy = GetUser(alert.CreatedById);
                readyAlerts.Add(alert);
            }

            var JsonFormat = JsonConvert.SerializeObject(readyAlerts);

            User user = await _userManager.GetUserAsync(HttpContext.User);

            return new JsonAlertViewModel { Alerts = readyAlerts, JsonFormat = JsonFormat, User = user };
        }

        public List<Alert> GetAlertsBySimilarity(Alert alert)
        {
            HashSet<string> tags = alert.Tags.Split(",").ToHashSet();
            List<Alert> sortedAlerts = new List<Alert> { alert };
            Dictionary<Alert, int> alertSimilarityPairs = new Dictionary<Alert, int>();
            IQueryable<Alert> alerts = (from row in applicationDbContext.Alerts select row);

            foreach (Alert _alert in alerts)
            {
                if (_alert.Id == alert.Id) continue;
                alertSimilarityPairs[_alert] = 0;
                string[] _tags = _alert.Tags.Split(",");

                foreach (var _tag in _tags)
                {
                    if (tags.Contains(_tag))
                    {
                        alertSimilarityPairs[_alert]++;
                    }
                }

            }
            foreach (var AlertValue in alertSimilarityPairs.OrderByDescending(key => key.Value))
            {
                sortedAlerts.Add(AlertValue.Key);
            }
            return sortedAlerts;
        }
    }

}
