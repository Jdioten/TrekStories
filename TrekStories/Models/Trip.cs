using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrekStories.Models
{
    public enum TripCategory
    {
        coastal, mountainous, forest, desert, cultural, unclassifiable
    }

    //public enum Country?
    //or use below?
    // GetCountries() method
    //IEnumerable<string> GetCountries()
    //{
    //    return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
    //                      .Select(x => new RegionInfo(x.LCID).EnglishName)
    //                      .Distinct()
    //                      .OrderBy(x => x);
    //}

    public class Trip
    {
        [Key]
        public int TripId { get; set; }

        [Required(ErrorMessage ="Please give your trip a descriptive title.")]
        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please specify a country.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please specify a category.")]
        public TripCategory TripCategory { get; set; }

        [Required(ErrorMessage = "Please indicate the trip start date.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }

        [StringLength(500, ErrorMessage = "Notes are limited to 500 characters maximum.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Total Cost")]
        [DataType(DataType.Currency)]
        public double TotalCost { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a positive distance.")]
        public int TotalWalkingDistance { get; set; }

        [Required]
        public string TripOwner { get; set; }

        public virtual ICollection<Step> Steps { get; set; }

        //public string UserId { get; set; }
        ////[ForeignKey("UserId")]
        //public virtual ApplicationUser TripOwner { get; set; }  
    }
}