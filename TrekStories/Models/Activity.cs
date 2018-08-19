using System;
using System.ComponentModel.DataAnnotations;

namespace TrekStories.Models
{
    public class Activity
    {
        [Key]
        public int ID { get; set; }
        
        [Required(ErrorMessage = "Activity Name is required.")]
        [StringLength(40, ErrorMessage = "Activity Name cannot be longer than 40 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage ="The start time is required to order the activities chronologically.")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a positive price.")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public double Price { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(200, ErrorMessage = "Notes are limited to 200 characters maximum.")]
        public string Notes { get; set; }

        [Required]
        public int StepId { get; set; }
        public virtual Step Step { get; set; }
    }
}