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
        [StringLength(40, ErrorMessage = "The accommodation name cannot be longer than 40 characters.")]
        public string Name { get; set; }

        [StringLength(100)]
        [Display(Name = "Street Address")]
        public string Street { get; set; }

        [StringLength(50)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your estimated check-in time.")]
        [Display(Name = "Check-In")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime CheckIn { get; set; }

        [Required(ErrorMessage = "Please enter your check-out date and time.")]
        [Display(Name = "Check-Out")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime CheckOut { get; set; }

        [Display(Name = "Booking Confirmation Document")]
        public string ConfirmationFileUrl { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:c}")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a positive price.")]
        public double Price { get; set; }


        public bool IsCheckInBeforeCheckOut()
        {
            if (CheckIn.Date < CheckOut.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<DateTime> GetDatesBetweenInAndOut()
        {
            var dates = new List<DateTime>();

            for (var dt = CheckIn.Date; dt < CheckOut.Date; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            return dates;
        }
    }
}