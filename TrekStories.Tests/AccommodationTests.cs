using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Models;
using System.Collections.Generic;

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

            Assert.AreEqual(true, acc.IsCheckInBeforeCheckOut());
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

            Assert.AreEqual(false, acc1.IsCheckInBeforeCheckOut());
            Assert.AreEqual(false, acc2.IsCheckInBeforeCheckOut());
        }

        [TestMethod]
        public void CanGetDatesBetweenCheckInAndCheckOut()
        {
            Accommodation acc1 = new Accommodation()
            {
                CheckIn = new DateTime(2018, 2, 3, 9, 0, 0),
                CheckOut = new DateTime(2018, 2, 5, 14, 0, 0)
            };

            var dates = acc1.GetDatesBetweenInAndOut();
            var expected = new List<DateTime>();
            expected.Add(new DateTime(2018, 2, 3, 0, 0, 0));
            expected.Add(new DateTime(2018, 2, 4, 0, 0, 0));

            Assert.AreEqual(2, dates.Count);
            CollectionAssert.AreEqual(expected, dates);
        }
    }
}
