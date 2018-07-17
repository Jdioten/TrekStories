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
            var result = await controller.EditLeisure(leisureToCreate) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            //Assert.AreEqual("Activities", result.RouteValues["controller"]);
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

        [TestMethod]
        public async Task CanEditTransport()
        {
            // Arrange - create the mock repository with leisure activities
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Transport transport1 = new Transport() { ID = 1, Name = "Train" };
            Transport transport2 = new Transport() { ID = 2, Name = "Bus" };
            tc.Activities.Add(transport1);
            tc.Activities.Add(transport2);
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Act
            var result1 = await controller.Edit(1) as ViewResult;
            var t1 = (Transport)result1.ViewData.Model;
            var result2 = await controller.Edit(2) as ViewResult;
            var t2 = (Transport)result2.ViewData.Model;
            // Assert
            Assert.AreEqual(1, t1.ID);
            Assert.AreEqual(2, t2.ID);
            Assert.AreEqual("Train", t1.Name);
        }

        [TestMethod]
        public async Task CanEditLeisure()
        {
            // Arrange - create the mock repository with leisure activities
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            LeisureActivity leisure1 = new LeisureActivity() { ID = 1, Name = "Boat Trip"};
            LeisureActivity leisure2 = new LeisureActivity() { ID = 2, Name = "Museum Visit"};
            tc.Activities.Add(leisure1);
            tc.Activities.Add(leisure2);
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Act
            var result1 = await controller.Edit(1) as ViewResult;
            var l1 = (LeisureActivity)result1.ViewData.Model;
            var result2 = await controller.Edit(2) as ViewResult;
            var l2 = (LeisureActivity)result2.ViewData.Model;
            // Assert
            Assert.AreEqual(1, l1.ID);
            Assert.AreEqual(2, l2.ID);
            Assert.AreEqual("Boat Trip", l1.Name);
        }

        [TestMethod]
        public async Task CannotEditNonexistentLeisure()
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            LeisureActivity leisure1 = new LeisureActivity() { ID = 1, Name = "Boat Trip" };
            tc.Activities.Add(leisure1);
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Act
            var badResult = await controller.Edit(4);
            // Assert
            Assert.IsInstanceOfType(badResult, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public async Task CanSaveValidLeisureActivityChanges()
        {
            // Arrange - create mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Arrange - create a leisure activity
            LeisureActivity leisure = new LeisureActivity() { ID = 1, Name = "Boat Trip" };
            // Act - try to save the activity
            ActionResult result = await controller.EditLeisure(leisure);

            // Assert - check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task CannotSaveInvalidChanges()
        {
            // Arrange - create mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Arrange - create a leisure activity
            LeisureActivity leisure = new LeisureActivity() { ID = 1, Name = "Boat Trip" };
            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");
            // Act - try to save the activity
            ActionResult result = await controller.EditLeisure(leisure);

            // Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task CanDeleteValidLeisureActivity()
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            tc.Activities.Add(new LeisureActivity() { ID = 1, Name = "Aquapark" });
            LeisureActivity l2 = new LeisureActivity() { ID = 2, Name = "Test" };
            tc.Activities.Add(l2);
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Act - delete an activity
            var result = await controller.Delete(2);
            // Assert - ensure that the activity is deleted from repository
            Assert.IsNull(tc.Activities.FirstOrDefault(l => l.ID == l2.ID));
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