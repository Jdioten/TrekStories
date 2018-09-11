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

namespace TrekStories.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public ReviewController() { }

        public ReviewController(ITrekStoriesContext context)
        {
            db = context;
        }

        // GET: Review
        public async Task<ActionResult> Index()
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
        public ActionResult Create()
        {
            ViewBag.ReviewId = new SelectList(db.Steps, "StepId", "From");
            return View();
        }

        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //will most probably need a view model!
        public async Task<ActionResult> Create([Bind(Include = "Rating,PrivateNotes,PublicNotes")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ReviewId = new SelectList(db.Steps, "StepId", "From", review.ReviewId);
            return View(review);
        }

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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ReviewId,Rating,PrivateNotes,PublicNotes,StepId")] Review review)
        {
            if (ModelState.IsValid)
            {
                //Use View Model!!
                //db.Entry(review).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ReviewId = new SelectList(db.Steps, "StepId", "From", review.ReviewId);
            return View(review);
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
