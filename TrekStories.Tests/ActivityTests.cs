using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TrekStories.Models;

namespace TrekStories.Tests
{
    [TestClass]
    class ActivityTests
    {
        [TestMethod]
        public void CanSetArrivalTime()
        {
            Transport transport1 = new Transport {
                StartTime = new DateTime(2018, 6, 12, 9, 22, 0),
                Duration = 20
            };

            Transport transport2 = new Transport
            {
                StartTime = new DateTime(2018, 6, 12, 9, 22, 0),
                Duration = 70
            };

            Transport transport3 = new Transport
            {
                StartTime = new DateTime(2018, 6, 12, 9, 22, 0),
                Duration = 1440 //24 hrs
            };

            DateTime result1 = new DateTime(2018, 6, 12, 9, 42, 0);
            DateTime result2 = new DateTime(2018, 6, 12, 10, 32, 0);
            DateTime result3 = new DateTime(2018, 6, 13, 9, 22, 0);

            Assert.AreEqual(result1, transport1.ArrivalTime);
            Assert.AreEqual(result2, transport2.ArrivalTime);
            Assert.AreEqual(result3, transport3.ArrivalTime);
        }
    }
}
