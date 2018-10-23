using System;
using System.ComponentModel.DataAnnotations;

namespace TrekStories.Models
{
    public class TripSearchModel
    {
        [Display(Name = "Title Keyword")]
        public string TitleKeyword { get; set; }
        public string Country { get; set; }
        [Display(Name = "Trip Category")]
        public TripCategory? TripCategory { get; set; }
        [Display(Name = "Minimum Duration")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please enter a number above 0.")]
        public int? MinDuration { get; set; }
        [Display(Name = "Maximum Duration")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please enter a number above 0.")]
        public int? MaxDuration { get; set; }
    }
}