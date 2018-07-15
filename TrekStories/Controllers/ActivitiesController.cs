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
    public class ActivitiesController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public ActivitiesController() { }

        public ActivitiesController(ITrekStoriesContext context)
        {
            db = context;
        }

        // GET: Activities
        public async Task<ActionResult> Index()
        {
            var activities = db.Activities.Include(a => a.Step);
            return View(await activities.ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = await db.Activities.FindAsync(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        //Create methods use same view as Edit methods
        public ActionResult CreateLeisure()
        {
            return View("EditLeisure", new LeisureActivity());
        }

        public ActionResult CreateTransport()
        {
            return View("EditTransport", new Transport());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateLeisure([Bind(Include = "Name,StartTime,Price,Notes,Address,LeisureCategory")] LeisureActivity leisureActivity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Activities.Add(leisureActivity);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
            }
            //ViewBag.StepId = new SelectList(db.Steps, "StepId", "From", activity.StepId);
            return View(leisureActivity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTransport([Bind(Include = "Name,StartTime,Price,Notes,TrasnportType,Company,Destination,Duration")] Transport transport)
        {
            try { 
                if (ModelState.IsValid)
                {
                    db.Activities.Add(transport);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
            }
            //ViewBag.StepId = new SelectList(db.Steps, "StepId", "From", activity.StepId);
            return View(transport);
        }

        // GET: Activities/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activity = await db.Activities.FindAsync(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            if (activity is LeisureActivity)
            {
                return View("EditLeisure");
            }
            else if (activity is Transport)
            {
                return View("EditTransport");
            }
            //IQueryable<LeisureActivity> bla = from b in db.Activities.OfType<LeisureActivity>() select b;
            ViewBag.StepId = new SelectList(db.Steps, "StepId", "From", activity.StepId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLeisure([Bind(Include = "Name,StartTime,Price,Notes,Address,LeisureCategory")] LeisureActivity leisureActivity)
        {
            if (ModelState.IsValid)
            {
                try { 
                    db.MarkAsModified(leisureActivity);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            //ViewBag.StepId = new SelectList(db.Steps, "StepId", "From", activity.StepId);
            return View(leisureActivity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTransport([Bind(Include = "Name,StartTime,Price,Notes,TrasnportType,Company,Destination,Duration")] Transport transport)
        {
            if (ModelState.IsValid)
            {
                try
                { 
                    db.MarkAsModified(transport);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            //ViewBag.StepId = new SelectList(db.Steps, "StepId", "From", activity.StepId);
            return View(transport);
        }

        // GET: Activities/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Please try again, and if the problem persists, contact the system administrator.";
            }
            Activity activity = await db.Activities.FindAsync(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Activity activityToDelete = new Activity() { ID = id };
                db.MarkAsDeleted(activityToDelete);
                await db.SaveChangesAsync();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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
