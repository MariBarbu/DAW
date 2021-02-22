using TaskManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
//using AppContext = TaskManagement.Models.AppContext;

namespace TaskManagement.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="Editor,Admin,User")]
        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "You are not allowed to edit this Comment!";
                return Redirect("/Tasks/Show/" + comm.TaskId);
            }
                
        }

        [HttpPut]
        [Authorize(Roles ="Editor,Admin,User")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comm = db.Comments.Find(id);

                if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(comm))
                    {
                        comm.CommentContent = requestComment.CommentContent;
                        db.SaveChanges();
                    }
                        return Redirect("/Tasks/Show/" + comm.TaskId);
                }
                else
                {
                    TempData["message"] = "You are not allowed to edit this Comment!";
                    return Redirect("/Tasks/Show/" + comm.TaskId);
                }

            }
            catch (Exception)
            {
                return View(requestComment);
            }

        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Admin,User")]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + comm.TaskId);
            }
            else
            {
                TempData["message"] = "You are not allowed to delete this Comment!";
                return Redirect("/Tasks/Show/" + comm.TaskId);
            }
        }




    }
}