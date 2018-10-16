using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Abstract;
using TrekStories.DAL;
using TrekStories.Models;

namespace TrekStories.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TripController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public TripController() { }

        public TripController(ITrekStoriesContext context)
        {
            db = context;
        }

        // GET: Trip
        public async Task<ActionResult> Index(string searchString)
        {
            string user_id = User.Identity.GetUserId();
            var trips = from t in db.Trips
                        where String.Equals(t.TripOwner, user_id)
                        select t;
            if (!String.IsNullOrEmpty(searchString))
            {
                trips = trips.Where(t => t.Title.Contains(searchString));
            }
            return View(await trips.OrderByDescending(t => t.StartDate).ToListAsync());
        }

        [AllowAnonymous]
        public async Task<ActionResult> Search(TripSearchModel searchModel)
        {
            List<Trip> trips = await GetTrips(searchModel).ToListAsync();
            return View(trips);
        }

        private IQueryable<Trip> GetTrips(TripSearchModel searchModel)
        {
            var result = db.Trips.AsQueryable();
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.TitleKeyword))
                {
                    result = result.Where(t => t.Title.Contains(searchModel.TitleKeyword));
                }   
                if (searchModel.TripCategory.HasValue)
                {
                    result = result.Where(t => t.TripCategory == searchModel.TripCategory);
                }   
                if (!string.IsNullOrEmpty(searchModel.Country))
                {
                    result = result.Where(t => t.Country == searchModel.Country);
                }   
                if (searchModel.MinDuration.HasValue)
                {
                    result = result.Where(t => t.Duration >= searchModel.MinDuration);
                }  
                if (searchModel.MaxDuration.HasValue)
                {
                    result = result.Where(t => t.Duration <= searchModel.MaxDuration);
                }   
            }
            return result;
        }

        // GET: Trip/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int id = 1)
        {
            Trip trip = await db.Trips.FindAsync(id);
            if (trip == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the trip you are looking for doesn't exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            return View(trip);
        }

        // GET: Trip/Create
        public ActionResult Create()
        {
            ViewBag.CountryList = Trip.GetCountries();
            return View();
        }

        // POST: Trip/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Title,Country,TripCategory,StartDate,Notes")] Trip trip)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    trip.TripOwner = User.Identity.GetUserId();

                    //check if user already has a trip with that name
                    if (await db.Trips.AnyAsync(t => t.TripOwner == trip.TripOwner && t.Title.ToLower() == trip.Title.ToLower()))
                    {
                        ModelState.AddModelError("", "You have already created a trip with that title. Please give this trip a different title.");
                    }
                    else
                    {
                        db.Trips.Add(trip);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = trip.TripId });
                    }
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
            }
            ViewBag.CountryList = Trip.GetCountries();
            return View(trip);
        }

        // GET: Trip/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = await db.Trips.FindAsync(id);
            if (trip == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the trip you are looking for doesn't exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            if (trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                    new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot edit it."),
                    "Trip", "Index"));
            }
            ViewBag.CountryList = Trip.GetCountries();
            return View(trip);
        }

        // POST: Trip/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tripToUpdate = db.Trips.Find(id);

            if (tripToUpdate.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                    new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot edit it."),
                    "Trip", "Index"));
            }

            DateTime oldDate = tripToUpdate.StartDate;

            if (TryUpdateModel(tripToUpdate, "",
               new string[] { "Title", "Country", "TripCategory", "StartDate", "Notes" }))
            {
                try
                {
                    //if Start Date was modified, reassign all trip accommodations to steps
                    DateTime newDate = tripToUpdate.StartDate;
                    if (oldDate != newDate)
                    {
                        //get all acc on the trip
                        var tripAccommodations = from s in tripToUpdate.Steps
                                                 join a in db.Accommodations
                                                 on s.AccommodationId equals a.AccommodationId
                                                 select a;

                        //for each accommodation, call 'assign to step' method
                        foreach (Accommodation acc in tripAccommodations)
                        {
                            try
                            {
                                AccommodationController.AssignAccommodationToStep(acc, tripToUpdate, false);

                                //remove accommodation from previously assigned steps now out of range
                                List<Step> oldSteps = await db.Steps.Where(s => s.AccommodationId == acc.AccommodationId).Include(s => s.Trip).ToListAsync();
                                foreach (var oldStep in oldSteps)
                                {
                                    if (oldStep.Date.Date < acc.CheckIn.Date || oldStep.Date.Date >= acc.CheckOut.Date)
                                    {
                                        oldStep.AccommodationId = null;
                                    }
                                }
                            }
                            catch (ArgumentException)
                            {
                                ViewBag.ErrorMessage = "The trip cannot be updated because an accommodation is outside the trip dates range.";
                                ViewBag.CountryList = Trip.GetCountries();
                                return View(tripToUpdate);
                            }
                        }
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = tripToUpdate.TripId });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            ViewBag.CountryList = Trip.GetCountries();
            return View(tripToUpdate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetSummaryReport(int id)
        {
            Trip trip = await db.Trips.Include(t => t.Steps).SingleOrDefaultAsync(t => t.TripId == id);
            if (trip == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the trip you are looking for doesn't exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            ViewBag.TripTitle = trip.Title;

            var tripSteps = trip.Steps.OrderBy(s => s.SequenceNo).ToArray();
            List<ActivityThreadViewModel>[] activities = new List<ActivityThreadViewModel>[tripSteps.Length];

            var stepcontroller = new StepController();
            for (int i = 0; i < tripSteps.Length; i++)
            {
                activities[i] = stepcontroller.CreateActivityThread(tripSteps[i]);
            }
            ViewBag.ActivityThread = activities;
            ViewBag.TripSteps = tripSteps;

            var tripAccommodations = from s in trip.Steps
                                     join a in db.Accommodations
                                     on s.AccommodationId equals a.AccommodationId
                                     select a;
            ViewBag.TripAccommodations = tripAccommodations.Distinct().OrderBy(a => a.Name);
            return new RotativaHQ.MVC5.ViewAsPdf("Summary", tripSteps) { FileName = "TripSummary.pdf" };
        }

        public async Task<ActionResult> GetSouvenirReport(int id)
        {
            Trip trip = await db.Trips.Include(t => t.Steps).SingleOrDefaultAsync(t => t.TripId == id);
            if (trip == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the trip you are looking for doesn't exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            if (trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                    new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot access the souvenir report."),
                    "Trip", "Index"));
            }
            ViewBag.TripTitle = trip.Title;

            var tripSteps = trip.Steps.OrderBy(s => s.SequenceNo).ToList();
            if (tripSteps.Count == 0)
            {
                return View("CustomisedError", new HandleErrorInfo(
                    new UnauthorizedAccessException("Please first add steps to the trip."),
                    "Trip", "Index"));
            }

            //return View("Souvenir", tripSteps);
            return new RotativaHQ.MVC5.ViewAsPdf("Souvenir", tripSteps) { FileName = "SouvenirReport.pdf" };
        }
    }
}
