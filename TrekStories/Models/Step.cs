using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public class Step
    {
        public int StepId { get; set; }
        public int SequenceNo { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int WalkingTime { get; set; }
        public int WalkingDistance { get; set; }
        public int Ascent { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        public int TripId { get; set; }
        public int AccommodationID { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual Accommodation Accommodation { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual Review Review { get; set; }
    }
}