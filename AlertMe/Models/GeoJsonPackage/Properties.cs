using System.ComponentModel.DataAnnotations;

namespace AlertMe.Models
{
    public class Properties
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public AlertLevel AlertLevel { get; set; }
    }
}