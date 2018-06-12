using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public enum TransportType
    {
        boat, plane, train, tram, metro //populate further!!!
    }


    public class Transport : Activity
    {
        public TransportType TransportType { get; set; }
        public string Company { get; set; }
        public string Destination { get; set; }
        public int Duration { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}