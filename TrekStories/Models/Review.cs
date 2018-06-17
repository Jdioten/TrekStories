using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public class Review
    {
        [Key, ForeignKey("Step")]
        public int ReviewId { get; set; }
        [Range(1,5)]
        public int Rating { get; set; }

        [Display(Name = "Private Notes")]
        [DataType(DataType.MultilineText)]
        public string PrivateNotes { get; set; }

        [Display(Name = "Public Notes")]
        [DataType(DataType.MultilineText)]
        public string PublicNotes { get; set; }

        [Display(Name = "Pictures")]
        public List<string> PicturesUrl { get; set; }

        [Required]
        public int StepId { get; set; }
        public virtual Step Step { get; set; }
    }
}