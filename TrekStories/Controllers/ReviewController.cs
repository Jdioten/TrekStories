using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrekStories.Abstract;
using TrekStories.DAL;
using TrekStories.Models;
using TrekStories.Utilities;

namespace TrekStories.Controllers
{
    [RequireHttps]
    [Authorize]
    public class ReviewController : Controller
    {
        private const string IMAGES_CONTAINER_NAME = "trekstories-reviewimages-blobcontainer";

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

        //// GET: Review
        //public async Task<ActionResult> Index()  //passing trip id??
        //{
        //    var reviews = db.Reviews.Include(r => r.Step);
        //    return View(await reviews.ToListAsync());
        //}

        //// GET: Review/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Review review = await db.Reviews.FindAsync(id);
        //    if (review == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(review);
        //}

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
                                new UnauthorizedAccessException("Oops, the step you are trying to review doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review != null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new ArgumentException("Oops, this step has already been reviewed, please edit the existing comment instead."),
                                "Trip", "Index"));
            }
            ViewBag.StepId = id.Value;
            ViewBag.From = step.From;
            ViewBag.To = step.To;
            ViewBag.Rating = 0;
            ViewBag.PhotoCount = 0;
            return View("Edit", new Review() { StepId = id.Value});
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
            Step step = await db.Steps.FindAsync(id);
            if (step == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the step you are trying to review doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            Review review = await db.Reviews.FindAsync(id);
            ViewBag.Rating = review.Rating;
            ViewBag.StepId = id.Value;
            ViewBag.From = step.From;
            ViewBag.To = step.To;
            ViewBag.PhotoCount = StepController.GetReviewPicturesCount(step);
            return View(review);
        }

        // POST: Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Review review)
        {
            //check if the step belongs to authenticated user!
            Step step = await db.Steps.FindAsync(review.StepId);
            if (step.Trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this review doesn't seem to be yours, you cannot add nor edit it."),
                                "Trip", "Index"));
            }

            if (ModelState.IsValid)
            {
                if (review.ReviewId == 0)
                {
                    review.ReviewId = review.StepId;
                    db.Reviews.Add(review);
                }
                else
                {
                    db.MarkAsModified(review);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Step", new { id = review.StepId });
            }
            else
            {
                ViewBag.StepId = review.StepId;
                ViewBag.From = step.From;
                ViewBag.To = step.To;
                return View(review);
            }    
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage(HttpPostedFileBase file, int revId)
        {
            if (file != null)
            { 
                file = file ?? Request.Files["file"];

                //check file size and extension
                if (FileUploadUtility.InvalidFileSize(file))
                {
                    TempData["message"] = "The file cannot be bigger than 7MB.";
                }
                else if (FileUploadUtility.InvalidFileExtension(file))
                {
                    TempData["message"] = "The file type is not authorized for upload.";
                }
                else
                {
                    Review review = await db.Reviews.FindAsync(revId);
                    if (review == null)
                    {
                        return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the review you want to add images to does not exist."),
                                "Trip", "Index"));
                    }

                    //build filename with timestamp to reduce risk of duplicates
                    string fileName = FileUploadUtility.GetFilenameWithTimestamp(file.FileName);

                    Stream imageStream = file.InputStream;
                    CloudBlockBlob result;

                    try
                    {
                        result = utility.UploadBlob(fileName, IMAGES_CONTAINER_NAME, imageStream);    
                    }
                    catch (Exception e)
                    {
                        TempData["message"] = e.Message;
                        return RedirectToAction("Edit", new { id = revId });
                    }
                    Image uploadedImage = new Image { ReviewId = review.ReviewId, Url = result.Uri.ToString() };
                    db.Images.Add(uploadedImage);
                    db.SaveChanges();
                } 
            }
            else
            {
                TempData["message"] = "Please browse for a file to upload.";
            }
            return RedirectToAction("Edit", new { id = revId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteImage(int ImgId)
        {
            Image imageToDelete = await db.Images.FindAsync(ImgId);

            if (imageToDelete == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int revId = imageToDelete.Review.ReviewId;

            //remove from database
            db.Images.Remove(imageToDelete);
            db.SaveChanges();

            //remove from Cloud Storage
            string blobNameToDelete = imageToDelete.Url.Split('/').Last();

            try
            {
                utility.DeleteBlob(blobNameToDelete, IMAGES_CONTAINER_NAME);
            }
            catch (Exception e)
            {
                TempData["message"] = "There was an error when deleting the file from Blob Storage. Error Message: " + e.Message;
            }
            return RedirectToAction("Edit", new { id = revId });
        }

        //// GET: Review/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Review review = await db.Reviews.FindAsync(id);
        //    if (review == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(review);
        //}

        //// POST: Review/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Review review = await db.Reviews.FindAsync(id);
        //    db.Reviews.Remove(review);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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
