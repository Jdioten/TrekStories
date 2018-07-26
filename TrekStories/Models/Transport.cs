using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrekStories.Models
{
    public enum TransportType
    {
        boat, plane, train, tram, metro, bus, car, hitchhiking, bike, foot
    }


    public class Transport : Activity
    {
        [Required (ErrorMessage = "Indicate the Transport Type.")]
        [Display(Name = "Transport Type")]
        public TransportType TransportType { get; set; }
        [Display(Name = "Transport Company")]
        public string Company { get; set; }
        [Required(ErrorMessage = "Indicate the Destination.")]
        public string Destination { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "The duration is invalid.")]
        [Required(ErrorMessage = "Indicate the Duration.")]
        [Display(Name = "Duration (Minutes)")]
        public double Duration { get; set; }

        public DateTime ArrivalTime {
            get
            {
                return StartTime.AddMinutes(Duration);
            }
        }
    }
}