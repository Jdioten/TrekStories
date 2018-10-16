namespace TrekStories.Models
{
    public class TripSearchModel
    {
        public string TitleKeyword { get; set; }
        public string Country { get; set; }
        public TripCategory? TripCategory { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
    }
}