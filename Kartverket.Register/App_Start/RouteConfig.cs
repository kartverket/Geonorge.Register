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

            routes.MapRoute("DokReport", "api/register/det-offentlige-kartgrunnlaget/rapport", new { controller = "ApiRoot", action = "GetDokStatusReport" });
            routes.MapRoute("Dataset", "datasett", new { controller = "DisplayDataset", action = "Index" });
            routes.MapRoute("DokDekning", "register/det-offentlige-kartgrunnlaget/dekning", new { controller = "DokCoverage", action = "Index" });
            routes.MapRoute("InspireMonitoring", "api/register/inspire-statusregister/monitoring-report", new { controller = "ApiRoot", action = "InspireMonitoring" });
            routes.MapRoute("InspireDatasetMonitoring", "api/register/{registerName}/{itemowner}/{item}/monitoring-report", new { controller = "ApiRoot", action = "InspireDatasetMonitoring" });

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Registers", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
