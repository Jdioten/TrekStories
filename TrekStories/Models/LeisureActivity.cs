using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public enum LeisureCategory
    {
        aquatic, sports, museum, musical, cultural, nature, entertainmentparc, celebration, gastronomy, other
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