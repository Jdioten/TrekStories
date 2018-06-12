using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public class Accommodation
    {
        //[Key]
        public int AccommodationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        //[RegularExpression(@"1\d{10}$", ErrorMessage="Number must be 10 digits long."] what if country prefix?
        //or phone
        public string PhoneNumber { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string ConfirmationFileUrl { get; set; }
        //[DataType(DataType.Currency)]
        //[DisplayFormat(NullDisplayText = "n/a", ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        //or
        //[DataType(DataType=Currency)]
        //[Column(TypeName="money")]
        //and add in web config <globalization culture = "auto" uiCulture="auto" />
        public double Price { get; set; }

        public virtual ICollection<Step> Steps { get; set; }
    }
}