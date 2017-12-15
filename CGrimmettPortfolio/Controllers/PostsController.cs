using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CGrimmettPortfolio.Models;
using System.IO;
using CGrimmettPortfolio.Helpers;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;


namespace CGrimmettPortfolio.Controllers
{
    [RequireHttps]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index(int? page, int? Id)
        {
            int pageSize = 3;      //Number of items to populate on page
            int pageNumber = (page ?? 1);
            Post post = db.Post.Find(Id);

            return View(db.Post.OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize));
        }

        //Search Method
        public ActionResult SearchResult(string search, int? page, int? Id)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            Post post = db.Post.Find(Id);
            ViewBag.Search = search;

            return View(db.Post.Where(p => p.Title.Contains(search) || p.Slug.Contains(search) || p.Body.Contains(search) /*|| p.Comments.Contains(search)*/)
                .OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize));
        }

        // GET: Posts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.FirstOrDefault(p => p.Slug == Slug);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CreationDate,UpdatedDate,Title,Body,Media")] Post post, HttpPostedFileBase image)
        {

            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invaild Format.");
            }
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.UrlFriendly(post.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(post);
                }
                if (db.Post.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(post);
                }


                if (image != null)
                {

                    var filepath = "/template/assets/";
                    var absPath = Server.MapPath("~" + filepath);

                    if(post.Media != string.Empty)
                    { 
                    post.Media = filepath + image.FileName;
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                    post.CreationDate = System.DateTime.Now;
                    }
                }
                   post.Slug = Slug;
                   db.Post.Add(post);
                   db.SaveChanges();
                   return RedirectToAction("Index");
            } 
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CreationDate,UpdatedDate,Title,Body,Media")] Post post, string media, string Slug, HttpPostedFileBase image)
        {

            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invaild Format.");
            }

            if (ModelState.IsValid)
            {

                if (image != null)
                {

                    var filepath = "/template/assets/New Imgs/";
                    var absPath = Server.MapPath("~" + filepath);
                    post.Media = filepath + image.FileName;
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }

                else
                {
                    post.Media = media;
                }
                post.Slug = Slug;
                post.UpdatedDate = System.DateTime.Now;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(Id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Post post = db.Post.Find(id);
            db.Post.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        //[RequireHttps]
        [ValidateAntiForgeryToken]
        public ActionResult CommentCreate([Bind(Include = "Id,PostId,Body,Created,AuthorId")] Comment comment)
        { // only pass in the bind the attributes that have forms
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(comment.Body))
                {
                    var post = db.Post.Find(comment.PostId);
                    comment.CreationDate = System.DateTime.Now;
                    comment.AuthorId = User.Identity.GetUserId();
                    db.Comment.Add(comment);
                    db.SaveChanges();

                    
                    return RedirectToAction("Details", new { Slug = post.Slug });
                }
            }
            
            return RedirectToAction("Index");
        }
    }
}
