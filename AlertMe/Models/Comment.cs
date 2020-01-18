using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlertMe.Models
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CommentedBy { get; set; }

        public int AlertId { get; set; }

        public string CommentContent { get; set; }

        public DateTime CommentedAt { get; set; }

        [ForeignKey("CommentedBy")]
        public User User { get; set; }

        [ForeignKey("AlertId")]
        public Alert Alert { get; set; }

        [NotMapped]
        public string PicOfCommento { get; set; }


    }
}
