using AlertMe.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlertMe.ViewModels
{
    public class JsonAlertViewModel
    {

        public List<Alert> Alerts { get; set; }

        public string JsonFormat { get; set; }

        public User User { get; set; }

        public int NotificationsNumber { get; set; }

    }
}