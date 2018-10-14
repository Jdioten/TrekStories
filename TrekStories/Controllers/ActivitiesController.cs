using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Abstract;
using TrekStories.DAL;
using TrekStories.Models;
using TrekStories.Utilities;

namespace TrekStories.Controllers
{
    [RequireHttps]
    [Authorize]
    public class ActivitiesController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public ActivitiesController() { }

        public ActivitiesController(ITrekStoriesContext context)
        {
            db = context;
        }

        const string NULL_ACTIVITY_ERROR = "Oops, the activity you are looking for doesn't seem to exist. Please try navigating to the main page again.";
        const string NULL_STEP_ERROR = "Oops, the step you want to add an activity to doesn't seem to exist. Please try navigating to the main page again.";

        // GET: Activities/Details/5
        public async Task<ActionResult> Details(int id = 1)
        {
            var activity = await db.Activities.FindAsync(id);
            if (activity == null)
            {
                return View("CustomisedError", new HandleErrorInfo( new UnauthorizedAccessException(NULL_ACTIVITY_ERROR), "Trip", "Index"));
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
                return View("CustomisedError", new HandleErrorInfo(new UnauthorizedAccessException(NULL_STEP_ERROR), "Trip", "Index"));
            }
            ViewBag.StepId = stepId;
            ViewBag.From = step.From;
            ViewBag.To = step.To;
            ViewBag.Currency = CurrencyHelper.GetCurrency();
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
                return View("CustomisedError", new HandleErrorInfo(new UnauthorizedAccessException(NULL_STEP_ERROR), "Trip", "Index"));
            }
            ViewBag.StepId = stepId;
            ViewBag.From = step.From;
            ViewBag.To = step.To;
            ViewBag.Currency = CurrencyHelper.GetCurrency();
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
                return View("CustomisedError", new HandleErrorInfo(new UnauthorizedAccessException(NULL_ACTIVITY_ERROR), "Trip", "Index"));
            }
            
            ViewBag.StepId = activity.StepId;
            ViewBag.From = activity.Step.From;
            ViewBag.To = activity.Step.To;
            ViewBag.Currency = CurrencyHelper.GetCurrency();
            if (activity is LeisureActivity)
            {
                return View("EditLeisure", activity);
            }
            else   //Transport
            {
                return View("EditTransport", activity);
            }
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

                        //update trip budget
                        Step step = await db.Steps.Include(s => s.Trip).FirstOrDefaultAsync(s => s.StepId == leisureActivity.StepId);
                        if (step.Trip.TripOwner != User.Identity.GetUserId())
                        {
                            return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this step doesn't seem to be yours, you cannot add an activity to it."),
                                "Trip", "Index"));
                        }
                        step.Trip.TotalCost += leisureActivity.Price;
                    }
                    else
                    {
                        LeisureActivity dbEntry = (LeisureActivity)db.Activities.FindAsync(leisureActivity.ID).Result;
                        if (dbEntry.Step.Trip.TripOwner != User.Identity.GetUserId())
                        {
                            return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this activity doesn't seem to be yours, you cannot edit it."),
                                "Trip", "Index"));
                        }
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
                catch (RetryLimitExceededException)
                {
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
                        if (step.Trip.TripOwner != User.Identity.GetUserId())
                        {
                            return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this step doesn't seem to be yours, you cannot add an activity to it."),
                                "Trip", "Index"));
                        }
                        step.Trip.TotalCost += transport.Price;
                    }
                    else
                    {
                        Transport dbEntry = (Transport)db.Activities.FindAsync(transport.ID).Result;
                        if (dbEntry.Step.Trip.TripOwner != User.Identity.GetUserId())
                        {
                            return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this activity doesn't seem to be yours, you cannot edit it."),
                                "Trip", "Index"));
                        }
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
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            return View(transport);
        }

        // POST: Activities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int actId)
        {
            Activity activityToDelete = await db.Activities.FindAsync(actId);
            if (activityToDelete.Step.Trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this activity doesn't seem to be yours, you cannot delete it."),
                                "Trip", "Index"));
            }
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
