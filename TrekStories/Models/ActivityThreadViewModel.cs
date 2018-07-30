using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public class ActivityThreadViewModel
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Icon { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string Controller { get; set; }
    }
}