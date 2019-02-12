using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlertMe.Models;
using Microsoft.EntityFrameworkCore;
using AlertMe.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using AlertMe.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using AlertMe.Data;

namespace Alert_Me.Controllers
{

    public class UserController : Controller
    {
        SignInManager<User> signInManager;
        public UserController(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
         
        ApplicationDbContext applicationDbContext = GetApplicationDbContext.GetApplication();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public IActionResult Index()
        {
            return View();
        }

        #region register

        [HttpPost]
        public JsonResult GetCity([FromBody] JObject data)
        {
            if (data == null)
            {
                return Json(null);
            }
            int id = data.GetValue("state").Value<int>();
            string city = data.GetValue("state").Value<string>();
            var cities = (from row in applicationDbContext.States where row.Id == id select row.Cities).FirstOrDefault();
            
            return Json(cities);
        }

        [Route("/user/register")]
        public IActionResult Register()
        {
            StateUserViewModel stateUserViewModel = new StateUserViewModel
            {
                States = applicationDbContext.States.ToList()
            };
            return View(stateUserViewModel);
        }

        [Route("Users/Create")]
        public async Task<IActionResult> Create(User user) {

            if (!ModelState.IsValid)
            {
                StateUserViewModel stateUserViewModel = new StateUserViewModel
                {
                    States = applicationDbContext.States.ToList(),
                    User = user
                };

                return View("Register", stateUserViewModel);
            }

            applicationDbContext.SaveChanges();

            return await SignInUser(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

            if (result.Succeeded)
            {
                
                    return RedirectToAction("Index", "Home");
                
            }
        
            ModelState.AddModelError("","Invalid login attempt"); 
            return View(model);
        }

        private async Task<IActionResult> SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Alerts", "Home");
        }

        public IActionResult Login()
        {
            return View(new LoginModel());
        }


        #endregion

        public IActionResult Logout()
        {
            return View();
        }

    }
}
