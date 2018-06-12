using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrekStories.Startup))]
namespace TrekStories
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
