using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace AlertMe.Models
{
    public class Geometry
    {
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Required]
        public string type = "Point";

        [JsonIgnore]
        [Required]
        public double Longtude { get; set; }

        [JsonIgnore]
        [Required]
        public double Latitude { get; set; }

        [NotMapped]
        public double[] coordinates;
    }
}