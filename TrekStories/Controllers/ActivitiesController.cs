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
using System.Globalization;

namespace TrekStories.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public ActivitiesController() { }

        public ActivitiesController(ITrekStoriesContext context)
        {
            db = context;
        }

        // GET: Activities
        //public async Task<ActionResult> Index()
        //{
        //    var activities = db.Activities.Include(a => a.Step);
        //    return View(await activities.ToListAsync());
        //}

        // GET: Activities/Details/5
        public async Task<ActionResult> Details(int? id)
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
                return View("DetailsLeisure", activity);
            }
            else   //Transport
            {
                return View("DetailsTransport", activity);
            }
        }

        //Create methods use same view as Edit methods
        public async Task<ActionResult> CreateLeisure(int? stepId)
        {
            if (stepId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = await db.Steps.FindAsync(stepId);
            if (step == null)
            {
                return HttpNotFound();
            }
            ViewBag.StepId = stepId;
            ViewBag.From = step.From;
            ViewBag.To = step.To;
            ViewBag.Currency = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            return View("EditLeisure", new LeisureActivity());
        }

        public async Task<ActionResult> CreateTransport(int? stepId)
        {
            if (stepId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = await db.Steps.FindAsync(stepId);
            if (step == null)
            {
                return HttpNotFound();
            }
            ViewBag.StepId = stepId;
            ViewBag.From = step.From;
            ViewBag.To = step.To;
            ViewBag.Currency = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            return View("EditTransport", new Transport());
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
            ViewBag.StepId = activity.StepId;
            ViewBag.From = activity.Step.From;
            ViewBag.To = activity.Step.To;
            ViewBag.Currency = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            if (activity is LeisureActivity)
            {
                return View("EditLeisure", activity);
            }
            else   //Transport
            {
                return View("EditTransport", activity);
            }
            //IQueryable<LeisureActivity> bla = from b in db.Activities.OfType<LeisureActivity>() select b;
        }

        // POST: Activities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLeisure([Bind(Include = "ID,Name,StartTime,Price,Notes,Address,LeisureCategory, StepId")] LeisureActivity leisureActivity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //if new activity, add to db
                    if (leisureActivity.ID == 0)
                    {
                        db.Activities.Add(leisureActivity);

                        //REFACTOR THIS METHOD?
                        //update trip budget
                        Step step = await db.Steps.Include(s => s.Trip).FirstOrDefaultAsync(s => s.StepId == leisureActivity.StepId);
                        step.Trip.TotalCost += leisureActivity.Price;
                    }
                    else
                    {
                        LeisureActivity dbEntry = (LeisureActivity)db.Activities.FindAsync(leisureActivity.ID).Result;
                        //update trip budget
                        dbEntry.Step.Trip.TotalCost = dbEntry.Step.Trip.TotalCost - dbEntry.Price + leisureActivity.Price;
                        if (dbEntry != null)
                        {
                            dbEntry.Name = leisureActivity.Name;
                            dbEntry.StartTime = leisureActivity.StartTime;
                            dbEntry.Price = leisureActivity.Price;
                            dbEntry.Notes = leisureActivity.Notes;
                            dbEntry.Address = leisureActivity.Address;
                            dbEntry.LeisureCategory = leisureActivity.LeisureCategory;
                            dbEntry.StepId = leisureActivity.StepId;
                        }
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Step", new { id = leisureActivity.StepId });
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            return View(leisureActivity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTransport([Bind(Include = "ID,Name,StartTime,Price,Notes,TransportType,Company,Destination,Duration,StepId")] Transport transport)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //if new transport, add to db
                    if (transport.ID == 0)
                    {
                        db.Activities.Add(transport);

                        //update trip budget
                        Step step = await db.Steps.Include(s => s.Trip).FirstOrDefaultAsync(s => s.StepId == transport.StepId);
                        step.Trip.TotalCost += transport.Price;
                    }
                    else
                    {
                        Transport dbEntry = (Transport)db.Activities.FindAsync(transport.ID).Result;
                        //update trip budget
                        dbEntry.Step.Trip.TotalCost = dbEntry.Step.Trip.TotalCost - dbEntry.Price + transport.Price;
                        if (dbEntry != null)
                        {
                            dbEntry.Name = transport.Name;
                            dbEntry.StartTime = transport.StartTime;
                            dbEntry.Price = transport.Price;
                            dbEntry.Notes = transport.Notes;
                            dbEntry.TransportType = transport.TransportType;
                            dbEntry.Company = transport.Company;
                            dbEntry.Destination = transport.Destination;
                            dbEntry.Duration = transport.Duration;
                            dbEntry.StepId = transport.StepId;
                        }
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Step", new { id = transport.StepId });
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            return View(transport);
        }

        // GET: Activities/Delete/5
        //public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    if (saveChangesError.GetValueOrDefault())
        //    {
        //        ViewBag.ErrorMessage = "Delete failed. Please try again, and if the problem persists, contact the system administrator.";
        //    }
        //    Activity activity = await db.Activities.FindAsync(id);
        //    if (activity == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(activity);
        //}

        // POST: Activities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int actId)
        {
            Activity activityToDelete = await db.Activities.FindAsync(actId);
            if (activityToDelete != null)
            {
                db.Activities.Remove(activityToDelete);

                //update trip budget
                Step step = await db.Steps.FindAsync(activityToDelete.StepId);
                step.Trip.TotalCost -= activityToDelete.Price;

                await db.SaveChangesAsync();
                TempData["message"] = string.Format("Activity '{0}' was deleted successfully.", activityToDelete.Name);
            }
            return RedirectToAction("Details", "Step", new { id = activityToDelete.StepId });
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
