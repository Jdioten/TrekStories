using Microsoft.AspNet.Identity;
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

namespace TrekStories.Controllers
{
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
            var trips = from t in db.Trips
                        select t;  //add where userid =vlogged in user
            if (!String.IsNullOrEmpty(searchString))
            {
                trips = trips.Where(t => t.Title.Contains(searchString));
            }
            return View(await trips.OrderByDescending(t => t.StartDate).ToListAsync());
        }

        // GET: Trip/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = await db.Trips.FindAsync(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // GET: Trip/Create
        public ActionResult Create()
        {
            //ViewBag.Title = "Create";
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
                    trip.TripOwner = "User1";   //User.Identity.GetUserId();  NB: should this be set in class??

                    //check if user already has a trip with that name
                    if (db.Trips.Any(t => t.TripOwner == trip.TripOwner && t.Title.ToLower() == trip.Title.ToLower())) //should be anyasync if can fix unit test
                    {
                        ModelState.AddModelError("", "You have already created a trip with that title. Please give this trip a different title.");
                    }
                    else
                    {
                    db.Trips.Add(trip);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = trip.TripId});
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
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
                return HttpNotFound();
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
                        //use materialised view?
                        //get all acc on the trip
                        var tripAccommodations = from s in tripToUpdate.Steps
                                                 join a in db.Accommodations
                                                 on s.AccommodationId equals a.AccommodationId
                                                 select a;

                        //for each accommodation call assign to step method
                        foreach (Accommodation acc in tripAccommodations)
                        {
                            try
                            {
                                AccommodationController.AssignAccommodationToStep(acc, tripToUpdate, false);

                                //remove accommodation from previously assigned steps now out of range
                                List<Step> oldSteps = await db.Steps.Where(s => s.AccommodationId == acc.AccommodationId).Include(s => s.Trip).ToListAsync();
                                foreach (var oldStep in oldSteps)
                                {
                                    if (oldStep.Date.Day < acc.CheckIn.Day || oldStep.Date.Day >= acc.CheckOut.Day)
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


                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Details", new { id = tripToUpdate.TripId });
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            ViewBag.CountryList = Trip.GetCountries();
            return View(tripToUpdate);
        }

        // GET: Trip/Delete/5
        //public ActionResult Delete(int? id, bool? saveChangesError = false)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    if (saveChangesError.GetValueOrDefault())
        //    {
        //        ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists, contact the system administrator.";
        //    }
        //    Trip trip = db.Trips.Find(id);
        //    if (trip == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(trip);
        //}

        //// POST: Trip/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id)
        //{

        //    try
        //    {
        //        Trip tripToDelete = new Trip() { TripId = id };
        //        db.Entry(tripToDelete).State = EntityState.Deleted;
        //        db.SaveChanges();
        //    }
        //    catch (DataException/* dex */)
        //    {
        //        //Log the error (uncomment dex variable name and add a line here to write a log.
        //        return RedirectToAction("Delete", new { id = id, saveChangesError = true });
        //    }
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

        /*public ActionResult DisplayFullTripDetails()
        {

        }

        public void GenerateSummaryReport()
        {

        }

        public ActionResult DisplaySummarisedTripDetails()
        {

        }

        public void GenerateSouvenirReport()
        {

        }*/
    }
}
