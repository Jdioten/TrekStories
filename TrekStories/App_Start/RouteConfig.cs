using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TrekStories
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Step Creation",
                url: "Create/onTrip{tripId}/Step{seqNo}",
                defaults: new { Controller = "Step", action = "Create" }
            );

            routes.MapRoute(
                name: "Accommodation List",
                url: "Accommodation/Index/{tripId}/{sortOrder}",
                defaults: new { Controller = "Accommodation", action = "Index", sortOrder = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
