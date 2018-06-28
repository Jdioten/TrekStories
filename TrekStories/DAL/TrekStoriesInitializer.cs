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
            var trips = new List<Trip>
            {
            new Trip{Title="GR201 in Mallorca", Country="Spain", TripCategory=TripCategory.coast,
                StartDate = new DateTime(2015, 8, 2), TripOwner = "ABC123"},
            new Trip{Title="Slieve Bloom Way", Country="Ireland", TripCategory=TripCategory.forest,
                StartDate = new DateTime(2015, 4, 12), TripOwner = "ABC123"},
            new Trip{Title="Cooley Peninsula", Country="Ireland", TripCategory=TripCategory.coast,
                StartDate = new DateTime(2016, 4, 22), TripOwner = "ABC123"},
            new Trip{Title="Zagori Circuit", Country="Greece", TripCategory=TripCategory.cultural,
                StartDate = new DateTime(2016, 10, 7), TripOwner = "ABC123"},
            };

            trips.ForEach(t => context.Trips.Add(t));
            context.SaveChanges();

            /*var steps = new List<Step>
            {
            new Step{},
            };
            steps.ForEach(s => context.Steps.Add(s));
            context.SaveChanges();*/
        }
    }
}