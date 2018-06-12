using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        //range from 1 to 5
        public int Rating { get; set; }
        public string PrivateNotes { get; set; }
        public string PublicNotes { get; set; }
        public List<string> PicturesUrl { get; set; }

        public int StepId { get; set }
        public virtual Step Step { get; set; }
    }
}