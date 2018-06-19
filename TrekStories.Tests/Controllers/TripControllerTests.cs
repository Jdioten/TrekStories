using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrekStories.Tests;
using TrekStories.Models;

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
        public void CreateTest()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            tc.Trips.Add(new Trip
            {
                Title = "Test Trip",
                Country = "Ireland",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12),
                TripOwner = "ABC123"
            });

            //var controller = new DeploymentsController(tdc);
            //var result = controller.Details("a12");
            //Assert.IsNotNull(result);
            //Assert.IsInstanceOfType(result, typeof(ViewResult));
            //Deployment d = (Deployment)((ViewResult)result).Model;
            //Assert.AreEqual(d.AppName, "MyApp");

            //Of course I should also add a test that asserts a failure when running the details method 
            //with an invalid DeploymentID or an empty Deployments list.

            Assert.Fail();
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
    }
}