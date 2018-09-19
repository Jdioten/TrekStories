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
using TrekStories.Tests.UnitTestHelpers;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class ReviewControllerTests
    {
        [TestMethod()]
        public void ReviewControllerTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ReviewControllerTest1()
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

        [TestMethod]
        public async Task CanCreateReviewOnStep()
        {
            // Arrange - create the mock repository with a step
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step1 = new Step { StepId = 1, From = "A", To = "B" };
            tc.Steps.Add(step1);
            // Arrange - create the controller
            ReviewController controller = new ReviewController(tc);
            // Act
            var result = await controller.Create(1) as ViewResult;
            var review = (Review)result.ViewData.Model;
            // Assert
            Assert.AreEqual("Edit", result.ViewName);
            Assert.AreEqual(1, review.StepId);
        }

        [TestMethod]
        public async Task CannotCreateReviewOnNonexistentStep()
        {
            // Arrange - create the mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step1 = new Step { StepId = 1, From = "A", To = "B" };
            tc.Steps.Add(step1);
            // Arrange - create the controller
            ReviewController controller = new ReviewController(tc);
            // Act
            var badResult = await controller.Create(4) as ViewResult;
            // Assert
            Assert.AreEqual("CustomisedError", badResult.ViewName);
        }

        [TestMethod]
        public async Task CanAddValidReview()
        {
            // Arrange - create mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            // Arrange - create the controller
            ReviewController controller = new ReviewController(tc).WithAuthenticatedUser("ABC123");
            // Arrange - create a review for an existing step
            Step step = new Step { StepId = 21, Trip = new Trip { TripId = 321, TripOwner = "ABC123" } };
            tc.Steps.Add(step);
            Review review = new Review() { StepId = 21, PrivateNotes = "Test" };
            // Act - try to save the step
            ActionResult result = await controller.Edit(review);
            // Assert - check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task CannotSaveInvalidReview()
        {
            // Arrange - create mock repository
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            // Arrange - create the controller
            ReviewController controller = new ReviewController(tc);
            // Arrange - create a review
            Step step = new Step { StepId = 21, Trip = new Trip { TripId = 321, TripOwner = "ABC123" } };
            tc.Steps.Add(step);
            Review review = new Review() { StepId = 21, Step = step, PrivateNotes = "Test" };
            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");
            // Act - try to save the review
            ActionResult result = await controller.Edit(review);
            // Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
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
        public void UploadImageTest()
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