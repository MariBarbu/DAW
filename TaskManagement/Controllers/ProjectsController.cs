using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;
//using AppContext = TaskManagement.Models.AppContext;


namespace TaskManagement.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Projects
        [Authorize(Roles ="Admin,Editor,User")]
        public ActionResult Index()
        {
            var projects = from project in db.Projects.Include("User")
                             orderby project.ProjectTitle
                             select project;
            ViewBag.Projects = projects;
            if(TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            ViewBag.showButtonsp = false;

            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.showButtonsp = true;
            }

            ViewBag.isAdminp = User.IsInRole("Admin");
            ViewBag.currentUserp = User.Identity.GetUserId();
            ViewBag.currentTeamId = db.Users.Find(User.Identity.GetUserId()).TeamId;
            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(int id)
        {
            Project project = db.Projects.Find(id);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            ViewBag.showButtons = false;

            if(User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.showButtons = true;
            }
            project.TeamNames = GetAllTeams();
            ViewBag.ListOfTeams = project.TeamNames; 

            ViewBag.isAdmin = User.IsInRole("Admin");
            ViewBag.currentUser = User.Identity.GetUserId();

            return View(project);
        }

        [Authorize(Roles = "Editor,Admin,User")]
        public ActionResult New()
        {
            Project project = new Project();

            //preluam ID-ul utilizatorului curent
            project.UserId = User.Identity.GetUserId();
            var currentUserId = User.Identity.GetUserId();

            project.TeamId = 4;

            if(User.IsInRole("User"))
            {
                ApplicationDbContext context = new ApplicationDbContext();

                var roleManager = new RoleManager<IdentityRole>(new
                   RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new
               UserStore<ApplicationUser>(context));
                var roles = from role in db.Roles select role;
                foreach (var role in roles)
                {
                    UserManager.RemoveFromRole(currentUserId, role.Name);
                }
                UserManager.AddToRole(currentUserId, "Editor");
                db.SaveChanges();
                return View(project);
            }

            return View(project);
        }
        
        [HttpPost]
        [Authorize(Roles = "Editor,Admin,User")]
        public ActionResult New(Project project)
        {
            project.UserId = User.Identity.GetUserId();
            project.TeamId = 4;
            try
            {
                if(ModelState.IsValid)
                {
                    db.Projects.Add(project);
                    db.SaveChanges();
                    TempData["message"] = "The project has been added!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(project);
                }
            }
            catch (Exception)
            {
                return View(project);
            }
        }

        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Project project = db.Projects.Find(id);

            project.TeamNames = GetAllTeams();

            if(project.UserId==User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(project);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit this Project!";
                return Redirect("/Projects/Show/"+project.ProjectId);
            }
        }


        [HttpPut]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id, Project requestProject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Project project = db.Projects.Find(id);
                    if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(project))
                        {
                            project.ProjectTitle = requestProject.ProjectTitle;
                            project.Description = requestProject.Description;
                            project.ProjectDeadline = requestProject.ProjectDeadline;
                            project.TeamId = requestProject.TeamId;
                            db.SaveChanges();
                            TempData["message"] = "The project has been updated!";
                            return RedirectToAction("Index");
                        }
                        return View(requestProject);
                    }
                    else
                    {
                        TempData["message"] = "You are not allowed to edit this Project!";
                        return Redirect("/Projects/Show/" + project.ProjectId);
                    }
                }
                else
                {
                    requestProject.TeamNames = GetAllTeams();
                    return View(requestProject);
                }
                   
            }
            catch (Exception)
            {
                requestProject.TeamNames = GetAllTeams();
                return View(requestProject);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Projects.Remove(project);
                db.SaveChanges();
                TempData["message"] = "The project has been deleted!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You are not allowed to delete this Project!";
                return Redirect("/Projects/Show/" + project.ProjectId);
            }
          
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllTeams()
        {
            var selectTeam = new List<SelectListItem>();
            var teams = from team in db.Teams
                        select team;
            foreach (var team in teams)
            {
                selectTeam.Add(new SelectListItem
                {
                    Value = team.TeamId.ToString(),
                    Text = team.TeamName.ToString(),
                });
            }
            return selectTeam;
        }

    }
}