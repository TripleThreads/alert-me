using AlertMe.Models;

namespace AlertMe.ViewModels
{
    public class ViewProfileViewModel
    {
        public User User { get; set; }

        public bool IsLoggedInUser { get; set; }

        public bool LoggedInUserSubscribed { get; set; }

    }
}
