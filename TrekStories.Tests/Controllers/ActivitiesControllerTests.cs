using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Models;
using TrekStories.Tests;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class ActivitiesControllerTests
    {
        [TestMethod()]
        public async Task DetailsReturnsCorrectView()
        {
            // Arrange - create the mock repository with leisure activities
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Transport transport1 = new Transport() { ID = 1, Name = "Train" };
            LeisureActivity leisure1 = new LeisureActivity() { ID = 2, Name = "Boat Trip" };
            LeisureActivity leisure2 = new LeisureActivity() { ID = 3, Name = "Museum Visit" };
            tc.Activities.Add(transport1);
            tc.Activities.Add(leisure1);
            tc.Activities.Add(leisure2);
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Act
            var result1 = await controller.Details(1) as ViewResult;
            var t1 = (Transport)result1.ViewData.Model;
            var result2 = await controller.Details(3) as ViewResult;
            var l2 = (LeisureActivity)result2.ViewData.Model;
            // Assert
            Assert.AreEqual("DetailsTransport", result1.ViewName);
            Assert.AreEqual(1, t1.ID);
            Assert.AreEqual("DetailsLeisure", result2.ViewName);
            Assert.AreEqual("Museum Visit", l2.Name);
        }

        [TestMethod()]
        public async Task CanCreateTransportOnStep()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            ActivitiesController controller = new ActivitiesController(tc);
            Step step = new Step() { StepId = 2 };
            tc.Steps.Add(step);

            // Act
            var result = await controller.CreateTransport(2) as ViewResult;

            // Assert
            Assert.AreEqual("EditTransport", result.ViewName);
            Assert.AreEqual(2, result.ViewBag.StepId);
        }

        [TestMethod()]
        public async Task CanCreateLeisureOnStep()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            ActivitiesController controller = new ActivitiesController(tc);
            Step step = new Step() { StepId = 2 };
            tc.Steps.Add(step);

            // Act
            var result = await controller.CreateLeisure(2) as ViewResult;

            // Assert
            Assert.AreEqual("EditLeisure", result.ViewName);
            Assert.AreEqual(2, result.ViewBag.StepId);
        }

        [TestMethod()]
        public async Task CanCreateLeisureWithEditMethodAndUpdateBudget()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            ActivitiesController controller = new ActivitiesController(tc);
            Step step = new Step { StepId = 2, Trip = new Trip { TripId = 321, TotalCost = 100} };
            tc.Steps.Add(step);
            LeisureActivity leisureToCreate = new LeisureActivity() { Name = "Boat Trip", StartTime = new DateTime(2018, 7, 16, 9, 30, 0), LeisureCategory = LeisureCategory.aquatic, Price = 20, StepId = 2 };

            // Act
            var result = await controller.EditLeisure(leisureToCreate) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual("Step", result.RouteValues["controller"]);
            Assert.AreEqual(120, step.Trip.TotalCost);
        }

        [TestMethod()]
        public async Task CreateTransportUpdatesBudget()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            ActivitiesController controller = new ActivitiesController(tc);
            Trip trip = new Trip { TripId = 1, TotalCost = 10, StartDate = new DateTime(2018, 7, 15, 9, 30, 0) };
            Step step = new Step { StepId = 2, TripId = 1, Trip = trip, SequenceNo = 1 };
            tc.Trips.Add(trip);
            tc.Steps.Add(step);
            Transport transportToCreate = new Transport() { Name = "Train to Paris", StartTime = new DateTime(2018, 7, 16, 9, 30, 0), Duration = 120, Price = 23, StepId = 2, Step = step };

            // Act
            var result = await controller.EditTransport(transportToCreate) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", result.RouteValues["action"]);
            Assert.AreEqual("Step", result.RouteValues["controller"]);
            Assert.AreEqual(33, trip.TotalCost);
            Assert.AreEqual(new DateTime(2018, 7, 15, 11, 30, 0), transportToCreate.GetArrivalTime());
        }

        [TestMethod]
        public async Task CanEditTransport()
        {
            // Arrange - create the mock repository with leisure activities
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step1 = new Step { StepId = 1, From = "A", To = "B" };
            Step step2 = new Step { StepId = 2, From = "B", To = "C" };
            Transport transport1 = new Transport() { ID = 1, Name = "Train", StepId = 1, Step = step1 };
            Transport transport2 = new Transport() { ID = 2, Name = "Bus", StepId = 2, Step = step2 };
            tc.Steps.Add(step1);
            tc.Steps.Add(step2);
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
            Step step1 = new Step { StepId = 1, From = "A", To = "B" };
            Step step2 = new Step { StepId = 2, From = "B", To = "C" };
            LeisureActivity leisure1 = new LeisureActivity() { ID = 1, Name = "Boat Trip", StepId = 1, Step = step1};
            LeisureActivity leisure2 = new LeisureActivity() { ID = 2, Name = "Museum Visit", StepId = 2, Step = step2};
            tc.Steps.Add(step1);
            tc.Steps.Add(step2);
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
            leisure.Step = new Step { StepId = 123, Trip = new Trip { TripId = 321, TotalCost = 100 } };
            tc.Activities.Add(leisure);
            LeisureActivity updatedLeisure = new LeisureActivity()
            {
                ID = 1,
                Name = "Name Changed",
                StartTime = new DateTime(2018, 8, 1, 14, 30, 0)
            };
            // Act - try to save the activity
            ActionResult result = await controller.EditLeisure(leisure);

            // Assert - check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task EditTransportUpdatesBudget()
        {
            // Arrange - create mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Arrange - create a transport
            Trip trip = new Trip { TripId = 321, TotalCost = 100 };
            Transport transport = new Transport() { ID = 1, Name = "Bus Transportation", Price = 60 };
            transport.Step = new Step { StepId = 123, Trip = trip };
            tc.Trips.Add(trip);
            tc.Activities.Add(transport);
            Transport updatedTransport = new Transport()
            {
                ID = 1,
                Name = "Name Change",
                Destination = "Roma",
                Duration = 20,
                StartTime = new DateTime(2018,8,1,9,30,0),
                Price = 50
            };
            // Act - try to save the activity
            ActionResult result = await controller.EditTransport(updatedTransport);

            // Assert - check the method result type
            Assert.AreEqual(90, trip.TotalCost);
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
        public async Task CanDeleteValidActivityAndUpdateBudget()
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Trip trip = new Trip { TripId = 1, Title = "Trip Name", TotalCost = 20 };
            tc.Trips.Add(trip);
            tc.Steps.Add(new Step { StepId = 123, Trip = trip});
            tc.Activities.Add(new LeisureActivity() { ID = 1, Name = "Aquapark" });
            LeisureActivity l2 = new LeisureActivity() { ID = 2, Name = "Test", StepId = 123, Price = 15 };
            tc.Activities.Add(l2);
            // Arrange - create the controller
            ActivitiesController controller = new ActivitiesController(tc);
            // Act - delete an activity
            var result = await controller.Delete(2);
            // Assert - ensure that the activity is deleted from repository
            Assert.IsNull(tc.Activities.FirstOrDefault(l => l.ID == l2.ID));
            // Assert - ensure that the trip budget was updated
            Assert.AreEqual(5, trip.TotalCost);
        }
    }
}