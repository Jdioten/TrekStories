using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrekStories.Models
{
    public class Accommodation
    {
        [Key]
        public int AccommodationId { get; set; }
        [Required(ErrorMessage = "Please enter the name of your accommodation.")]
        [StringLength(40, ErrorMessage = "Accommodation Name cannot be longer than 40 characters.")]
        public string Name { get; set; }

        [StringLength(150)]
        public string Address { get; set; }
        //[RegularExpression(@"1\d{10}$", ErrorMessage="Number must be 10 digits long."] what if country prefix?
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your estimated check-in time.")]
        [Display(Name = "Check-In")]
        public DateTime CheckIn { get; set; }

        [Required(ErrorMessage = "Please enter your check-out date and time.")]
        [Display(Name = "Check-Out")]
        public DateTime CheckOut { get; set; }

        [Display(Name = "Booking Confirmation Document")]
        public string ConfirmationFileUrl { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price.")]
        //or
        //[DataType(DataType=Currency)]
        //[Column(TypeName="money")]
        //and add in web config <globalization culture = "auto" uiCulture="auto" />
        public double Price { get; set; }

        public virtual ICollection<Step> Steps { get; set; }
    }
}