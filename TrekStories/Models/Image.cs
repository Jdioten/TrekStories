using System.ComponentModel.DataAnnotations;

namespace TrekStories.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }

        [Required]
        public int ReviewId { get; set; }
        public virtual Review Review { get; set; }
    }
}