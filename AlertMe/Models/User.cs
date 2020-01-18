using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AlertMe.Models
{
    public class User : IdentityUser
    {
        
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Name field is required")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name field is required")]
        public string LastName { get; set; }

        [Display(Name = "Date of birth")]
        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Please enter your city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter your state")]
        public string State { get; set; }

        [Display(Name = "Upload profile picture")]
        public byte[] Picture { get; set; }

        public int NumberOfSubscribers { get; set; }

    }
}
