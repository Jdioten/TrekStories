using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public class Activity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public double Price { get; set; }
        public string Notes { get; set; }

        public int StepId { get; set; }
        public virtual Step Step { get; set; }
    }
}