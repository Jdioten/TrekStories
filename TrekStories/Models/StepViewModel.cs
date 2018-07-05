using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrekStories.Models
{
    public class StepViewModel
    {
        [HiddenInput]
        public int? StepId { get; set; }
        [HiddenInput]
        public int SequenceNo { get; set; }
        [HiddenInput]
        public int TripId { get; set; }
        [HiddenInput]
        public int? AccommodationId { get; set; }

        [Required, StringLength(20, ErrorMessage = "Enter a starting point of maximum 20 characters.")]
        public string From { get; set; }
        [Required, StringLength(20, ErrorMessage = "Enter an arrival point of maximum 20 characters.")]
        public string To { get; set; }

        public int WalkingTimeHours { get; set; }
        public int WalkingTimeMinutes { get; set; }


        [Display(Name = "Walking Distance")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a positive distance.")]
        public double WalkingDistance { get; set; }
        public int Ascent { get; set; }

        [StringLength(100, ErrorMessage = "The description cannot be longer than 100 characters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "Notes are limited to 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}