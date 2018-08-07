using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TrekStories.Abstract;
using TrekStories.Models;

namespace TrekStories.DAL
{
    public class TrekStoriesContext : DbContext, ITrekStoriesContext
    {
        public TrekStoriesContext() : base("TrekStoriesContext")
        {
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public void MarkAsDeleted(object item)
        {
            Entry(item).State = EntityState.Deleted;
        }
    }
}