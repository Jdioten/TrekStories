﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrekStories.Abstract;
using TrekStories.DAL;
using TrekStories.Models;

namespace TrekStories.Controllers
{
    public class StepController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public StepController() { }

        public StepController(ITrekStoriesContext context)
        {
            db = context;
        }

        // GET: Step
        public async Task<ActionResult> Index()
        {
            var steps = db.Steps.Include(s => s.Accommodation).Include(s => s.Review).Include(s => s.Trip);
            return View(await steps.ToListAsync());
        }

        // GET: Step/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = db.Steps.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            return View(step);
        }

        // GET: Step/Create
        public async Task<ActionResult> Create(int? tripId, int? seqNo)
        {
            if (tripId == null || seqNo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = await db.Trips.FindAsync(tripId);
            if (trip == null)
            {
                return HttpNotFound();
            }

            //ViewBag.AccommodationId = new SelectList(db.Accommodations, "AccommodationId", "Name");
            //ViewBag.StepId = new SelectList(db.Reviews, "ReviewId", "PrivateNotes");
            ViewBag.TripId = tripId;
            ViewBag.SeqNo = seqNo;
            ViewBag.TripTitle = trip.Title;
            return View("Create");
        }

        // POST: Step/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SequenceNo,From,To,WalkingTime,WalkingDistance,Ascent,Description,Notes,TripId")] Step step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Steps.Add(step);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = step.StepId });
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
            }
            //ViewBag.AccommodationId = new SelectList(db.Accommodations, "AccommodationId", "Name", step.AccommodationId);
            //ViewBag.StepId = new SelectList(db.Reviews, "ReviewId", "PrivateNotes", step.StepId);
            //ViewBag.TripId = new SelectList(db.Trips, "TripId", "Title", step.TripId);
            return View(step);
        }

        // GET: Step/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = db.Steps.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccommodationId = new SelectList(db.Accommodations, "AccommodationId", "Name", step.AccommodationId);
            ViewBag.StepId = new SelectList(db.Reviews, "ReviewId", "PrivateNotes", step.StepId);
            ViewBag.TripId = new SelectList(db.Trips, "TripId", "Title", step.TripId);
            return View(step);
        }

        // POST: Step/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
            //([Bind(Include = "From,To,WalkingTime,WalkingDistance,Ascent,Description,Notes")] Step step)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var stepToUpdate = db.Steps.Find(id);
            if (TryUpdateModel(stepToUpdate, "",
               new string[] { "From", "To", "WalkingTime", "WalkingDistance", "Ascent", "Description", "Notes" }))
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
            return View(stepToUpdate);
            //---
            //if (ModelState.IsValid)
            //{
            //    db.Entry(step).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //ViewBag.AccommodationId = new SelectList(db.Accommodations, "AccommodationId", "Name", step.AccommodationId);
            //ViewBag.StepId = new SelectList(db.Reviews, "ReviewId", "PrivateNotes", step.StepId);
            //ViewBag.TripId = new SelectList(db.Trips, "TripId", "Title", step.TripId);
            //return View(step);
        }

        // GET: Step/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = db.Steps.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            return View(step);
        }

        // POST: Step/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Step step = db.Steps.Find(id);
            db.Steps.Remove(step);
            db.SaveChanges();
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
