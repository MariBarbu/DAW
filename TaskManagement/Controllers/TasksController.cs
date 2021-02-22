using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;
//using AppContext = TaskManagement.Models.AppContext;


namespace TaskManagement.Controllers
{
    public class TasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Tasks
        [Authorize(Roles ="User,Editor,Admin")]
        public ActionResult Index()
        {

            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(int id)
        {

            Taskk task = db.Tasks.Find(id);
            task.UserNames = GetAllUsers(task.Project.TeamId);
            ViewBag.ListOfUsers = task.UserNames;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            SetAccessRights();

            return View(task);
        }

        [HttpPost]
        public ActionResult Show(Comment comm)
        {
            comm.CommentDate = DateTime.Now;
            comm.UserId = User.Identity.GetUserId();
            try
            {
                if(ModelState.IsValid)
                {
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    return Redirect("/Tasks/Show/" + comm.TaskId);
                }
                else
                {
                    Taskk task = db.Tasks.Find(comm.TaskId);
                    SetAccessRights();
                    return View(task);
                }
            }
            catch (Exception)
            {
                Taskk task = db.Tasks.Find(comm.TaskId);
                SetAccessRights();
                return View(task);
            }

        }

        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New(int id)
        {
            Taskk task = new Taskk();
            task.ProjectId = id;
            var project = db.Projects.Find(id);
            task.AssignedUserId = "test";
            ViewBag.ProjectTitle = project.ProjectTitle;
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                // task.Projects = GetAllProjects();
                task.UserId = User.Identity.GetUserId();
                return View(task);
            }
            else
            {
                TempData["message"] = "You are not allowed to add a new Task!";
                return Redirect("/Projects/Show/" + project.ProjectId);
            }
            
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New(Taskk task)
        {
            task.UserId = User.Identity.GetUserId();
            var project = db.Projects.Find(task.ProjectId);
            task.AssignedUserId = "test";
            try
            {
                if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    db.Tasks.Add(task);
                    db.SaveChanges();
                    TempData["message"] = "The task has been added!";
                    return Redirect("/Projects/Show/" + task.ProjectId);
                }
                else
                {
                    TempData["message"] = "You are not allowed to add a new Task!";
                    return Redirect("/Projects/Show/" + task.Project.ProjectId);
                }

            }
            catch (Exception)
            {
                return View(task);
            }
        }

        [Authorize(Roles = "Editor,Admin,User")]
        public ActionResult Edit(int id)
        {
            Taskk task = db.Tasks.Find(id);
            task.UserNames = GetAllUsers(task.Project.TeamId);
            if (task.UserId == User.Identity.GetUserId() || User.IsInRole("Admin")||User.IsInRole("User"))
            {
                return View(task);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit this Task!";
                return Redirect("/Tasks/Show/" + task.TaskId);
            }

        }


        [HttpPut]
        [Authorize(Roles = "Editor,Admin,User")]
        public ActionResult Edit(int id, Taskk requestTask )
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Taskk task = db.Tasks.Find(id);
                    if (task.UserId == User.Identity.GetUserId() || User.IsInRole("Admin")||User.IsInRole("User"))
                    {
                        if (TryUpdateModel(task))
                        {

                            task.TaskName = requestTask.TaskName;
                            task.TaskContent = requestTask.TaskContent;
                            task.TaskDeadline = requestTask.TaskDeadline;
                            task.TaskStatus = requestTask.TaskStatus;
                            task.TaskStartDate = requestTask.TaskStartDate;
                            task.ProjectId = requestTask.ProjectId;
                            db.SaveChanges();
                            TempData["message"] = "The task has been updated!";
                            return Redirect("/Projects/Show/" + task.ProjectId);
                        }
                        else
                        {
                            return View(requestTask);
                        }
                    }
                    else
                    {
                        TempData["message"] = "You are not allowed to edit this Task!";
                        return Redirect("/Tasks/Show/" + task.TaskId);
                    }

                }
                else
                {
                    requestTask.UserNames = GetAllUsers(requestTask.Project.TeamId);
                    return View(requestTask);
                }

            }
            catch (Exception)
            {
                //requestTask.UserNames = GetAllUsers(requestTask.Project.TeamId);
                return View(requestTask);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Taskk task = db.Tasks.Find(id);
            if (task.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
                TempData["message"] = "The task has been deleted!";
                return Redirect("/Projects/Show/" + task.ProjectId);
            }
            else
            {
                TempData["message"] = "You are not allowed to delete this Task!";
                return Redirect("/Tasks/Show/" + task.TaskId);
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllUsers(int idEchipa)
        {
            var selectUser = new List<SelectListItem>();
            var users = from user in db.Users
                        where (user.TeamId == idEchipa)
                        select user;
            foreach (var user in users)
            {
                selectUser.Add(new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.UserName.ToString(),
                });
            }
            return selectUser;
        }


        private void SetAccessRights()
        {
            ViewBag.showButtonst = false;

            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.showButtonst = true;
            }

            ViewBag.isAdmint = User.IsInRole("Admin");
            ViewBag.currentUsert = User.Identity.GetUserId();
        }
    }

   
}

