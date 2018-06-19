using System.Linq;
using TrekStories.Models;

namespace TrekStories.Tests
{
    class TestTripDbSet : TestDbSet<Trip>
    {
        public override Trip Find(params object[] keyValues)
        {
            return this.SingleOrDefault(trip => trip.TripId == (int)keyValues.Single());
        }
    }

    class TestStepDbSet : TestDbSet<Step>
    {
        public override Step Find(params object[] keyValues)
        {
            return this.SingleOrDefault(step => step.StepId == (int)keyValues.Single());
        }
    }

    class TestAccommodationDbSet : TestDbSet<Accommodation>
    {
        public override Accommodation Find(params object[] keyValues)
        {
            return this.SingleOrDefault(a => a.AccommodationId == (int)keyValues.Single());
        }
    }

    class TestActivityDbSet : TestDbSet<Activity>
    {
        public override Activity Find(params object[] keyValues)
        {
            return this.SingleOrDefault(act => act.ID == (int)keyValues.Single());
        }
    }

    class TestReviewDbSet : TestDbSet<Review>
    {
        public override Review Find(params object[] keyValues)
        {
            return this.SingleOrDefault(review => review.ReviewId == (int)keyValues.Single());
        }
    }
}
