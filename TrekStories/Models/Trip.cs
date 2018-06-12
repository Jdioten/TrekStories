using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public int TripId { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public TripCategory TripCategory { get; set; }
        public int Duration { get; set; }
        public string Notes { get; set; }
        public double TotalCost { get; set; }
        public int TotalWalkingDistance { get; set; }

        public string UserId { get; set; }
        //[ForeignKey("UserId")]
        public virtual ApplicationUser TripOwner { get; set; }
        public virtual ICollection<Step> Steps { get; set; }
    }
}