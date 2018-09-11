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
    public class AccommodationControllerTests
    {
        [TestMethod]
        public async Task IndexContainsAllAccommodationsForTripInCorrectOrder()
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Accommodation acc1 = new Accommodation { AccommodationId = 1, Name = "Hotel Z" };
            Accommodation acc2 = new Accommodation { AccommodationId = 2, Name = "Hotel A" };
            tc.Accommodations.Add(acc1);
            tc.Accommodations.Add(acc2);
            Trip trip = new Trip
            {
                TripId = 123,
                TripOwner = "ABC123",
                Steps = new List<Step>()
                {
                    new Step {StepId = 11, AccommodationId = 1},
                    new Step {StepId = 12, AccommodationId = 2},
                    new Step {StepId = 13 }
                }
            };
            tc.Trips.Add(trip);
            // Arrange - create a controller
            AccommodationController controller = new AccommodationController(tc).WithAuthenticatedUser("ABC123");
            // Action
            var viewResult = await controller.Index(123, null) as ViewResult;
            Accommodation[] result = ((IEnumerable<Accommodation>)viewResult.ViewData.Model).ToArray();
            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(2, result[0].AccommodationId);
            Assert.AreEqual(1, result[1].AccommodationId);
        }

        [TestMethod()]
        public async Task DetailsReturnsCorrectAccommodation()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            tc.Accommodations.Add(new Accommodation() { AccommodationId = 99 });
            AccommodationController controller = new AccommodationController(tc);
            // Act
            var result = await controller.Details(99) as ViewResult;
            var accommodation99 = (Accommodation)result.ViewData.Model;
            // Assert
            Assert.AreEqual(99, accommodation99.AccommodationId);
        }

        [TestMethod()]
        public async Task CanCreateAccommodationAndUpdateTripBudget()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            Step step = new Step { StepId = 11, TripId = 1, SequenceNo = 1,
             Trip = new Trip
             {
                 TripId = 1,
                 Title = "Test Trip",
                 Country = "Ireland",
                 TripCategory = TripCategory.forest,
                 StartDate = new DateTime(2018, 11, 28),
                 TotalCost = 80,
                 TripOwner = "ABC123"
             }
            };
            tc.Trips.Add(new Trip
            {
                TripId = 1,
                TripOwner = "ABC123",
                StartDate = new DateTime(2018, 11, 28),
                TotalCost = 80,
                Steps = new List<Step>() { step }
            });
            tc.Steps.Add(step);
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 28, 14, 0, 0), CheckOut = new DateTime(2018, 11, 29, 10, 0, 0), Price = 80, ConfirmationFileUrl = "1" };

            AccommodationController controller = new AccommodationController(tc).WithAuthenticatedUser("ABC123");
            // Act
            var result = await controller.Create(newAccommodation) as RedirectToRouteResult;
            Trip trip = tc.Trips.Find(1);

            // Assert
            Assert.AreEqual("Edit", result.RouteValues["action"]);
            Assert.AreEqual(160, trip.TotalCost);
        }

        [TestMethod()]
        public async Task CannotCreateAccommodationIfModelError()
        {
            //Arrange
            AccommodationController controller = new AccommodationController();
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 29), CheckOut = new DateTime(2018, 11, 28, 10, 0, 0) };

            var result = await controller.Create(newAccommodation) as ViewResult;
            controller.ModelState.AddModelError("", "Error");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
        }

        [TestMethod()]
        public async Task CannotCreateAccommodationWithInvalidCheckInOut()
        {
            AccommodationController controller = new AccommodationController();
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 29), CheckOut = new DateTime(2018, 11, 28, 10, 0, 0)};

            var result = await controller.Create(newAccommodation) as ViewResult;

            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ViewData.ModelState.Count == 1,
                 "Please check the check-in and check-out dates. Check-out cannot be before check-in.");
        }

        [TestMethod()]
        public async Task CannotCreateAccommodationForStepsWhereAccommodationExists()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            //add step with accommodation
            Step step = new Step
            {
                StepId = 11,
                TripId = 1,
                SequenceNo = 1,
                Trip = new Trip
                {
                    TripId = 1,
                    Title = "Test Trip",
                    Country = "Ireland",
                    TripCategory = TripCategory.forest,
                    StartDate = new DateTime(2018, 11, 28),
                    TotalCost = 80,
                    TripOwner = "ABC123"
                },
                AccommodationId = 12,
                Accommodation = new Accommodation { AccommodationId = 12, CheckIn = new DateTime(2018, 11, 28, 16, 0, 0) }
            };
            tc.Trips.Add(new Trip
            {
                TripId = 1,
                TripOwner = "ABC123",
                StartDate = new DateTime(2018, 11, 28),
                TotalCost = 80,
                Steps = new List<Step>() { step }
            });
            tc.Steps.Add(step);

            AccommodationController controller = new AccommodationController(tc).WithAuthenticatedUser("ABC123");

            //try to add accommodation for that step with existing acommodation..
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 28, 14, 0, 0), CheckOut = new DateTime(2018, 11, 29, 10, 0, 0), Price = 80, ConfirmationFileUrl = "1" };

            var result = await controller.Create(newAccommodation);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("An accommodation already exists for Step 1", controller.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public async Task CanEditAccommodation()
        {
            // Arrange - create the mock repository with accommodations
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Accommodation acc1 = new Accommodation { AccommodationId = 1, Name = "Hotel A"};
            Accommodation acc2 = new Accommodation { AccommodationId = 2, Name = "Hotel B" };
            tc.Accommodations.Add(acc1);
            tc.Accommodations.Add(acc2);
            // Arrange - create the controller
            AccommodationController controller = new AccommodationController(tc);
            // Act
            var result1 = await controller.Edit(1) as ViewResult;
            var a1 = (Accommodation)result1.ViewData.Model;
            var result2 = await controller.Edit(2) as ViewResult;
            var a2 = (Accommodation)result2.ViewData.Model;
            // Assert
            Assert.AreEqual(1, a1.AccommodationId);
            Assert.AreEqual(2, a2.AccommodationId);
            Assert.AreEqual("Hotel A", a1.Name);
        }

        [TestMethod]
        public async Task CannotEditNonexistentAccommodation()
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Accommodation acc1 = new Accommodation { AccommodationId = 1, Name = "Hotel A" };
            tc.Accommodations.Add(acc1);
            // Arrange - create the controller
            AccommodationController controller = new AccommodationController(tc);
            // Act
            var badResult = await controller.Edit(4) as ViewResult;
            // Assert
            Assert.AreEqual("CustomisedError", badResult.ViewName);
        }

        [TestMethod]
        public async Task CanSaveValidAccommodationChangesAndUpdateTripBudget()
        {
            // Arrange - create mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            // Arrange - create an accommodation and a step linked to it
            Accommodation acc = new Accommodation { AccommodationId = 1, Name = "Hotel A", CheckIn = new DateTime(2018,8,10,10,0,0), CheckOut = new DateTime(2018,8,11,10,0,0), Price = 50 };
            tc.Accommodations.Add(acc);
            Trip trip = new Trip { TripId = 11, StartDate = new DateTime(2018, 8, 10), TotalCost = 100, TripOwner = "ABC123" };
            Step step = new Step { StepId = 123, Trip = trip, AccommodationId = 1, SequenceNo = 1 };
            tc.Trips.Add(trip);
            tc.Steps.Add(step);

            // Arrange - create the controller with update accommodation data
            var controller = new AccommodationController(tc).WithIncomingValues(new FormCollection {
                { "AccommodationId", "1" }, { "Name", "Name Changed" }, { "CheckIn", "10-08-2018 10:00 AM" }, { "CheckOut", "11-08-2018 10:00 AM" }, {"Price", "60"}
            }).WithAuthenticatedUser("ABC123");

            // Act - try to save the accommodation
            var result = await controller.EditPost(1) as RedirectToRouteResult;
            Trip updatedTrip = tc.Trips.Find(11);

            // Assert - check the method result type
            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual("Trip", result.RouteValues["controller"]);
            Assert.AreEqual(110, updatedTrip.TotalCost);
        }

        [TestMethod]
        public async Task CannotEditAccommodationIfBeforeTripStartDate()
        {
            // Arrange - create mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            // Arrange - create an accommodation and a step linked to it
            Accommodation acc = new Accommodation { AccommodationId = 1, Name = "Hotel A", CheckIn = new DateTime(2018, 8, 10, 10, 0, 0), CheckOut = new DateTime(2018, 8, 11, 10, 0, 0) };
            tc.Accommodations.Add(acc);
            Trip trip = new Trip { TripId = 11, StartDate = new DateTime(2018, 8, 10), TripOwner = "ABC123" };
            Step step = new Step { StepId = 123, Trip = trip, AccommodationId = 1, SequenceNo = 1 };
            tc.Steps.Add(step);

            // Arrange - create the controller with update accommodation data
            var controller = new AccommodationController(tc).WithIncomingValues(new FormCollection {
                { "AccommodationId", "1" }, { "Name", "Name Changed" }, { "CheckIn", "09-08-2018 10:00 AM" }, { "CheckOut", "11-08-2018 10:00 AM" }
            }).WithAuthenticatedUser("ABC123");

            // Act - try to save the accommodation
            var result = await controller.EditPost(1) as ViewResult;

            // Assert - check the method result type
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public async Task CannotEditAccommodationIfOneAlreadyExistsForThoseDates()
        {
            // Arrange - create mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            // Arrange - create two accommodations linked to 2 different steps
            Accommodation acc1 = new Accommodation { AccommodationId = 1, Name = "Hotel A", CheckIn = new DateTime(2018, 8, 10, 12, 0, 0), CheckOut = new DateTime(2018, 8, 11, 10, 0, 0), Price = 0 };
            Accommodation acc2 = new Accommodation { AccommodationId = 2, Name = "Hotel B", CheckIn = new DateTime(2018, 8, 11, 12, 0, 0), CheckOut = new DateTime(2018, 8, 12, 10, 0, 0), Price = 0 };
            tc.Accommodations.Add(acc1);
            tc.Accommodations.Add(acc2);
            Trip trip = new Trip { TripId = 11, StartDate = new DateTime(2018, 8, 10), TripOwner = "ABC123", TotalCost = 0,
            Steps = new List<Step>()
            {
                new Step { StepId = 123, TripId = 11, AccommodationId = 1, SequenceNo = 1, Trip = new Trip {StartDate = new DateTime(2018, 8, 10) } },
                new Step { StepId = 124, TripId = 11, AccommodationId = 2, SequenceNo = 2, Trip = new Trip {StartDate = new DateTime(2018, 8, 10) } }
            }
            };
            tc.Trips.Add(trip);
            Step step1 = new Step { StepId = 123, TripId = 11, Trip = trip, AccommodationId = 1, Accommodation = acc1, SequenceNo = 1 };
            Step step2 = new Step { StepId = 124, TripId = 11, Trip = trip, AccommodationId = 2, Accommodation = acc2, SequenceNo = 2 };
            tc.Steps.Add(step1);
            tc.Steps.Add(step2);


            // Arrange - create the controller with update accommodation data
            var controller = new AccommodationController(tc).WithIncomingValues(new FormCollection {
                { "AccommodationId", "1" }, { "Name", "Name Changed" }, { "CheckIn", "11-08-2018 10:00 AM" }, { "CheckOut", "12-08-2018 10:00 AM" }, {"Price", "0"}
            }).WithAuthenticatedUser("ABC123");

            // Act - try to save the activity
            var result = await controller.EditPost(1) as ViewResult;

            // Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("An accommodation already exists for Step 2", controller.ViewBag.ErrorMessage);
        }

        [TestMethod()]
        public async Task DeleteReturnsCorrectAccommodation()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Accommodation acc = new Accommodation { AccommodationId = 10, Name = "Hotel Test" };
            tc.Accommodations.Add(acc);
            var controller = new AccommodationController(tc);
            // Act - delete the accommodation
            var result = await controller.Delete(10) as ViewResult;
            var resultAcc = (Accommodation)result.ViewData.Model;
            // Assert
            Assert.AreEqual(10, resultAcc.AccommodationId);
            Assert.AreEqual("Hotel Test", resultAcc.Name);
        }

        [TestMethod]
        public async Task CanDeleteAccommodationAndUpdateBudget()
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip trip = new Trip { TripId = 1, Title = "Trip Name", TotalCost = 120, TripOwner = "ABC123" };
            tc.Trips.Add(trip);
            tc.Steps.Add(new Step { StepId = 123, Trip = trip, AccommodationId = 2 });
            Accommodation a2 = new Accommodation() { AccommodationId = 2, Name = "Test", Price = 70 };
            tc.Accommodations.Add(a2);
            // Arrange - create the controller
            AccommodationController controller = new AccommodationController(tc).WithAuthenticatedUser("ABC123");
            // Act - delete an accommodation
            var result = await controller.DeleteConfirmed(2);
            // Assert - ensure that the accommodation is deleted from repository
            Assert.IsNull(tc.Accommodations.FirstOrDefault(a => a.AccommodationId == a2.AccommodationId));
            // Assert - ensure that the trip budget was updated
            Assert.AreEqual(trip.TotalCost, 50);
        }
    }
}