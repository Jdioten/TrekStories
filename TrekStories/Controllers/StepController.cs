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
    public class StepController : Controller
    {
        private ITrekStoriesContext db = new TrekStoriesContext();

        public StepController() { }

        public StepController(ITrekStoriesContext context)
        {
            db = context;
        }

        // GET: Step
        //public async Task<ActionResult> Index()
        //{
        //    var steps = db.Steps.Include(s => s.Accommodation).Include(s => s.Review).Include(s => s.Trip);
        //    return View(await steps.ToListAsync());
        //}

        // GET: Step/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int id = 1)
        {
            Step step = await db.Steps.Include(s => s.Accommodation).Include(s => s.Review).FirstOrDefaultAsync(s => s.StepId == id);
            if (step == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the step you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            //create array for pagination in view
            ViewBag.Steps = await db.Steps.Where(s => s.TripId == step.TripId).OrderBy(s =>s.SequenceNo).Select(s => s.StepId).ToArrayAsync();

            //create activity thread
            ViewBag.ActivityThread = CreateActivityThread(step);
            ViewBag.HideReview = step.Date > DateTime.Today ? "hidden" : "";
            ViewBag.PhotoCount = GetReviewPicturesCount(step);

            return View(step);
        }

        [NonAction]
        public static int GetReviewPicturesCount(Step step)
        {
            if (step.Review == null)
            {
                return 0;
            }
            else
            {
                return step.Review.Images.Count;
            }
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
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the step you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            if (trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot add a step to it."),
                                "Trip", "Index"));
            }
            ViewBag.TripId = tripId;
            ViewBag.SeqNo = seqNo;
            ViewBag.TripTitle = trip.Title;
            return View("Create", new StepViewModel());
        }

        // POST: Step/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StepViewModel stepViewModel)
        {
            Trip trip = await db.Trips.FindAsync(stepViewModel.TripId);
            if (trip == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the page you are trying to access does not exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            if (trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this trip doesn't seem to be yours, you cannot add a step to it."),
                                "Trip", "Index"));
            }
            try
            {
                if (ModelState.IsValid)
                {
                    Step newStep = new Step()
                    {
                        SequenceNo = stepViewModel.SequenceNo,
                        From = stepViewModel.From,
                        To = stepViewModel.To,
                        WalkingTime = stepViewModel.WalkingTimeHours + stepViewModel.WalkingTimeMinutes/60.0,
                        WalkingDistance = stepViewModel.WalkingDistance,
                        Ascent = stepViewModel.Ascent,
                        Description = stepViewModel.Description,
                        Notes = stepViewModel.Notes,
                        TripId = stepViewModel.TripId
                    };

                    //retrieve all subsequent steps and update seq no
                    foreach (Step item in db.Steps.Where(s => s.TripId == newStep.TripId && s.SequenceNo >= newStep.SequenceNo))
                    {
                        item.SequenceNo++;
                    }

                    db.Steps.Add(newStep);
                    await db.SaveChangesAsync();

                    //retrieve all steps where seq no >= to new step.seq no in an array including new step and assign accommodation of previous step for that seq no 
                    Step[] subsequentSteps = await db.Steps.Where(s => s.TripId == newStep.TripId && s.SequenceNo >= newStep.SequenceNo).OrderBy(s => s.SequenceNo).ToArrayAsync();
                    for (int i = 0; i <subsequentSteps.Length-1; i++)
                    {
                        subsequentSteps[i].AccommodationId = subsequentSteps[i + 1].AccommodationId;
                    }
                    //set last one to null
                    subsequentSteps[subsequentSteps.Length - 1].AccommodationId = null;

                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = newStep.StepId });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
            }
            ViewBag.TripId = stepViewModel.TripId;
            ViewBag.SeqNo = stepViewModel.SequenceNo;
            ViewBag.TripTitle = trip.Title;
            return View(stepViewModel);
        }

        // GET: Step/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = await db.Steps.FindAsync(id);
            if (step == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the step you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            if (step.Trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this step doesn't seem to be yours, you cannot edit it."),
                                "Trip", "Index"));
            }
            StepViewModel stepToEdit = new StepViewModel()
            {
                StepId = step.StepId,
                SequenceNo = step.SequenceNo,
                From = step.From,
                To = step.To,
                WalkingTimeHours = (int)step.WalkingTime,
                WalkingTimeMinutes = (int)((step.WalkingTime%1) * 60),
                WalkingDistance = step.WalkingDistance,
                Ascent = step.Ascent,
                Description = step.Description,
                Notes = step.Notes,
                TripId = step.TripId
            };

            return View(stepToEdit);
        }

        // POST: Step/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StepViewModel vm)
        {
            if (vm == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Step stepToUpdate = await db.Steps.FindAsync(vm.StepId.Value); //chnage to below to avoid object reference null
            Step stepToUpdate = await db.Steps.Include(t => t.Trip).FirstOrDefaultAsync(x => x.StepId == vm.StepId.Value);
            if (stepToUpdate == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the step you are looking for doesn't seem to exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            if (stepToUpdate.Trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this step doesn't seem to be yours, you cannot edit it."),
                                "Trip", "Index"));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //assign vm values to step
                    stepToUpdate.Ascent = vm.Ascent;
                    stepToUpdate.Description = vm.Description;
                    stepToUpdate.From = vm.From;
                    stepToUpdate.Notes = vm.Notes;
                    stepToUpdate.To = vm.To;
                    stepToUpdate.WalkingDistance = vm.WalkingDistance;
                    stepToUpdate.WalkingTime = vm.WalkingTimeHours + vm.WalkingTimeMinutes / 60.0;
                    //stepToUpdate.Trip =

                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", "Trip", new { id = stepToUpdate.TripId});
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact the system administrator.");
                }
            }
            return View(vm);
        }

        // GET: Step/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Please try again, and if the problem persists, contact the system administrator.";
            }
            Step step = await db.Steps.FindAsync(id);
            if (step == null)
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, the step you are trying to delete doesn't exist. Please try navigating to the main page again."),
                                "Trip", "Index"));
            }
            if (step.Trip.TripOwner != User.Identity.GetUserId())
            {
                return View("CustomisedError", new HandleErrorInfo(
                                new UnauthorizedAccessException("Oops, this step doesn't seem to be yours, you cannot delete it."),
                                "Trip", "Index"));
            }
            if (step.Accommodation != null)
            {
                TempData["message"] = string.Format("Step " + step.SequenceNo + " cannot be deleted because it is linked to an accommodation. " +
                    "Please first edit or delete the accommodation for the step.");
                return RedirectToAction("Details", "Step", new { id = step.StepId });
            }
            Review rev = step.Review;
            if (rev != null)
            {
                if (rev.Images.Count > 0)
                {
                    TempData["message"] = string.Format("Step " + step.SequenceNo + " cannot be deleted because it is linked to a review with images. " +
                    "Please first delete the images.");
                    return RedirectToAction("Details", "Step", new { id = step.StepId });
                }
            }
            return View(step);
        }

        // POST: Step/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Step stepToDelete = await db.Steps.Include(s => s.Trip).Include(s => s.Activities).Include(s => s.Review).SingleOrDefaultAsync(s => s.StepId == id);
            try
            {
                if (stepToDelete == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (stepToDelete.Trip.TripOwner != User.Identity.GetUserId())
                {
                    return View("CustomisedError", new HandleErrorInfo(
                                    new UnauthorizedAccessException("Oops, this step doesn't seem to be yours, you cannot edit it."),
                                    "Trip", "Index"));
                }
                if (stepToDelete.Accommodation != null)
                {
                    TempData["message"] = string.Format("Step " + stepToDelete.SequenceNo + " cannot be deleted because it is linked to an accommodation. " +
                        "Please first edit or delete the accommodation for the step.");
                    return RedirectToAction("Details", "Step", new { id = stepToDelete.StepId });
                }
                //retrieve all subsequent steps and update seq no
                foreach (Step step in db.Steps.Where(s => s.TripId == stepToDelete.TripId))
                {
                    if (step.SequenceNo > stepToDelete.SequenceNo)
                    {
                        step.SequenceNo--;
                    }
                }

                foreach (var item in stepToDelete.Activities)
                {
                    stepToDelete.Trip.TotalCost -= item.Price;
                }

                Review rev = stepToDelete.Review;
                if (rev != null)
                {
                    if (rev.Images.Count > 0)
                    {
                        TempData["message"] = string.Format("Step " + stepToDelete.SequenceNo + " cannot be deleted because it is linked to a review with images. " +
                        "Please first delete the images.");
                        return RedirectToAction("Details", "Step", new { id = stepToDelete.StepId });
                    }
                    db.Reviews.Remove(rev);
                }      

                db.Steps.Remove(stepToDelete);
                await db.SaveChangesAsync();
            }
            catch (RetryLimitExceededException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true});
            }
            return RedirectToAction("Details", "Trip", new { id = stepToDelete.TripId });
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
        public List<ActivityThreadViewModel> CreateActivityThread(Step step)
        {
            List<ActivityThreadViewModel> activityThread = new List<ActivityThreadViewModel>();

            AddLeisureToActivityThread(ref activityThread, step.StepId);
            AddTransportToActivityThread(ref activityThread, step);
            AddAccommodationToActivityThread(ref activityThread, step);

            return activityThread.OrderBy(a => a.StartTime.TimeOfDay).ToList();
        }

        private void AddLeisureToActivityThread(ref List<ActivityThreadViewModel> activityThread, int stepId)
        {
            foreach (LeisureActivity activity in db.Activities.OfType<LeisureActivity>().Where(a => a.StepId == stepId).ToList())
            {
                activityThread.Add(new ActivityThreadViewModel
                {
                    ID = activity.ID,
                    StartTime = activity.StartTime,
                    Name = activity.Name,
                    Price = activity.Price,
                    Icon = GetLeisureIcon(activity.LeisureCategory.ToString()),
                    Controller = "Activities"
                });
            }
        }

        private void AddTransportToActivityThread(ref List<ActivityThreadViewModel> activityThread, Step step)
        {
            var transportActivities = db.Activities.OfType<Transport>();

            var stepActivities = transportActivities.Where(a => a.StepId == step.StepId).ToList();
            AddStepTransport(ref activityThread, stepActivities, false);
            
            var transportsArrivingOnDay = (from s in step.Trip.Steps
                                           join a in transportActivities
                                           on s.StepId equals a.StepId
                                           where a.GetArrivalTime().Date == step.Date.Date
                                           select a).Except(stepActivities).ToList();
            AddStepTransport(ref activityThread, transportsArrivingOnDay, true);
        }

        private void AddStepTransport(ref List<ActivityThreadViewModel> activityThread, List<Transport> stepTransport, bool arrival)
        {
            foreach (Transport activity in stepTransport)
            {
                ActivityThreadViewModel activityVm = new ActivityThreadViewModel
                {
                    ID = activity.ID,
                    Price = activity.Price,
                    Icon = GetTransportIcon(activity.TransportType.ToString()),
                    Controller = "Activities"
                };

                if (arrival)
                {
                    activityVm.StartTime = activity.GetArrivalTime();
                    activityVm.Name = "Arrival " + activity.Name;
                    activityVm.ArrivalTime = null;
                }
                else
                {
                    activityVm.StartTime = activity.StartTime;
                    activityVm.Name = activity.Name;
                    activityVm.ArrivalTime = activity.GetArrivalTime();
                }
                activityThread.Add(activityVm);
            }
        }

        private void AddAccommodationToActivityThread(ref List<ActivityThreadViewModel> activityThread, Step step)
        {
            //Add check-in if happening on step date
            if (step.Accommodation != null)
            {
                AddChekInToActivityThread(ref activityThread, step.Accommodation, step.Date);
            }

            //Add check-out if happening on step date
            AddChekOutToActivityThread(ref activityThread, step);
        }

        private void AddChekInToActivityThread(ref List<ActivityThreadViewModel> activityThread, Accommodation accommodation, DateTime date)
        {
            //needs to search in accommodations for matching check-in
            if (accommodation.CheckIn.Date == date.Date)
            {
                activityThread.Add(new ActivityThreadViewModel
                {
                    ID = accommodation.AccommodationId,
                    StartTime = accommodation.CheckIn,
                    Name = "Check-In at " + accommodation.Name,
                    Price = accommodation.Price,
                    Icon = "fas fa-bed",
                    Controller = "Accommodation"
                });
            }
        }

        private void AddChekOutToActivityThread(ref List<ActivityThreadViewModel> activityThread, Step step)
        {
            var tripAccommodation = (from s in step.Trip.Steps
                                     join a in db.Accommodations
                                     on s.AccommodationId equals a.AccommodationId
                                     where a.CheckOut.Date == step.Date.Date
                                     select a).Distinct().SingleOrDefault();
            if (tripAccommodation != null)
            {
                activityThread.Add(new ActivityThreadViewModel
                {
                    ID = tripAccommodation.AccommodationId,
                    StartTime = tripAccommodation.CheckOut,
                    Name = "Check-Out at " + tripAccommodation.Name,
                    Price = tripAccommodation.Price,
                    Icon = "fas fa-bed",
                    Controller = "Accommodation"
                });
            }
        }

        public string GetTransportIcon(string type)
        {
            switch (type)
            {
                case "boat": return "fas fa-ship";
                case "plane": return "fas fa-plane";
                case "train":
                case "tram":
                case "metro":
                    { return "fas fa-subway"; }
                case "bus": return "fas fa-bus";
                case "car": return "fas fa-car";
                case "hitchhiking": return "fas fa-thumbs-up";
                case "bike": return "fas fa-bicycle";
                case "foot": return "fas fa-walking";

                default: return "";
            }
        }

        public string GetLeisureIcon(string type)
        {
            switch (type)
            {
                case "aquatic": return "fas fa-swimmer";
                case "sports": return "fas fa-dribbble";
                case "musical": return "fas fa-music";
                case "cultural": return "fas fa-university";
                case "nature": return "fas fa-paw";
                case "gastronomy": return "fas fa-utensils";
                case "other": return "fas fa-puzzle-piece";

                default: return "";
            }
        }
    }
}
