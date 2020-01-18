using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlertMe.Models
{
    public class Feature
    {

        [Key, Column(Order = 0)]
        public int Id { get; set; }

        public string type = "Feature";

        [Required]
        public Properties properties { get; set; }

        [Required]
        public Geometry geometry { get; set; }
    }
}