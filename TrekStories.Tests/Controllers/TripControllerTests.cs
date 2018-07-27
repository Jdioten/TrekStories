using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Models;
using TrekStories.Tests;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class TripControllerTests
    {
        [TestMethod()]
        public async Task IndexContainsAllTrip() //modify later to test userId
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip trip1 = new Trip
            {
                Title = "Trip 1",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            };
            Trip trip2 = new Trip
            {
                Title = "Trip 2",
                Country = "Spain",
                TripCategory = TripCategory.coast,
                StartDate = new DateTime(2015, 4, 13),
                TripOwner = "ABC123"
            };
            Trip trip3 = new Trip
            {
                Title = "Trip 3",
                Country = "Belgium",
                TripCategory = TripCategory.countryside,
                StartDate = new DateTime(2015, 4, 16),
                TripOwner = "ABC123"
            };
            tc.Trips.Add(trip1);
            tc.Trips.Add(trip2);
            tc.Trips.Add(trip3);

            // Arrange - create a controller
            var controller = new TripController(tc);
            // Action
            var viewResult = await controller.Index(null) as ViewResult;
            Trip[] result = ((IEnumerable<Trip>)viewResult.ViewData.Model).ToArray();
            // Assert - ordered descending
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("Trip 3", result[0].Title);
            Assert.AreEqual("Trip 2", result[1].Title);
            Assert.AreEqual("Trip 1", result[2].Title);
        }

        [TestMethod()]
        public async Task DetailsReturnsCorrectTrip()
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

            var controller = new TripController(tc);

            var result = await controller.Details(1) as ViewResult;
            Trip t = (Trip)((ViewResult)result).Model;

            Assert.AreEqual("Test Trip", t.Title);
            Assert.AreEqual(TripCategory.forest, t.TripCategory);
            Assert.AreEqual(0, t.Duration);
            Assert.AreEqual(0, t.TotalCost);
            Assert.AreEqual(45, t.TotalWalkingDistance);
        }

        [TestMethod()]
        public async Task DetailsForNoIdReturnsBadRequest()
        {
            var controller = new TripController(new TestTrekStoriesContext());
            var expected = (int)System.Net.HttpStatusCode.BadRequest;

            var badResult = await controller.Details(null) as HttpStatusCodeResult;
            Assert.AreEqual(expected, badResult.StatusCode);
        }

        [TestMethod()]
        public async Task DetailsForNonExistingTripReturnsNotFound()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            var controller = new TripController(tc);

            var badResult = await controller.Details(1);

            Assert.IsInstanceOfType(badResult, typeof(HttpNotFoundResult));
        }

        [TestMethod()]
        public void CanValidateTrip()
        {
            Trip newTrip = new Trip
            {
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            };
            var context = new ValidationContext(newTrip, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(newTrip, context, result, true);

            Assert.IsTrue(valid);
        }

        [TestMethod()]
        public void DoesNotValidateTripWithNoName()
        {
            // Arrange
            Trip newTrip = new Trip
            {
                Title = "",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            };
            var context = new ValidationContext(newTrip, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(newTrip, context, result, true);

            Assert.IsFalse(valid);
            Assert.AreEqual(result.First().ErrorMessage, "Please give your trip a descriptive title.");
        }

        [TestMethod()]
        public async Task CanCreateTrip()
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

            var controller = new TripController(tc);
            var result = await controller.Create(newTrip) as RedirectToRouteResult;

            Assert.AreEqual("Details", result.RouteValues["action"]);
        }

        [TestMethod()]
        public async Task CannotCreateTripWithModelErrors()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip newTrip = new Trip
            {
                Title = "",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            };

            var controller = new TripController(tc);
            controller.ModelState.AddModelError("", "Error");
            var result = await controller.Create(newTrip) as ViewResult;

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
        }

        [TestMethod()]
        public async Task CannotCreateTripWithSameTitleForSameUser() //mind username assigned by controller when getting userid from current user!
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip trip = new Trip
            {
                Title = "Test",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "User1"
            };
            tc.Trips.Add(trip);
            Trip newTrip = new Trip
            {
                Title = "Test",
                Country = "Spain",
                TripCategory = TripCategory.coast,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "User1"
            };

            var controller = new TripController(tc);
            var result = await controller.Create(newTrip) as ViewResult;

            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.IsTrue(controller.ViewData.ModelState.Count == 1,
                 "You have already created a trip with that title. Please give this trip a different title.");
        }

        [TestMethod()]
        public async Task EditTripReturnsCorrectDetails()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            var expectedTrip = new Trip
            {
                TripId = 1,
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123",
                TotalWalkingDistance = 45
            };

            tc.Trips.Add(expectedTrip);
            var controller = new TripController(tc);

            // act
            var result = await controller.Edit(1) as ViewResult;
            var resultData = (Trip)result.ViewData.Model;

            // assert
            Assert.AreEqual(expectedTrip.Title, resultData.Title);
            Assert.AreEqual(expectedTrip.Country, resultData.Country);
            Assert.AreEqual(expectedTrip.TotalWalkingDistance, resultData.TotalWalkingDistance);
        }

        [TestMethod]
        public async Task Cannot_Edit_Nonexistent_Trip()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            var expectedTrip = new Trip
            {
                TripId = 1,
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123",
            };
            tc.Trips.Add(expectedTrip);
            var controller = new TripController(tc);

            // Act
            var badResult = await controller.Edit(2);
            // Assert
            Assert.IsInstanceOfType(badResult, typeof(HttpNotFoundResult));
        }
    }
}