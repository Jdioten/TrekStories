using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrekStories.Tests;
using TrekStories.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class TripControllerTests
    {
        [TestMethod()]
        public void TripControllerTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void TripControllerTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DetailsTest()
        {
            Assert.Fail();
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
        public void CanCreateTrip()
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
            var result = controller.Create(newTrip) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["action"]);            
        }

        [TestMethod()]
        public void CannotCreateTripWithModelErrors()
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
            var result = controller.Create(newTrip) as ViewResult;

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
        }

        [TestMethod()]
        public void EditTest()
        {
           // Trip t = (Trip)((ViewResult)result).Model;
           // Assert.AreEqual(t.Title, "Test Trip");
            //Assert.AreEqual(t.TripCategory, TripCategory.forest);
            //Assert.AreEqual(t.Duration, 0);
            //Assert.AreEqual(t.TotalCost, 0);
            //Assert.AreEqual(t.TotalWalkingDistance, 0);

            Assert.Fail();
        }

        [TestMethod()]
        public void EditPostTest()
        {
            Assert.Fail();
        }
    }
}