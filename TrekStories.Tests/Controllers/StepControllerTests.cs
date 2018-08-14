using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Models;
using TrekStories.Tests;
using TrekStories.Tests.UnitTestHelpers;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class StepControllerTests
    {
        [TestMethod()]
        public async Task DetailsReturnsCorrectStep()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            StepController controller = new StepController(tc);
            Step step = new Step() {
                StepId = 123,
                SequenceNo = 2,
                From = "B",
                To = "C",
                WalkingDistance = 0,
                WalkingTime = 3.5
            };
            step.Trip = new Trip { TripId = 1, Steps = new List<Step>() { step } };
            tc.Steps.Add(step);

            ViewResult result = await controller.Details(123) as ViewResult;

            var step123 = (Step)result.ViewData.Model;
            Assert.AreEqual(2, step123.SequenceNo);
            Assert.AreEqual("B", step123.From);
            Assert.AreEqual(1, controller.ViewBag.Steps.Length);
            Assert.AreEqual(123, controller.ViewBag.Steps[0]);
        }

        [TestMethod()]
        public async Task CanCreateStepForTrip()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            var trip = new Trip
            {
                TripId = 1,
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123",
                TotalWalkingDistance = 45
            };
            tc.Trips.Add(trip);
            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");
            var result = await controller.Create(1, 1) as ViewResult;
            Assert.AreEqual("Create", result.ViewName);
        }

        [TestMethod()]
        public async Task CanCreateStep()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip newTrip = new Trip
            {
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            };
            tc.Trips.Add(newTrip);
            StepViewModel stepViewModel = new StepViewModel
            {
                SequenceNo = 1,
                From = "A",
                To = "B",
                WalkingTimeHours = 2,
                WalkingTimeMinutes = 30,
                WalkingDistance = 12,
                Ascent = 630,
                Description = "A lovely walk",
                Notes = null,
                TripId = newTrip.TripId
            };
            
            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");
            var result = await controller.Create(stepViewModel) as RedirectToRouteResult;
            Step created = tc.Steps.First();

            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual("A", created.From);
            Assert.AreEqual(2.5, created.WalkingTime);
            Assert.AreEqual(12, created.WalkingDistance);
        }

        [TestMethod()]
        public async Task CreateStepEditsSeqNoOfSubsequentSteps()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip newTrip = new Trip
            {
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            };
            tc.Trips.Add(newTrip);
            Step stepA = new Step { StepId = 11, SequenceNo = 1};
            Step stepB = new Step { StepId = 12, SequenceNo = 2 };
            Step stepC = new Step { StepId = 10, SequenceNo = 3 };
            tc.Steps.Add(stepA);
            tc.Steps.Add(stepB);
            tc.Steps.Add(stepC);

            StepViewModel stepViewModel = new StepViewModel
            {
                SequenceNo = 2,
                From = "B",
                To = "C",
                WalkingTimeHours = 2,
                WalkingTimeMinutes = 30,
                WalkingDistance = 12,
                Ascent = 630,
                Description = "A lovely walk",
                Notes = null,
                TripId = newTrip.TripId
            };

            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");
            var result = await controller.Create(stepViewModel);

            Assert.AreEqual(1, stepA.SequenceNo);
            Assert.AreEqual(3, stepB.SequenceNo);
            Assert.AreEqual(4, stepC.SequenceNo);
        }

        [TestMethod()]
        public async Task InsertStepReassignsAccommodations()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip newTrip = new Trip
            {
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            };
            tc.Trips.Add(newTrip);
            Accommodation acc = new Accommodation { AccommodationId = 122};
            tc.Accommodations.Add(acc);
            Step stepA = new Step { StepId = 11, SequenceNo = 1 };
            Step stepB = new Step { StepId = 12, SequenceNo = 2, AccommodationId = 122 };
            Step stepC = new Step { StepId = 10, SequenceNo = 3 };
            tc.Steps.Add(stepA);
            tc.Steps.Add(stepB);
            tc.Steps.Add(stepC);

            StepViewModel stepViewModel = new StepViewModel
            {
                SequenceNo = 2,
                From = "B",
                To = "C",
                WalkingTimeHours = 2,
                WalkingTimeMinutes = 30,
                WalkingDistance = 12,
                Ascent = 630,
                Description = "A lovely walk",
                Notes = null,
                TripId = newTrip.TripId
            };

            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");
            var result = await controller.Create(stepViewModel);
            var insertedStep = tc.Steps.FirstOrDefault(s => s.SequenceNo == 2);

            Assert.AreEqual(null, stepB.AccommodationId);
            Assert.AreEqual(122, insertedStep.AccommodationId);
        }

        [TestMethod()]
        public async Task CannotCreateStepForNonexistentTrip()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            StepViewModel stepViewModel = new StepViewModel
            {
                SequenceNo = 1,
                From = "A",
                To = "B",
                WalkingTimeHours = 2,
                WalkingTimeMinutes = 30,
                WalkingDistance = 12,
                Ascent = 630,
                Description = "A lovely walk",
                Notes = null,
                TripId = 2
            };

            var controller = new StepController(tc);

            var badResult = await controller.Create(stepViewModel);

            Assert.IsInstanceOfType(badResult, typeof(HttpNotFoundResult));
        }


        [TestMethod()]
        public async Task CannotCreateStepWithModelErrors()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip newTrip = new Trip
            {
                TripId = 1,
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            };
            tc.Trips.Add(newTrip);
            StepViewModel stepViewModel = new StepViewModel
            {
                SequenceNo = 1,
                From = "",
                To = "",
                WalkingTimeHours = 2,
                WalkingTimeMinutes = 30,
                WalkingDistance = 12,
                Ascent = 630,
                Description = "A lovely walk",
                Notes = null,
                TripId = 1
            };

            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");
            controller.ModelState.AddModelError("", "Error");
            var result = await controller.Create(stepViewModel) as ViewResult;

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
        }

        [TestMethod()]
        public async Task EditStepReturnsCorrectStepVm()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step = new Step
            {
                StepId = 123,
                SequenceNo = 2,
                From = "B",
                To = "C",
                WalkingDistance = 0,
                WalkingTime = 3.5,
                Trip = new Trip { TripOwner = "ABC123"}
            };
            tc.Steps.Add(step);

            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");

            var result = await controller.Edit(123) as ViewResult;
            var resultStep = (StepViewModel)result.ViewData.Model;

            Assert.AreEqual(123, resultStep.StepId);
            Assert.AreEqual(3, resultStep.WalkingTimeHours);
            Assert.AreEqual(30, resultStep.WalkingTimeMinutes);
        }

        [TestMethod()]
        public async Task CanEditPostStep()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip trip = new Trip { TripId = 111, TripOwner = "ABC123" };
            tc.Trips.Add(trip);
            Step step = new Step
            {
                StepId = 123,
                TripId = 111,
                Trip = trip
            };
            tc.Steps.Add(step);
            StepViewModel stepVm = new StepViewModel
            {
                StepId = 123,
                SequenceNo = 2,
                From = "B",
                To = "C",
                WalkingDistance = 0,
                WalkingTimeHours = 2,
                WalkingTimeMinutes = 30,
                TripId = 111
            };

            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");

            var result = await controller.Edit(stepVm) as RedirectToRouteResult;

            Assert.AreEqual("Details", result.RouteValues["action"]);
        }

        [TestMethod()]
        public async Task CannotEditStepWithModelErrors()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step = new Step
            {
                StepId = 123,
                Trip = new Trip { TripId = 111, TripOwner = "ABC123" }
        };
            tc.Steps.Add(step);
            StepViewModel stepVm = new StepViewModel { StepId = 123, TripId = 111 };

            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");
            controller.ModelState.AddModelError("", "Error");

            var result = await controller.Edit(stepVm) as ViewResult;

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
        }

        [TestMethod()]
        public async Task DeleteReturnsCorrectStep()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step = new Step { StepId = 10, SequenceNo = 3, Trip = new Trip { TripOwner = "ABC123"} };
            tc.Steps.Add(step);
            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");

            var result = await controller.Delete(10) as ViewResult;
            var resultStep = (Step)result.ViewData.Model;

            Assert.AreEqual(10, resultStep.StepId);
            Assert.AreEqual(3, resultStep.SequenceNo);
        }

        [TestMethod]
        public async Task CanDeleteValidSteps()
        {
            // Arrange - create a trip wit steps
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip trip = new Trip
            {
                TripId = 1,
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123",
            };
            tc.Trips.Add(trip);
            Step stepA = new Step { StepId = 11, SequenceNo = 1, TripId = 1, Trip = trip };
            Step stepB = new Step { StepId = 12, SequenceNo = 2, TripId = 1, Trip = trip };
            Step stepC = new Step { StepId = 10, SequenceNo = 3, TripId = 1, Trip = trip };
            tc.Steps.Add(stepA);
            tc.Steps.Add(stepB);
            tc.Steps.Add(stepC);

            // Arrange - create the controller
            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");
            // Act - delete the step
            var result = await controller.DeleteConfirmed(12);
            // Assert - ensure that step was deleted and sequence no updated
            Assert.AreEqual(1, stepA.SequenceNo);
            Assert.AreEqual(2, stepC.SequenceNo);
            Assert.IsNull(tc.Steps.FirstOrDefault(s => s.StepId == stepB.StepId));
        }

        [TestMethod()]
        public async Task DeleteNonExistingStepReturnsNotFound()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            var controller = new StepController(tc);

            var badResult = await controller.Delete(12);

            Assert.IsInstanceOfType(badResult, typeof(HttpNotFoundResult));
        }

        [TestMethod()]
        public async Task CannotDeleteStepWithAccommodation()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step stepA = new Step {
                StepId = 11,
                SequenceNo = 1,
                TripId = 2,
                Accommodation = new Accommodation { AccommodationId = 1, Name = "Hotel Zuki"},
                Trip = new Trip { TripOwner = "ABC123"}
            };
            tc.Steps.Add(stepA);

            var controller = new StepController(tc).WithAuthenticatedUser("ABC123");
            var result = await controller.Delete(11) as RedirectToRouteResult;

            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual("Step", result.RouteValues["controller"]);
            Assert.AreEqual(
                "Step 1 cannot be deleted because it is linked to an accommodation. Please first edit or delete the accommodation for the step.",
                controller.TempData["message"]);
        }
    }
}