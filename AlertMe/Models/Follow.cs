using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlertMe.Models
{
    public class Follow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string SubscriberId { get; set; }

        public string SubscribedToId { get; set; }

        public bool UnSeen { get; set; }

        [ForeignKey("SubscriberId")]
        public User Subscriber { get; set; }

        [ForeignKey("SubscribedToId")]
        public User SubscribedTo { get; set; }
    }
}
