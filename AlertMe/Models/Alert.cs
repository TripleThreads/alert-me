using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlertMe.Models
{
    public class Alert
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string CreatedById;

        [Required]
        public string Title { get; set; }

        [Required]
        public string Tags { get; set; }

        [Required(ErrorMessage = "Please specify location of alert.")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Please specify location of alert.")]
        public double Latitude { get; set; }

        [Required]
        public string Description { get; set; }

        public int FalseAlarm { get; set; }

        public int Warning { get; set; }

        public int Critical { get; set; }

        public int NumberOfComments { get; set; }

        [Display(Name = "Upload image")]
        public byte[] Picture { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime LastCheck { get; set; }

        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; }

        [NotMapped]
        public GeoJson UserAlertStrings { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        [NotMapped]
        public string ParsedImage { get; set; }

    }
}
