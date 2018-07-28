using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrekStories.Models;
using System.Web.Mvc;
using TrekStories.Tests;

namespace TrekStories.Controllers.Tests
{
    [TestClass()]
    public class AccommodationControllerTests
    {
        [TestMethod()]
        public void AccommodationControllerTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void AccommodationControllerTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void IndexTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public async Task DetailsReturnsCorrectAccommodation()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            tc.Accommodations.Add(new Accommodation() { AccommodationId = 99 });
            AccommodationController controller = new AccommodationController(tc);

            // Act
            var result = await controller.Details(99) as ViewResult;
            var accommodation99 = (Accommodation)result.ViewData.Model;
            // Assert
            Assert.AreEqual(99, accommodation99.AccommodationId);
        }

        [TestMethod()]
        public async Task CanCreateAccommodation()
        {
            //Arrange
            TestTrekStoriesContext tc = new TestTrekStoriesContext();
            AccommodationController controller = new AccommodationController(tc);
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 28, 14, 0, 0), CheckOut = new DateTime(2018, 11, 29, 10, 0, 0) };

            // Act
            var result = await controller.Create(newAccommodation) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod()]
        public async Task CannotCreateAccommodationIfModelError()
        {
            //Arrange
            AccommodationController controller = new AccommodationController();
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 29), CheckOut = new DateTime(2018, 11, 28, 10, 0, 0) };

            var result = await controller.Create(newAccommodation) as ViewResult;
            controller.ModelState.AddModelError("", "Error");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData.ModelState[""].Errors);
        }

        [TestMethod()]
        public async Task CannotCreateAccommodationWithInvalidCheckInOut()
        {
            AccommodationController controller = new AccommodationController();
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 29), CheckOut = new DateTime(2018, 11, 28, 10, 0, 0)};

            var result = await controller.Create(newAccommodation) as ViewResult;

            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ViewData.ModelState.Count == 1,
                 "Please check the check-in and check-out dates. Check-out cannot be before check-in.");
        }

        //to be completed
        [TestMethod()]
        public async Task CannotCreateAccommodationForStepsWhereAccommodationExists()
        {
            TestTrekStoriesContext tc = new TestTrekStoriesContext();

            //add step with accommodation

            AccommodationController controller = new AccommodationController();

            //try to add accommodation for that step with existing acommodation..
            Accommodation newAccommodation = new Accommodation() { CheckIn = new DateTime(2018, 11, 29), CheckOut = new DateTime(2018, 11, 28, 10, 0, 0) };

            var result = await controller.Create(newAccommodation) as ViewResult;

            Assert.IsTrue(!controller.ModelState.IsValid);
            //change text model error
            Assert.IsTrue(controller.ViewData.ModelState.Count == 1,
                 "Please check the check-in and check-out dates. Check-out cannot be before check-in.");
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