using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Required]
        [StringLength(20, ErrorMessage = "Enter a starting point of maximum 20 characters.")]
        public string From { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Enter an arrival point of maximum 20 characters.")]
        public string To { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        public DateTime Date
        {
            get
            {
                if (Trip != null)
                {
                    return Trip.StartDate.AddDays(SequenceNo - 1.0);
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }

        [Display(Name = "Walking Time")]
        public double WalkingTime { get; set; }
        [Display(Name = "Walking Distance")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a positive distance.")]
        public double WalkingDistance { get; set; }
        public int Ascent { get; set; }

        [StringLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "Notes are limited to 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required]
        public int TripId { get; set; }
        public int? AccommodationId { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual Accommodation Accommodation { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual Review Review { get; set; }
    }
}