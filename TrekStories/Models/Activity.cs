using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public class Activity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "Activity Name cannot be longer than 40 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage ="The start time is required to order the activities chronologically.")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public double Price { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required]
        public int StepId { get; set; }
        public virtual Step Step { get; set; }
    }
}