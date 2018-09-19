using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrekStories.DAL;
using TrekStories.Models;
using TrekStories.Abstract;
using System.IO;
using TrekStories.Utilities;
using Microsoft.AspNet.Identity;

namespace TrekStories.Controllers
{
    [RequireHttps]
    [Authorize]
    public class ReviewController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();
        private BlobUtility utility;

        public ReviewController()
        {
            utility = new BlobUtility();
        }

        public ReviewController(ITrekStoriesContext context)
        {
            db = context;
            utility = new BlobUtility();
        }

        // GET: Review
        public async Task<ActionResult> Index()  //passing trip id??
        {
            var reviews = db.Reviews.Include(r => r.Step);
            return View(await reviews.ToListAsync());
        }

        // GET: Review/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Review/Create
        public async Task<ActionResult> Create(int? id)  //stepId
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = await db.Steps.FindAsync(id);
            if (step == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the step you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            ViewBag.StepId = id;
            ViewBag.From = step.From;
            ViewBag.To = step.To;
            return View("Edit", new Review());
        }

        //// POST: Review/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        ////will most probably need a view model!
        //public async Task<ActionResult> Create([Bind(Include = "Rating,PrivateNotes,PublicNotes")] Review review)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Reviews.Add(review);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ReviewId = new SelectList(db.Steps, "StepId", "From", review.ReviewId);
        //    return View(review);
        //}

        // GET: Review/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReviewId = new SelectList(db.Steps, "StepId", "From", review.ReviewId);
            return View(review);
        }

        // POST: Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ReviewId,Rating,PrivateNotes,PublicNotes,StepId")] Review review)
        {
            if (ModelState.IsValid)
            {
                if (review.ReviewId == 0)
                {
                    db.Reviews.Add(review);
                }
                else
                {
                    db.MarkAsModified(review);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.StepId = review.StepId;
                ViewBag.From = review.Step.From;
                ViewBag.To = review.Step.To;
                return View(review);
            }    
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string containerName = "trekstories-reviewimages-blobcontainer"; //hardcoded container name for review images 
                file = file ?? Request.Files["file"];

                //check file size and extension
                //... using methods from fileuploadutility

                string fileName = Path.GetFileName(file.FileName);
                Stream imageStream = file.InputStream;
                var result = utility.UploadBlob(fileName, containerName, imageStream);
                if (result != null)
                {
                    //change below to just add image url to list of url string?
                    
                    //string loggedInUserId = User.Identity.GetUserId();
                    //UserImage userimage = new UserImage();
                    //userimage.Id = new Random().Next().ToString();
                    //userimage.UserId = loggedInUserId;
                    //userimage.ImageUrl = result.Uri.ToString();
                    //db.UserImages.Add(userimage);
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Review/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Review review = await db.Reviews.FindAsync(id);
            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
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
    }
}
