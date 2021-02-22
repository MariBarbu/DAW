using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManagement.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required (ErrorMessage ="The comment must have a content!")]
        public string CommentContent { get; set; }
        public DateTime CommentDate { get; set; }

        public int TaskId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}