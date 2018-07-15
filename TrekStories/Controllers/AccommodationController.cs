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
    public class AccommodationController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public AccommodationController() { }

        public AccommodationController(ITrekStoriesContext context)
        {
            db = context;
        }

        // GET: Accommodation
        public async Task<ActionResult> Index()
        {
            return View(await db.Accommodations.ToListAsync());
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
        public ActionResult Create()
        {
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
                    db.Accommodations.Add(accommodation);
                    await db.SaveChangesAsync();
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
            db.Accommodations.Remove(accommodation);
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
