using System.ComponentModel.DataAnnotations;

namespace TrekStories.Models
{
    public enum LeisureCategory
    {
        aquatic, sports, musical, cultural, nature, gastronomy, other
    }

    public class LeisureActivity : Activity
    {
        [StringLength(150)]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Leisure Category")]
        public LeisureCategory LeisureCategory { get; set; }
    }
}