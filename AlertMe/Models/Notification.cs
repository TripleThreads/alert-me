
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlertMe.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string NotifactionContent { get; set; }

        public bool Seen { get; set; }

        public int AlertId { get; set; }

        public string UserId { get; set; }

        [ForeignKey("AlertId")]
        public Alert Alert { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
