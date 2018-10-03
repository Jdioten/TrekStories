using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Abstract;
using TrekStories.DAL;
using TrekStories.Models;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using TrekStories.Utilities;
using System.Web;
using System.IO;

namespace TrekStories.Controllers
{
    [RequireHttps]
    [Authorize]
    public class AccommodationController : Controller
    {
        private const string BOOKING_CONTAINER_NAME = "trekstories-bookingconfirm-blobcontainer";

        private ITrekStoriesContext db = new TrekStoriesContext();
        private BlobUtility utility;

        public AccommodationController()
        {
            utility = new BlobUtility();
        }

        public AccommodationController(ITrekStoriesContext context)
        {
            db = context;
            utility = new BlobUtility();
        }

        // GET: Accommodation
        [AllowAnonymous]
        public async Task<ActionResult> Index(int? tripId, string sortOrder)
        {
            if (tripId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CheckInSortParm = sortOrder == "CheckIn" ? "checkin_desc" : "CheckIn";
            Trip trip = await db.Trips.Include(t => t.Steps).SingleOrDefaultAsync(t => t.TripId == tripId);
            if (trip == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the accommodation you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            if (trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot see its accommodations."),
                                "Trip", "Index"));
            }
            ViewBag.TripId = tripId;
            ViewBag.TripTitle = trip.Title;

            //return accommodations just for trip...
            var tripAccommodations = from s in trip.Steps
                                     join a in db.Accommodations
                                     on s.AccommodationId equals a.AccommodationId
                                     select a;

            switch (sortOrder)
            {
                case "name_desc":
                    tripAccommodations = tripAccommodations.OrderByDescending(a => a.Name);
                    break;
                case "CheckIn":
                    tripAccommodations = tripAccommodations.OrderBy(a => a.CheckIn);
                    break;
                case "checkin_desc":
                    tripAccommodations = tripAccommodations.OrderByDescending(a => a.CheckIn);
                    break;
                default:
                    tripAccommodations = tripAccommodations.OrderBy(a => a.Name);
                    break;
            }

            ViewBag.Currency = CurrencyHelper.GetCurrency();
            return View(tripAccommodations.ToList());
        }

        // GET: Accommodation/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int id = 1)
        {
            Accommodation accommodation = await db.Accommodations.FindAsync(id);
            if (accommodation == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the accommodation you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            return View(accommodation);
        }

        // GET: Accommodation/Create
        public ActionResult Create(int? id, DateTime? cIn, DateTime? cOut) //tripId
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Currency = CurrencyHelper.GetCurrency();
            ViewBag.TripId = id;

            //pass in steps dates as default
            if (cIn != null && cOut != null)
            {
                ViewBag.CheckIn = String.Format("{0:dd-MM-yyyy hh:mm tt}", cIn);
                ViewBag.CheckOut = String.Format("{0:dd-MM-yyyy hh:mm tt}", cOut);
            }
            return View();
        }

        // POST: Accommodation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Street,City,PhoneNumber,CheckIn,CheckOut,ConfirmationFileUrl,Price")] Accommodation accommodation)
        {
            try
            {
                //check valid check-in and check-out
                if (!accommodation.IsCheckInBeforeCheckOut())
                {
                    ModelState.AddModelError("", "Please check the check-in and check-out dates. Check-out cannot be before check-in.");
                }
                else if (ModelState.IsValid)
                {
                    //if before trip start date -> error
                    int tripId = Int32.Parse(accommodation.ConfirmationFileUrl); //temporarily storing tripid in confirmationurl

                    //extract to different method that could apply to different controllers!
                    Trip trip = await db.Trips.FindAsync(tripId);
                    if (trip.TripOwner != User.Identity.GetUserId())
                    {
                        return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot add an accommodation to it."),
                                "Trip", "Index"));
                    }


                    if (accommodation.CheckIn < trip.StartDate)
                    {
                        ModelState.AddModelError("", "The check-in date is before the trip start date (" + trip.StartDate.ToShortDateString() + "). Please correct.");
                    }
                    else
                    {
                        try
                        {
                            AssignAccommodationToStep(accommodation, trip, true);
                        }
                        catch (ArgumentException ex)
                        {
                            //give feedback to user about which step to check
                            ViewBag.ErrorMessage = ex.Message;
                            ViewBag.TripId = accommodation.ConfirmationFileUrl;
                            ViewBag.CheckIn = String.Format("{0:dd-MM-yyyy hh:mm tt}", accommodation.CheckIn);
                            ViewBag.CheckOut = String.Format("{0:dd-MM-yyyy hh:mm tt}", accommodation.CheckOut);
                            return View(accommodation);
                        }

                        //put below within try to be able to delete repetitive viewbag lines
                        accommodation.ConfirmationFileUrl = null;
                        db.Accommodations.Add(accommodation);

                        //increase trip budget
                        trip.TotalCost += accommodation.Price;

                        await db.SaveChangesAsync();
                        //return update view in case user wants to attach confirmation file
                        return RedirectToAction("Edit", new { id = accommodation.AccommodationId });
                    }
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
            }
            ViewBag.TripId = accommodation.ConfirmationFileUrl;
            ViewBag.CheckIn = String.Format("{0:dd-MM-yyyy hh:mm tt}", accommodation.CheckIn);
            ViewBag.CheckOut = String.Format("{0:dd-MM-yyyy hh:mm tt}", accommodation.CheckOut);
            return View(accommodation);
        }

