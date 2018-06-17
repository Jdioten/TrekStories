using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public enum TransportType
    {
        boat, plane, train, tram, metro //populate further!!!
    }


    public class Transport : Activity
    {
        private DateTime arrivalTime;

        [Required (ErrorMessage = "Indicate the Transport Type.")]
        [Display(Name = "Transport Type")]
        public TransportType TransportType { get; set; }
        [Display(Name = "Travelling Company")]
        public string Company { get; set; }
        [Required(ErrorMessage = "Indicate the Destination.")]
        public string Destination { get; set; }
        [Required(ErrorMessage = "Indicate the Duration.")]
        public double Duration { get; set; }

        public DateTime ArrivalTime {
            get
            {
                return arrivalTime;
            }
            set
            {
                arrivalTime = StartTime.AddMinutes(Duration);
            }
        }
    }
}