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

            // EPSG
            routes.MapRoute("CreateEPSGsub", "epsg/{parentRegister}/{registerowner}/{registername}/ny", new { controller = "EPSGs", action = "Create", registername = "registername" });
            routes.MapRoute("CreateEPSG", "epsg/{registername}/ny", new { controller = "EPSGs", action = "Create", registername = "registername" });
            routes.MapRoute("EditEPSGsub", "epsg/{parentRegister}/{registerowner}/{registername}/{itemowner}/{epsgname}/rediger", new { controller = "EPSGs", action = "Edit", registername = "registername" });
            routes.MapRoute("EditEPSG", "epsg/{registername}/{organization}/{epsgname}/rediger", new { controller = "EPSGs", action = "Edit", registername = "registername" });
            routes.MapRoute("DeleteEPSGsub", "epsg/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{epsgname}/slett", new { controller = "EPSGs", action = "Edit", registername = "registername" });
            routes.MapRoute("DeleteEPSG", "epsg/{registername}/{organization}/{epsgname}/slett", new { controller = "EPSGs", action = "Edit", registername = "registername" });
            routes.MapRoute("SignIn", "AuthServices/SignIn", new { controller = "AuthServices", action = "SignIn" });
            routes.MapRoute("LogOut", "AuthServices/LogOut", new { controller = "AuthServices", action = "LogOut" });
            routes.MapRoute("DokReport", "api/register/det-offentlige-kartgrunnlaget/rapport", new { controller = "ApiRoot", action = "GetDokStatusReport" });
            routes.MapRoute("Dataset", "datasett", new { controller = "DisplayDataset", action = "Index" });
            routes.MapRoute("DokDekning", "register/det-offentlige-kartgrunnlaget/dekning", new { controller = "DokCoverage", action = "Index" });
            routes.MapRoute("InspireMonitoring", "api/register/inspire-statusregister/monitoring-report", new { controller = "ApiRoot", action = "InspireMonitoring" });
            routes.MapRoute("InspireDatasetMonitoring", "api/register/{registerName}/{itemowner}/{item}/monitoring-report", new { controller = "ApiRoot", action = "InspireDatasetMonitoring" });
            //routes.MapRoute("NewEPSGParent", "epsg/{parentRegister}/{registerowner}/{registername}/ny", new { controller = "EPSGs", action = "Create" });

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Registers", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
