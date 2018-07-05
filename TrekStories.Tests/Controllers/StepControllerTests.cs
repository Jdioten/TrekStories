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
        public void CreateTest1()
        {
            Assert.Fail();
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