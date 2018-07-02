using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Models;

namespace TrekStories.Tests
{
    [TestClass]
    public class AccommodationTests
    {
        [TestMethod]
        public void CanCheckInBeforeCheckingOut()
        {
            Accommodation acc = new Accommodation() {
                CheckIn = new DateTime(2018,2,3,14,0,0),
                CheckOut = new DateTime(2018, 2, 4, 9, 0, 0)
            };

            Assert.AreEqual(acc.IsCheckInBeforeCheckOut(), true);
        }

        [TestMethod]
        public void CannotCheckOutBeforeCheckingIn()
        {
            Accommodation acc1 = new Accommodation()
            {
                CheckIn = new DateTime(2018, 2, 3, 9, 0, 0),
                CheckOut = new DateTime(2018, 2, 3, 14, 0, 0)
            };

            Accommodation acc2 = new Accommodation()
            {
                CheckIn = new DateTime(2018, 2, 4, 14, 0, 0),
                CheckOut = new DateTime(2018, 2, 3, 9, 0, 0)
            };

            Assert.AreEqual(acc1.IsCheckInBeforeCheckOut(), false);
            Assert.AreEqual(acc2.IsCheckInBeforeCheckOut(), false);
        }
    }
}
