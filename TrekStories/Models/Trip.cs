using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace TrekStories.Models
{
    public enum TripCategory
    {
        coast, countryside, mountains, forest, desert, architecture, unclassifiable
    }

    
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
        [Display(Name = "Trip Category")]
        public TripCategory TripCategory { get; set; }

        [Required(ErrorMessage = "Please indicate the trip start date.")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public int Duration { get; private set; }

        [StringLength(500, ErrorMessage = "Notes are limited to 500 characters maximum.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Total Cost")]
        [DataType(DataType.Currency)]
        public double TotalCost { get; set; }

        public double TotalWalkingDistance { get; set; }

        public string TripOwner { get; set; }

        public virtual ICollection<Step> Steps { get; set; }

        //to create list of countries
        public static IEnumerable<string> GetCountries()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                              .Select(x => new RegionInfo(x.LCID).EnglishName)
                              .Distinct()
                              .OrderBy(x => x);
        }
    }
}