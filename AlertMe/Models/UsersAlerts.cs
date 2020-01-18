using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlertMe.Models
{
    public class UsersAlerts
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public int AlertId { get; set; }

        public string UserId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public AlertLevel AlertLevel { get; set; }

        [ForeignKey("UserId")]
        public User AlertedBy { get; set; }

        [ForeignKey("AlertId")]
        public Alert Alert { get; set; }
    }
}
