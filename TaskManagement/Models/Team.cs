using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskManagement.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        [Required(ErrorMessage = "The Team must have a name!")]
        [MaxLength(30, ErrorMessage = "The Name of this Team must be shorter than 30 characters!")]
        public string TeamName { get; set; }
        [Required(ErrorMessage = "The Team must have a motto!")]
        [MinLength(20, ErrorMessage = "The Motto of this Team must be longer than 20 characters!")]
        public string Motto { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}