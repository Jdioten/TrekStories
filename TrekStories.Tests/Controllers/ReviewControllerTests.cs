using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrekStories.Models;
using TrekStories.Tests;
using TrekStories.Tests.UnitTestHelpers;
using Moq;
using System.Collections.Generic;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class ReviewControllerTests
    {
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
            // Arrange - create a review
            Step step = new Step { StepId = 21, Trip = new Trip { TripId = 321, TripOwner = "ABC123" } };
            tc.Steps.Add(step);
            Review review = new Review() { StepId = 21, Step = step, PrivateNotes = "Test" };
            // Arrange - create the controller
            ReviewController controller = new ReviewController(tc).WithAuthenticatedUser("ABC123");
            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");
            // Act - try to save the review
            ActionResult result = await controller.Edit(review);
            // Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task EditReviewReturnsCorrectDetails()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            var expectedReview = new Review
            {
                ReviewId = 1,
                PrivateNotes = "Test Private",
                StepId = 1,
                Rating = 3,
                Images = new List<Image>()
            };
            tc.Reviews.Add(expectedReview);
            tc.Steps.Add(new Step { StepId = 1, From = "A", To = "B", Review = expectedReview });

            ReviewController controller = new ReviewController(tc);

            // act
            var result = await controller.Edit(1) as ViewResult;
            var resultData = (Review)result.ViewData.Model;

            // assert
            Assert.AreEqual(expectedReview.Rating, resultData.Rating);
            Assert.AreEqual(expectedReview.PrivateNotes, resultData.PrivateNotes);
        }

        [TestMethod]
        public async Task CannotEditNonexistentReview()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            var review = new Review
            {
                ReviewId = 1,
                PrivateNotes = "Test Private",
                StepId = 1,
                Rating = 3
            };
            tc.Reviews.Add(review);
            var controller = new ReviewController(tc);

            // Act
            var badResult = await controller.Edit(2) as ViewResult;
            // Assert
            Assert.AreEqual("CustomisedError", badResult.ViewName);
        }

        [TestMethod]
        public async Task EditForNullIdReturnsBadRequest()
        {
            var controller = new ReviewController(new TestTrekStoriesContext());
            var expected = (int)System.Net.HttpStatusCode.BadRequest;

            int? test = null;
            var badResult = await controller.Edit(test) as HttpStatusCodeResult;
            Assert.AreEqual(expected, badResult.StatusCode);
        }

        [TestMethod]
        public async Task CanEditReview()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            var review = new Review
            {
                ReviewId = 1,
                PrivateNotes = "Test Private",
                StepId = 1,
                Rating = 3
            };
            tc.Reviews.Add(review);
            Step step = new Step { StepId = 1, Trip = new Trip { TripId = 10, TripOwner = "ABC123" } };
            tc.Steps.Add(step);

            ReviewController controller = new ReviewController(tc).WithAuthenticatedUser("ABC123");

            // Act
            var updatedReview = new Review
            {
                ReviewId = 1,
                PrivateNotes = "New notes",
                StepId = 1,
                Rating = 4
            };
            var result = await controller.Edit(updatedReview);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public async Task CannotEditSomebodyElseReview()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            var review = new Review
            {
                ReviewId = 1,
                PrivateNotes = "Test Private",
                StepId = 1,
                Rating = 3
            };
            tc.Reviews.Add(review);
            Step step = new Step { StepId = 1, Trip = new Trip { TripId = 10, TripOwner = "ABC123" } };
            tc.Steps.Add(step);

            ReviewController controller = new ReviewController(tc).WithAuthenticatedUser("AnotherUser");

            // Act
            var updatedReview = new Review
            {
                ReviewId = 1,
                PrivateNotes = "New notes",
                StepId = 1,
                Rating = 4
            };
            var result = await controller.Edit(updatedReview) as ViewResult;

            // Assert
            Assert.AreEqual("CustomisedError", result.ViewName);
        }

        [TestMethod()]
        public async Task CannotUploadNullImage()
        {
            ReviewController controller = new ReviewController();

            //mock Url used in RedirectResult
            var UrlHelperMock = new Mock<UrlHelper>();
            controller.Url = UrlHelperMock.Object;
            UrlHelperMock.Setup(x => x.Action("Edit", "Review", new { id = "1#AddPhoto" })).Returns("testUrl");

            await controller.UploadImageAsync(null, 1);
            Assert.AreEqual("Please browse for a file to upload.", controller.TempData["message"]);
        }

        [TestMethod()]
        public async Task CannotUploadImageWithInvalidSize()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step = new Step { StepId = 12, Trip = new Trip { TripId = 214, TripOwner = "User1" } };
            var review = new Review
            {
                ReviewId = 1,
                PrivateNotes = "Test Private",
                Step = step
            };
            tc.Reviews.Add(review);
            ReviewController controller = new ReviewController(tc).WithAuthenticatedUser("User1");
            var result = await controller.UploadImageAsync(new TestPostedFileBase(7168020), 1) as ViewResult;
            Assert.AreEqual("The file cannot be bigger than 7MB.", controller.TempData["message"]);
        }

        [TestMethod()]
        public async Task CannotUploadImageWithInvalidExtension()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step = new Step { StepId = 12, Trip = new Trip { TripId = 214, TripOwner = "User1" } };
            var review = new Review
            {
                ReviewId = 1,
                PrivateNotes = "Test Private",
                Step = step
            };
            tc.Reviews.Add(review);
            ReviewController controller = new ReviewController(tc).WithAuthenticatedUser("User1");
            var result = await controller.UploadImageAsync(new TestPostedFileBase("test.js"), 1) as ViewResult;
            Assert.AreEqual("The file type is not authorized for upload.", controller.TempData["message"]);
        }

        //[TestMethod()]
        //public async Task CanUploadImage()
        //{
        //    TestTrekStoriesContext tc = new TestTrekStoriesContext();
        //    var review = new Review
        //    {
        //        ReviewId = 1,
        //        PrivateNotes = "Test Private",
        //        StepId = 1,
        //        Rating = 3
        //    };
        //    tc.Reviews.Add(review);
        //    Image image1 = new Image { Id = 25, ReviewId = 1, Url = "url1" };
        //    tc.Images.Add(image1);
        //    //Step step = new Step { StepId = 1, Trip = new Trip { TripId = 10, TripOwner = "ABC123" } };
        //    //tc.Steps.Add(step);

        //    ReviewController controller = new ReviewController(tc);
        //    TestPostedFileBase testFile = new TestPostedFileBase("test.txt", 800, new MemoryStream(Encoding.UTF8.GetBytes("whatever")));

        //    var result = await controller.UploadImage(testFile, 1) as RedirectToRouteResult;
        //    var imagesCount = tc.Images.Where(i => i.ReviewId == 1).Count();

        //    Assert.AreEqual("Edit", result.RouteValues["action"]);
        //    Assert.AreEqual("test", controller.TempData["message"]);
        //    Assert.AreEqual(2, imagesCount);
        //}

        [TestMethod()]
        public async Task CanDeleteImage()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            Step step = new Step { StepId = 12, Trip = new Trip { TripId = 214, TripOwner = "User1" } };
            Image image1 = new Image { Id = 25, ReviewId = 1, Url = "url1", Review = new Review { ReviewId = 1, Step = step } };
            tc.Images.Add(image1);
            ReviewController controller = new ReviewController(tc).WithAuthenticatedUser("User1");

            var UrlHelperMock = new Mock<UrlHelper>();
            controller.Url = UrlHelperMock.Object;
            UrlHelperMock.Setup(x => x.Action("Edit", "Review", new { id = "12#AddPhoto" })).Returns("testUrl");

            var result = await controller.DeleteImageAsync(25) as RedirectToRouteResult;

            Assert.IsNull(tc.Images.Find(25));
        }
    }
}