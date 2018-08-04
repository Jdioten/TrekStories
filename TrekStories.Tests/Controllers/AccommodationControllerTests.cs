using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrekStories.Models;
using System.Web.Mvc;
using TrekStories.Tests;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class AccommodationControllerTests
    {
        [TestMethod()]
        public void AccommodationControllerTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void AccommodationControllerTest1()
        {
            throw new NotImplementedException();
        }

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
        public async Task CanCreateAccommodation()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            AccommodationController controller = new AccommodationController(tc);
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 28, 14, 0, 0), CheckOut = new DateTime(2018, 11, 29, 10, 0, 0) };

            // Act
            var result = await controller.Create(newAccommodation) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
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

        //to be completed
        [TestMethod()]
        public async Task CannotCreateAccommodationForStepsWhereAccommodationExists()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            //add step with accommodation

            AccommodationController controller = new AccommodationController();

            //try to add accommodation for that step with existing acommodation..
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 29), CheckOut = new DateTime(2018, 11, 28, 10, 0, 0) };

            var result = await controller.Create(newAccommodation) as ViewResult;

            Assert.IsTrue(!controller.ModelState.IsValid);
            //change text model error
            Assert.IsTrue(controller.ViewData.ModelState.Count == 1,
                 "Please check the check-in and check-out dates. Check-out cannot be before check-in.");
        }

        [TestMethod()]
        public void EditTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void EditTest1()
        {
            throw new NotImplementedException();
        }

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