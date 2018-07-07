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

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class StepControllerTests
    {
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
        public async Task CreateViewTest()
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
            var controller = new StepController(tc);
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
            
            var controller = new StepController(tc);
            var result = await controller.Create(stepViewModel) as RedirectToRouteResult;

            Step created = tc.Steps.First();


            Assert.AreEqual("Details", result.RouteValues["action"]);

            Assert.AreEqual("A", created.From);
            Assert.AreEqual(2.5, created.WalkingTime);
            Assert.AreEqual(12, created.WalkingDistance);
        }

        [TestMethod()]
        public async Task CannotCreateStepWithModelErrors()   //fails because of ToListAsync
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
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

            var controller = new StepController(tc);
            controller.ModelState.AddModelError("", "Error");
            var result = await controller.Create(stepViewModel) as ViewResult;

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
        }

        [TestMethod()]
        public void EditTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EditPostTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            Assert.Fail();
        }
    }
}