using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrekStories.Models;

namespace TrekStories.Tests
{
    [TestClass]
    public class ActivityTests
    {
        [TestMethod]
        public void CanGetArrivalTime()
        {
            Trip trip = new Trip { StartDate = new DateTime(2018, 6, 1, 0, 0, 0) };

            Step step = new Step { SequenceNo = 3, Trip = trip };

            Transport transport1 = new Transport
            {
                ID = 2,
                Name = "Test1",
                TransportType = TransportType.train,
                StartTime = new DateTime(2018, 6, 12, 9, 22, 0),
                Duration = 70,
                Step = step
            };

            Transport transport2 = new Transport
            {
                ID = 3,
                Name = "Test2",
                TransportType = TransportType.boat,
                StartTime = new DateTime(2018, 6, 12, 9, 22, 0),
                Duration = 1440, //24 hrs
                Step = step
            };

            Assert.AreEqual(new DateTime(2018, 6, 3, 10, 32, 0), transport1.GetArrivalTime());
            Assert.AreEqual(new DateTime(2018, 6, 4, 9, 22, 0), transport2.GetArrivalTime());
        }
    }
}
