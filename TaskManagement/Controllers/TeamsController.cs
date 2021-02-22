using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    public class TeamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Teams
        [Authorize(Roles = "Admin,Editor,User")]
        public ActionResult Index()
        {
            var teams = from team in db.Teams.Include("User")
                           orderby team.TeamId
                           select team;
            ViewBag.Teams = teams;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            var currentUserte = User.Identity.GetUserId();
            var user = db.Users.Find(currentUserte);
            ViewBag.teamId = user.TeamId;
            if (User.IsInRole("Admin"))
                ViewBag.rolul = 1;

            return View();
        }
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(int id)
        {
            Team team = db.Teams.Find(id);
            ViewBag.Projects = from project in db.Projects
                            where (project.TeamId == team.TeamId)
                            select project;
            ViewBag.Users = from user in db.Users
                         where (user.TeamId == team.TeamId)
                         select user;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
             
            ViewBag.showButtons = false;

            if (team.UserId==User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.showButtons = true;
            }

            ViewBag.isAdmin = User.IsInRole("Admin");
            ViewBag.currentUser = User.Identity.GetUserId();

            return View(team);
        }

        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New()
        {
            Team team = new Team();
            if (User.IsInRole("Editor")|| User.IsInRole("Admin"))
            {
                team.UserId = User.Identity.GetUserId();
                return View(team);
            }
            else
            {
                TempData["message"] = "You are not allowed to add a new Team!";
                return Redirect("/Teams/Index");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New(Team team)
        {
            team.UserId = User.Identity.GetUserId();
            try
            {
                if (User.IsInRole("Editor")|| User.IsInRole("Admin"))
                {
 
                    team.UserId = User.Identity.GetUserId();
                    db.Teams.Add(team);
                    db.SaveChanges();
                    //team.User.TeamId = team.TeamId;
                    TempData["message"] = "The team has been added!";
                    return Redirect("/Teams/Index");
                }
                else
                {
                    TempData["message"] = "You are not allowed to add a new Team!";
                    return Redirect("/Teams/Index");
                }

            }
            catch (Exception)
            {
                return View(team);
            }
        }

        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Team team = db.Teams.Find(id);

            if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(team);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit this Team!";
                return Redirect("/Teams/Show/" + team.TeamId);
            }
        }


        [HttpPut]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id, Team requestTeam)
        {
            try
            {
                Team team = db.Teams.Find(id);

                if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(team))
                    {
                        team.TeamName = requestTeam.TeamName;
                        team.Motto = requestTeam.Motto;

                        db.SaveChanges();
                        TempData["message"] = "The Team has been updated!";
                        return Redirect("/Teams/Show/" + team.TeamId);
                    }
                    return View(requestTeam);
                }
                else
                {
                    TempData["message"] = "You are not allowed to edit this Team!";
                    return Redirect("/Teams/Show/" + team.TeamId);
                }

            }
            catch (Exception)
            {
                return View(requestTeam);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Team team = db.Teams.Find(id);
            if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Teams.Remove(team);
                db.SaveChanges();
                TempData["message"] = "The team has been deleted!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You are not allowed to delete this Team!";
                return Redirect("/Teams/Show/" + team.TeamId);
            }

        }
        /*
        [NonAction]
        public IEnumerable<SelectListItem> GetTeamUsers()
        {
        }
        */
    }
}