using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public enum LeisureCategory
    {
        watersports, sports, museum, musical, cultural, nature, entertainmentparc, celebration, gastronomy, other
    }

    public class LeisureActivity : Activity
    {
        public string Address { get; set; }
        public LeisureCategory LeisureCategory { get; set; }
    }
}