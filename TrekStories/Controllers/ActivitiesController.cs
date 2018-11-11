using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
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
                return View("CustomisedError", new HandleErrorInfo(new UnauthorizedAccessException(NULL_ACTIVITY_ERROR), "Trip", "Index"));
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
                //if new activity, add to db
                if (leisureActivity.ID == 0)
                {
                    try
                    {
                        await UpdateTripBudgetIfOwner(leisureActivity);
                        db.Activities.Add(leisureActivity);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        return View("CustomisedError", new HandleErrorInfo(ex, "Trip", "Index"));
                    }
                }
                else
                {
                    try
                    {
                        await UpdateLeisureActivity(leisureActivity);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        return View("CustomisedError", new HandleErrorInfo(ex, "Trip", "Index"));
                    }
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Step", new { id = leisureActivity.StepId });
            }
            return View(leisureActivity);
        }

        private async Task UpdateTripBudgetIfOwner(Activity activity)
        {
            Step step = await db.Steps.Include(s => s.Trip).FirstOrDefaultAsync(s => s.StepId == activity.StepId);
            if (step.Trip.TripOwner != User.Identity.GetUserId())
            {
                throw new UnauthorizedAccessException("Oops, this step doesn't seem to be yours, you cannot add an activity to it.");
            }
            step.Trip.TotalCost += activity.Price;
        }

        private async Task UpdateLeisureActivity(LeisureActivity leisureActivity)
        {
            LeisureActivity dbEntry = (LeisureActivity)((await db.Activities.FindAsync(leisureActivity.ID)));

            if (dbEntry.Step.Trip.TripOwner != User.Identity.GetUserId())
            {
                throw new UnauthorizedAccessException("Oops, this activity doesn't seem to be yours, you cannot edit it.");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTransport([Bind(Include = "ID,Name,StartTime,Price,Notes,TransportType,Company,Destination,Duration,StepId")] Transport transport)
        {
            if (ModelState.IsValid)
            {
                //if new transport, add to db
                if (transport.ID == 0)
                {
                    try
                    {
                        await UpdateTripBudgetIfOwner(transport);
                        db.Activities.Add(transport);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        return View("CustomisedError", new HandleErrorInfo(ex, "Trip", "Index"));
                    }
                }
                else
                {
                    try
                    {
                        await UpdateTransportActivity(transport);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        return View("CustomisedError", new HandleErrorInfo(ex, "Trip", "Index"));
                    }
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Step", new { id = transport.StepId });
            }
            return View(transport);
        }

        private async Task UpdateTransportActivity(Transport transport)
        {
            Transport dbEntry = (Transport)await db.Activities.FindAsync(transport.ID);
            if (dbEntry.Step.Trip.TripOwner != User.Identity.GetUserId())
            {
                throw new UnauthorizedAccessException("Oops, this activity doesn't seem to be yours, you cannot edit it.");
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

        // POST: Activities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int actId)
        {
            var result = await (from a in db.Activities
                       join s in db.Steps on a.StepId equals s.StepId
                       join t in db.Trips on s.TripId equals t.TripId
                       where a.ID == actId
                       select new { a, t, s.StepId }).SingleOrDefaultAsync();

            if (result != null)
            {
                if (result.t.TripOwner != User.Identity.GetUserId())
                {
                    return View("CustomisedError", new HandleErrorInfo(
                                    new UnauthorizedAccessException("Oops, this activity doesn't seem to be yours, you cannot delete it."),
                                    "Trip", "Index"));
                }
                db.Activities.Remove(result.a);

                //update trip budget
                result.t.TotalCost -= result.a.Price;

                await db.SaveChangesAsync();
                TempData["message"] = string.Format("Activity '{0}' was deleted successfully.", result.a.Name);
            }
            return RedirectToAction("Details", "Step", new { id = result.StepId });
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
