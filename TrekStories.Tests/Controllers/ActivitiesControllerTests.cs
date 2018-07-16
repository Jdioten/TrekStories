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
    public class ActivitiesControllerTests
    {
        [TestMethod()]
        public void ActivitiesControllerTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ActivitiesControllerTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void IndexTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void DetailsTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public async Task CanCreateLeisure()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            ActivitiesController controller = new ActivitiesController(tc);
            LeisureActivity leisureToCreate = new LeisureActivity() { Name = "Boat Trip", StartTime = new DateTime(2018, 7, 16, 9, 30, 0), LeisureCategory = LeisureCategory.aquatic };

            // Act
            var result = await controller.CreateLeisure(leisureToCreate) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual("Step", result.RouteValues["controller"]);
        }

        [TestMethod()]
        public void CreateTransportTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void CreateLeisureTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void CreateTransportTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void EditTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void EditLeisureTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void EditTransportTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void DeleteTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            throw new NotImplementedException();
        }
    }
}