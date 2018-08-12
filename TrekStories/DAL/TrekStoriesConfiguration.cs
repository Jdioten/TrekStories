using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace TrekStories.DAL
{
    public class TrekStoriesConfiguration : DbConfiguration
    {
        public TrekStoriesConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}