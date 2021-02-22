using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskManagement.Models
{
    public class Taskk
    {
        [Key]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "The Task must have a name!")]
        [MaxLength(40, ErrorMessage = "The Name of this Task must be shorter than 40 characters!")]
        public string TaskName { get; set; }
        [Required(ErrorMessage = "The Task must have a content!")]
        [MinLength(30, ErrorMessage = "This Task's Content must contain at least 30 characters!")]
        public string TaskContent { get; set; }
        public string TaskStatus { get; set; }
        [Required(ErrorMessage = "The Task must have a start date!")]
        public DateTime TaskStartDate { get; set; }
        [Required(ErrorMessage = "The Task must have a deadline!")]
        public DateTime TaskDeadline { get; set; }
        public string AssignedUserId { get; set; }

        public int ProjectId { get; set; }

        

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public IEnumerable<SelectListItem> UserNames { get; set; }
    }
}
