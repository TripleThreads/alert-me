using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlertMe.Models
{
    public class AlertForSubscribed
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AlertId { get; set; }

        public string CreatedBy { get; set; }

        [ForeignKey("AlertId")]
        public Alert Alert { get; set; }

        [ForeignKey("CreatedBy")]
        public User User { get; set; }
    }
}
