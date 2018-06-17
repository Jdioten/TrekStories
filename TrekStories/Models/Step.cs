using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public class Step
    {
        [Key]
        public int StepId { get; set; }
        [Required]
        public int SequenceNo { get; set; }
        [Required (ErrorMessage = "Please enter a starting point.")]
        public string From { get; set; }
        [Required (ErrorMessage = "Please enter an arrival point.")]
        public string To { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name = "Walking Time")]
        public int WalkingTime { get; set; }
        [Display(Name = "Walking Distance")]
        public int WalkingDistance { get; set; }
        public int Ascent { get; set; }

        [StringLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [StringLength(500, ErrorMessage = "Notes are limited to 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required]
        public int TripId { get; set; }
        public int AccommodationId { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual Accommodation Accommodation { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual Review Review { get; set; }
    }
}