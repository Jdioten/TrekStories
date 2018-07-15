using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrekStories.Models;
using System.Web.Mvc;

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
        public void DetailsTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void CreateTest()
        {
            throw new NotImplementedException();
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