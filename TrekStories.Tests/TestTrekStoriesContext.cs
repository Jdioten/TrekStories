using System;
using System.Data.Entity;
using System.Threading.Tasks;
using TrekStories.Abstract;
using TrekStories.Models;

namespace TrekStories.Tests
{
    class TestTrekStoriesContext : ITrekStoriesContext
    {
        public TestTrekStoriesContext()
        {
            this.Trips = new TestTripDbSet();
            this.Steps = new TestStepDbSet();
            this.Accommodations = new TestAccommodationDbSet();
            this.Activities = new TestActivityDbSet();
            this.Reviews = new TestReviewDbSet();
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public int SaveChanges()
        {
            return 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            return 0;
        }

        public void MarkAsModified(Object item) { }
        public void MarkAsDeleted(Object item) { }
        public void Dispose() { }
    }
}
