using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Abstract;
using TrekStories.DAL;
using TrekStories.Models;

namespace TrekStories.Controllers
{
    public class AccommodationController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public AccommodationController() { }

        public AccommodationController(ITrekStoriesContext context)
        {
            db = context;
        }

        // GET: Accommodation
        public async Task<ActionResult> Index(int? tripId)
        {
            if (tripId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = await db.Trips.Include(t => t.Steps).SingleOrDefaultAsync(t => t.TripId == tripId);
            if (trip == null)
            {
                return HttpNotFound();
            }
            ViewBag.TripId = tripId;
            ViewBag.TripTitle = trip.Title;

            //SHOULD THIS BE WRITTEN ASYNC?
            //return accommodations just for trip...
            var tripAccommodations = from s in trip.Steps
                                     join a in db.Accommodations
                                     on s.AccommodationId equals a.AccommodationId
                                     select a;

            ViewBag.Currency = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

            return View(tripAccommodations);
        }

        // GET: Accommodation/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accommodation accommodation = await db.Accommodations.FindAsync(id);
            if (accommodation == null)
            {
                return HttpNotFound();
            }
            return View(accommodation);
        }

        // GET: Accommodation/Create
        public ActionResult Create(int? id) //tripId
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Currency = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            
            //pass in steps dates as default..

            return View();
        }

        // POST: Accommodation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Address,PhoneNumber,CheckIn,CheckOut,ConfirmationFileUrl,Price")] Accommodation accommodation)
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
                    Trip trip = new Trip(); //replace this!!!

                    try
                    {
                        AssignAccommodationToStep(accommodation, trip, true);
                    }
                    catch (ArgumentException ex)
                    {
                        //give feedback to user about which step to check
                        ViewBag.ErrorMessage = ex.Message;
                        return View(accommodation);
                    }

                    db.Accommodations.Add(accommodation);

                    //increase trip budget


                    await db.SaveChangesAsync();
                    //return update view in case user wants to attach confirmation file
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
            }
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
                return HttpNotFound();
            }
            return View(accommodation);
        }

        // POST: Accommodation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Address,PhoneNumber,CheckIn,CheckOut,ConfirmationFileUrl,Price")] Accommodation accommodation)
        {
            //check valid check-in and check-out
            if (!accommodation.IsCheckInBeforeCheckOut())
            {
                ModelState.AddModelError("", "Please check the check-in and check-out dates. Check-out cannot be before check-in.");
            }
            if (ModelState.IsValid)
            {
                db.MarkAsModified(accommodation);

                //update trip budget
                

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(accommodation);
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
                return HttpNotFound();
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
            {
                foreach (Step s in steps)
                {
                    s.AccommodationId = null;
                }
            }
            db.Accommodations.Remove(accommodation);

            //update trip budget
            steps.First().Trip.TotalCost -= accommodation.Price;

            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Trip", new { id = steps.First().Trip.TripId});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public static void AssignAccommodationToStep(Accommodation acc, Trip trip, bool insert)
        {
            //for any existing step within dates, check no accommodation exists
            var dates = acc.GetDatesBetweenInAndOut();
            foreach (var date in dates)
            {
                Step step = trip.Steps.FirstOrDefault(s => s.Date == date);
                if (step == null)
                {
                    throw new ArgumentException("There is no existing step for date" + date.ToShortDateString() + ".");   
                }
                else if (insert && step.AccommodationId != null) //or ==accID for updates?
                {
                    throw new ArgumentException("An accommodation already exists for Step " + step.SequenceNo);
                }
                else
                {
                    step.AccommodationId = acc.AccommodationId;
                }
            }
        }
    }
}
