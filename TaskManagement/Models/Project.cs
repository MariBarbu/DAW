using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace TaskManagement.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        [Required(ErrorMessage ="The Project must have a title!")][MaxLength(40,ErrorMessage ="The Title of this Project must be shorter than 40 characters!")]
        public string ProjectTitle { get; set; }
        [Required(ErrorMessage ="The Project must have a description!")][MinLength(30, ErrorMessage = "This Project's Description must contain at least 30 characters!")]
        public string Description { get; set; }
        [Required(ErrorMessage = "The Project must have a deadline!")]
        public DateTime ProjectDeadline { get; set; }

        public string UserId { get; set; }

        public int TeamId { get; set; }

        public virtual Team Team { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Taskk> Tasks { get; set; }

        public IEnumerable<SelectListItem> TeamNames { get; set; }
    }
}