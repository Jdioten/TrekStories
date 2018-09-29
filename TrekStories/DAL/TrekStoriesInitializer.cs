using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TrekStories.Models;

namespace TrekStories.DAL
{
    public class TrekStoriesInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<TrekStoriesContext>
    {
        protected override void Seed(TrekStoriesContext context)
        {
            //var trips = new List<Trip>
            //{
            //new Trip{Title="GR201 in Mallorca", Country="Spain", TripCategory=TripCategory.coast,
            //    StartDate = new DateTime(2015, 8, 2), TripOwner = "ABC123"},
            //new Trip{Title="Slieve Bloom Way", Country="Ireland", TripCategory=TripCategory.forest,
            //    StartDate = new DateTime(2015, 4, 12), TripOwner = "ABC123"},

            //trips.ForEach(t => context.Trips.Add(t));
            //context.SaveChanges();
        }
    }
}