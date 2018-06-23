using Microsoft.AspNet.Identity;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
        public ActionResult Index()
        {
            return View(db.Trips.ToList());
        }

        // GET: Trip/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // GET: Trip/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Create";
            ViewBag.CountryList = Trip.GetCountries();
            return View();
        }

        // POST: Trip/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Country,TripCategory,StartDate,Notes")] Trip trip)
        {
            try
            {              
                if (ModelState.IsValid)
                {
                    trip.TripOwner = "User1";   //User.Identity.GetUserId();

                    db.Trips.Add(trip);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
            }
            return View(trip);
        }

        // GET: Trip/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryList = Trip.GetCountries();
            return View(trip);
        }

        // POST: Trip/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tripToUpdate = db.Trips.Find(id);
            if (TryUpdateModel(tripToUpdate, "",
               new string[] { "Title", "Country", "TripCategory", "StartDate", "Notes" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
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
