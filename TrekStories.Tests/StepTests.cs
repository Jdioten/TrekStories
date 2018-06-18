using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Models;

namespace TrekStories.Tests
{
    [TestClass]
    public class StepTests
    {
        [TestMethod]
        public void CanCalculateStepDate()
        {
            Trip trip1 = new Trip
            {
                Country = "Ireland",
                TripId = 1,
                Title = "Test Trip",
                TripCategory = TripCategory.forest,
                StartDate = new DateTime(2018, 4, 12)
            };
            Step step1 = new Step { StepId = 1, SequenceNo = 1, From = "Paris", To = "Beauvais", TripId = 1, Trip = trip1 };
            Step step10 = new Step { StepId = 2, SequenceNo = 10, From = "La Rochelle", To = "Nantes", TripId = 1, Trip = trip1 };

            DateTime expectedResult1 = new DateTime(2018, 4, 12);
            DateTime expectedResult2 = new DateTime(2018, 4, 21);

            Assert.AreEqual(expectedResult1, step1.Date);
            Assert.AreEqual(expectedResult2, step10.Date);
        }
    }
}
