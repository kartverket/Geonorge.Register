using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kartverket.Register
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            

            routes.MapRoute("DisplayDataset", "datasett/{id}/{name}",
                new { controller = "DisplayDataset", action = "Display", name = UrlParameter.Optional },
                new { id = @"^\d+$" }
            );

            routes.MapRoute("Dataset", "datasett", new { controller = "DisplayDataset", action = "Index" });

            routes.MapRoute("DokDekning", "register/det-offentlige-kartgrunnlaget/dekning", new { controller = "DokDekning", action = "Index" });

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Registers", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
