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
            Transport transport1 = new Transport
            {
                ID = 1,
                Name = "Test1",
                TransportType = TransportType.tram,
                StartTime = new DateTime(2018, 6, 12, 9, 22, 0),
                Duration = 20
            };

            Transport transport2 = new Transport
            {
                ID = 2,
                Name = "Test2",
                TransportType = TransportType.train,
                StartTime = new DateTime(2018, 6, 12, 9, 22, 0),
                Duration = 70
            };

            Transport transport3 = new Transport
            {
                ID = 3,
                Name = "Test3",
                TransportType = TransportType.boat,
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
