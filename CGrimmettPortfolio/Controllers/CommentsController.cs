using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CGrimmettPortfolio.Models;
using Microsoft.AspNet.Identity;

namespace CGrimmettPortfolio.Controllers
{
    //[RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments/Edit/5
        [Authorize]
        //[RequireHttps]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comment.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("Admin") || User.IsInRole("Moderator") || user.Id == comment.AuthorId)
            {
                return View(comment);
            }
            else
            {
                var post = db.Post.Find(comment.PostId);
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        //[RequireHttps]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,AuthorId,Body,CreationDate,UpdatedDate,UpdatedReason")] Comment comment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var post = db.Post.Find(comment.PostId);
            if (User.IsInRole("Admin") || User.IsInRole("Moderator") || user.Id == comment.AuthorId)
            {
                if (ModelState.IsValid)
                {
                    comment.UpdatedDate = System.DateTime.Now;
                    db.Entry(comment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
                }
                return View(comment);
            }
            else
            {
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }
        }

        // GET: Comments/Delete/5
        [Authorize]
        //[RequireHttps]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comment.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("Admin") || User.IsInRole("Moderator") || user.Id == comment.AuthorId)
            {
                return View(comment);
            }
            else
            {
                var post = db.Post.Find(comment.PostId);
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }

        }

        // POST: Comments/Delete/5
        //[RequireHttps]
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comment.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            var post = db.Post.Find(comment.PostId);
            if (User.IsInRole("Admin") || User.IsInRole("Moderator") || user.Id == comment.AuthorId)
            {
                db.Comment.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }
            else
            {
                return RedirectToAction("Details", "Posts", new { Slug = post.Slug });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