        // GET: Accommodation/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accommodation accommodation = await db.Accommodations.FindAsync(id);
            if (accommodation == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the accommodation you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            return View(accommodation);
        }

        // POST: Accommodation/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id, HttpPostedFileBase file)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var accommodationToUpdate = await db.Accommodations.FindAsync(id);
            DateTime oldCheckIn = accommodationToUpdate.CheckIn;
            DateTime oldCheckOut = accommodationToUpdate.CheckOut;
            double oldPrice = accommodationToUpdate.Price;

            if (TryUpdateModel(accommodationToUpdate, "",
               new string[] { "Name", "Street", "City", "PhoneNumber", "CheckIn", "CheckOut", "ConfirmationFileUrl", "Price" }))
            {
                try
                {
                    //check valid check-in and check-out
                    if (!accommodationToUpdate.IsCheckInBeforeCheckOut())
                    {
                        ModelState.AddModelError("", "Please check the check-in and check-out dates. Check-out cannot be before check-in.");
                        return View(accommodationToUpdate);
                    }

                    //if before trip start date -> error
                    Step step = await db.Steps.Include(s => s.Trip).FirstAsync(s => s.AccommodationId == accommodationToUpdate.AccommodationId);
                    Trip trip = step.Trip;
                    if (trip.TripOwner != User.Identity.GetUserId())
                    {
                        return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot edit its accommodations."),
                                "Trip", "Index"));
                    }
                    if (accommodationToUpdate.CheckIn < trip.StartDate)
                    {
                        ModelState.AddModelError("", "The check-in date is before the trip start date (" + trip.StartDate.ToShortDateString() + "). Please correct.");
                        return View(accommodationToUpdate);
                    }

                    //check if dates have changed
                    if (oldCheckIn != accommodationToUpdate.CheckIn || oldCheckOut != accommodationToUpdate.CheckOut)
                    {
                        try
                        {
                            AssignAccommodationToStep(accommodationToUpdate, trip, true);

                            //remove accommodation from previously assigned steps now out of range
                            List<Step> oldSteps = await db.Steps.Where(s => s.AccommodationId == accommodationToUpdate.AccommodationId).Include(s => s.Trip).ToListAsync();
                            foreach (var oldStep in oldSteps)
                            {
                                if (oldStep.Date.Date < accommodationToUpdate.CheckIn.Date || oldStep.Date.Date >= accommodationToUpdate.CheckOut.Date)
                                {
                                    oldStep.AccommodationId = null;
                                }
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            //give feedback to user about which step to check
                            ViewBag.ErrorMessage = ex.Message;
                            return View(accommodationToUpdate);
                        }
                    }

                    if (file != null)
                    {
                        file = file ?? Request.Files["ConfirmationFileUrl"];

                        try
                        {
                            accommodationToUpdate.ConfirmationFileUrl = await utility.UploadBlobAsync(file, BOOKING_CONTAINER_NAME);
                        }
                        catch (Exception e)
                        {
                            TempData["message"] = e.Message;
                            return View(accommodationToUpdate);
                        }
                    }

                    //update trip budget
                    trip.TotalCost = trip.TotalCost - oldPrice + accommodationToUpdate.Price;

                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Trip", new { id = trip.TripId });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            return View(accommodationToUpdate);
        }

        // GET: Accommodation/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Please try again. If the problem persists, contact the system administrator.";
            }
            Accommodation accommodation = await db.Accommodations.FindAsync(id);
            if (accommodation == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the accommodation you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            return View(accommodation);
        }

        // POST: Accommodation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Accommodation accommodation = await db.Accommodations.FindAsync(id);

            //set accommodation id to null
            List<Step> steps = await db.Steps.Include(s => s.Trip).Where(s => s.AccommodationId == accommodation.AccommodationId).ToListAsync();
            if (steps.First().Trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot delete its accommodations."),
                                "Trip", "Index"));
            }

            foreach (Step s in steps)
            {
                s.AccommodationId = null;
            }

            db.Accommodations.Remove(accommodation);

            //update trip budget
            steps.First().Trip.TotalCost -= accommodation.Price;

            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Trip", new { id = steps.First().Trip.TripId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [NonAction]
        public static void AssignAccommodationToStep(Accommodation acc, Trip trip, bool insert)
        {
            //for any existing step within dates, check no accommodation exists
            List<DateTime> dates = acc.GetDatesBetweenInAndOut();
            foreach (var date in dates)
            {
                Step step = trip.Steps.FirstOrDefault(s => s.Date.Date == date.Date);
                if (step == null)
                {
                    throw new ArgumentException("There is no existing step for date " + date.ToShortDateString() + ". Please first create the step.");
                }
                //if new accommodation, check that there is no accommodation already on step
                else if (insert && step.AccommodationId != null && step.AccommodationId != acc.AccommodationId) //to allow for updates
                {
                    throw new ArgumentException("An accommodation already exists for Step " + step.SequenceNo);
                }
                step.AccommodationId = acc.AccommodationId;
            }
        }
    }
}
