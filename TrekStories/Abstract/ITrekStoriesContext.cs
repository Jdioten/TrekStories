using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrekStories.Models;

namespace TrekStories.Abstract
{
    public interface ITrekStoriesContext : IDisposable
    {
        DbSet<Trip> Trips { get; }
        DbSet<Step> Steps { get; }
        DbSet<Accommodation> Accommodations { get; }
        DbSet<Activity> Activities { get; }
        DbSet<Review> Reviews { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void MarkAsModified(Object item);
    }
}
