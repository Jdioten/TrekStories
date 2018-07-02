using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrekStories.Models;

namespace TrekStories.Tests
{
    class TestTripDbSet : TestDbSet<Trip>
    {
        public override Trip Find(params object[] keyValues)
        {
            return this.SingleOrDefault(trip => trip.TripId == (int)keyValues.Single());
        }

        public override Task<Trip> FindAsync(params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }

        public override Task<Trip> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }
    }

    class TestStepDbSet : TestDbSet<Step>
    {
        public override Step Find(params object[] keyValues)
        {
            return this.SingleOrDefault(step => step.StepId == (int)keyValues.Single());
        }

        public override Task<Step> FindAsync(params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }

        public override Task<Step> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }
    }

    class TestAccommodationDbSet : TestDbSet<Accommodation>
    {
        public override Accommodation Find(params object[] keyValues)
        {
            return this.SingleOrDefault(a => a.AccommodationId == (int)keyValues.Single());
        }

        public override Task<Accommodation> FindAsync(params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }

        public override Task<Accommodation> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }
    }

    class TestActivityDbSet : TestDbSet<Activity>
    {
        public override Activity Find(params object[] keyValues)
        {
            return this.SingleOrDefault(act => act.ID == (int)keyValues.Single());
        }

        public override Task<Activity> FindAsync(params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }

        public override Task<Activity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }
    }

    class TestReviewDbSet : TestDbSet<Review>
    {
        public override Review Find(params object[] keyValues)
        {
            return this.SingleOrDefault(review => review.ReviewId == (int)keyValues.Single());
        }

        public override Task<Review> FindAsync(params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }

        public override Task<Review> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task.FromResult(Find(keyValues));
        }
    }
}
