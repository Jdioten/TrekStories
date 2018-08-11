using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Models;
using TrekStories.Tests;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class AccommodationControllerTests
    {
        [TestMethod]
        public async Task IndexContainsAllAccommodationsForTrip()
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Accommodation acc1 = new Accommodation { AccommodationId = 1 };
            Accommodation acc2 = new Accommodation { AccommodationId = 2 };
            tc.Accommodations.Add(acc1);
            tc.Accommodations.Add(acc2);
            Trip trip = new Trip
            {
                TripId = 123,
                Steps = new List<Step>()
                {
                    new Step {StepId = 11, AccommodationId = 1},
                    new Step {StepId = 12, AccommodationId = 2},
                    new Step {StepId = 13 }
                }
            };
            tc.Trips.Add(trip);
            // Arrange - create a controller
            AccommodationController controller = new AccommodationController(tc);
            // Action
            var viewResult = await controller.Index(123) as ViewResult;
            Accommodation[] result = ((IEnumerable<Accommodation>)viewResult.ViewData.Model).ToArray();
            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(1, result[0].AccommodationId);
            Assert.AreEqual(2, result[1].AccommodationId);
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
                StartDate = new DateTime(2018, 11, 28),
                TotalCost = 80,
                Steps = new List<Step>() { step }
            });
            tc.Steps.Add(step);
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 28, 14, 0, 0), CheckOut = new DateTime(2018, 11, 29, 10, 0, 0), Price = 80, ConfirmationFileUrl = "1" };

            AccommodationController controller = new AccommodationController(tc);
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

        //to be completed!!!
        //[TestMethod()]
        //public async Task CannotCreateAccommodationForStepsWhereAccommodationExists()
        //{
        //    TestTrekStoriesContext tc = new TestTrekStoriesContext();

        //    //add step with accommodation

        //    AccommodationController controller = new AccommodationController();

        //    //try to add accommodation for that step with existing acommodation..
        //    Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 29), CheckOut = new DateTime(2018, 11, 28, 10, 0, 0) };

        //    var result = await controller.Create(newAccommodation) as ViewResult;

        //    Assert.IsTrue(!controller.ModelState.IsValid);
        //    //change text model error
        //    Assert.IsTrue(controller.ViewData.ModelState.Count == 1,
        //         "Please check the check-in and check-out dates. Check-out cannot be before check-in.");
        //}

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
            var badResult = await controller.Edit(4);
            // Assert
            Assert.IsInstanceOfType(badResult, typeof(HttpNotFoundResult));
        }

        //[TestMethod]  //find out why failing!!
        //public async Task CanSaveValidAccommodationChanges()
        //{
        //    // Arrange - create mock repository
        //    TestTrekStoriesContext tc = new TestTrekStoriesContext();

        //    // Arrange - create an accommodation and a step linked to it
        //    Accommodation acc = new Accommodation { AccommodationId = 1, Name = "Hotel A" };
        //    tc.Accommodations.Add(acc);
        //    Trip trip = new Trip { TripId = 11, StartDate = new DateTime(2018,8,10)};
        //    Step step = new Step { StepId = 123, Trip = trip, AccommodationId = 1, SequenceNo = 1 };
        //    tc.Steps.Add(step);

        //    // Arrange - create the controller with update accommodation data
        //    var controller = new AccommodationController(tc).WithIncomingValues(new FormCollection {
        //        { "AccommodationId", "1" }, { "Name", "Name Changed" }, { "CheckIn", "2018-08-10" }, { "CheckOut", "2018-08-11" }
        //    });

        //    // Act - try to save the activity
        //    ActionResult result = await controller.Edit(1);

        //    // Assert - check the method result type
        //    Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        //}

        //[TestMethod]
        //public async Task EditAccommodationUpdatesBudget()
        //{
        //    // Arrange - create mock repository
        //    TestTrekStoriesContext tc = new TestTrekStoriesContext();
        //    // Arrange - create the controller
        //    ActivitiesController controller = new ActivitiesController(tc);
        //    // Arrange - create a transport
        //    Trip trip = new Trip { TripId = 321, TotalCost = 100 };
        //    Transport transport = new Transport() { ID = 1, Name = "Bus Transportation", Price = 60 };
        //    transport.Step = new Step { StepId = 123, Trip = trip };
        //    tc.Trips.Add(trip);
        //    tc.Activities.Add(transport);
        //    Transport updatedTransport = new Transport()
        //    {
        //        ID = 1,
        //        Name = "Name Change",
        //        Destination = "Roma",
        //        Duration = 20,
        //        StartTime = new DateTime(2018, 8, 1, 9, 30, 0),
        //        Price = 50
        //    };
        //    // Act - try to save the activity
        //    ActionResult result = await controller.EditTransport(updatedTransport);

        //    // Assert - check the method result type
        //    Assert.AreEqual(90, trip.TotalCost);
        //}

        //[TestMethod]
        //public async Task CannotEditAccommodationIfBeforeTripStartDate()
        //{
        //    // Arrange - create mock repository
        //    TestTrekStoriesContext tc = new TestTrekStoriesContext();
        //    // Arrange - create the controller
        //    ActivitiesController controller = new ActivitiesController(tc);
        //    // Arrange - create a leisure activity
        //    LeisureActivity leisure = new LeisureActivity() { ID = 1, Name = "Boat Trip" };
        //    // Arrange - add an error to the model state
        //    controller.ModelState.AddModelError("error", "error");
        //    // Act - try to save the activity
        //    ActionResult result = await controller.EditLeisure(leisure);

        //    // Assert - check the method result type
        //    Assert.IsInstanceOfType(result, typeof(ViewResult));
        //}

        //[TestMethod]
        //public async Task CannotEditAccommodationIfOneAlreadyExistsForThoseDates()
        //{
        //    // Arrange - create mock repository
        //    TestTrekStoriesContext tc = new TestTrekStoriesContext();
        //    // Arrange - create the controller
        //    ActivitiesController controller = new ActivitiesController(tc);
        //    // Arrange - create a leisure activity
        //    LeisureActivity leisure = new LeisureActivity() { ID = 1, Name = "Boat Trip" };
        //    // Arrange - add an error to the model state
        //    controller.ModelState.AddModelError("error", "error");
        //    // Act - try to save the activity
        //    ActionResult result = await controller.EditLeisure(leisure);

        //    // Assert - check the method result type
        //    Assert.IsInstanceOfType(result, typeof(ViewResult));
        //}

        [TestMethod()]
        public async Task DeleteReturnsCorrectAccommodation()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Accommodation acc = new Accommodation { AccommodationId = 10, Name = "Hotel Test" };
            tc.Accommodations.Add(acc);
            var controller = new AccommodationController(tc);
            // Act - delete the product
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
            Trip trip = new Trip { TripId = 1, Title = "Trip Name", TotalCost = 120 };
            tc.Trips.Add(trip);
            tc.Steps.Add(new Step { StepId = 123, Trip = trip, AccommodationId = 2 });
            Accommodation a2 = new Accommodation() { AccommodationId = 2, Name = "Test", Price = 70 };
            tc.Accommodations.Add(a2);
            // Arrange - create the controller
            AccommodationController controller = new AccommodationController(tc);
            // Act - delete an accommodation
            var result = await controller.DeleteConfirmed(2);
            // Assert - ensure that the accommodation is deleted from repository
            Assert.IsNull(tc.Accommodations.FirstOrDefault(a => a.AccommodationId == a2.AccommodationId));
            // Assert - ensure that the trip budget was updated
            Assert.AreEqual(trip.TotalCost, 50);
        }
    }
}