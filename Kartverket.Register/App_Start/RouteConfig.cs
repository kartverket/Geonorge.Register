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

            // Register
            routes.MapRoute("RegisterCreate", "ny", new { controller = "Registers", action = "Create"});
            routes.MapRoute("RegisterEdit", "rediger/{registername}", new { controller = "Registers", action = "Edit"});
            routes.MapRoute("RegisterDelete", "slett/{registername}", new { controller = "Registers", action = "Delet"});
            routes.MapRoute("EditDokMunicipal", "dok/kommunalt/{municipalityCode}/rediger", new { controller = "Registers", action = "EditDokMunicipal" });

            // EPSG
            routes.MapRoute("CreateEPSGsub", "epsg/{parentRegister}/{registerowner}/{registername}/ny", new { controller = "EPSGs", action = "Create", registername = "registername" });
            routes.MapRoute("CreateEPSG", "epsg/{registername}/ny", new { controller = "EPSGs", action = "Create", registername = "registername" });
            routes.MapRoute("EditEPSGsub", "epsg/{parentRegister}/{registerowner}/{registername}/{itemowner}/{epsgname}/rediger", new { controller = "EPSGs", action = "Edit"});
            routes.MapRoute("EditEPSG", "epsg/{registername}/{organization}/{epsgname}/rediger", new { controller = "EPSGs", action = "Edit", registername = "registername" });
            routes.MapRoute("DeleteEPSGsub", "epsg/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{epsgname}/slett", new { controller = "EPSGs", action = "Delete"});
            routes.MapRoute("DeleteEPSG", "epsg/{registername}/{organization}/{epsgname}/slett", new { controller = "EPSGs", action = "Delete"});

            // CodelistValue
            routes.MapRoute("ImportCodelistValueSub", "kodeliste/{parentregister}/{registerowner}/{registername}/ny/import", new { controller = "CodelistValues", action = "Import" });
            routes.MapRoute("ImportCodelistValue", "kodeliste/{registername}/ny/import", new { controller = "CodelistValues", action = "Import" });
            routes.MapRoute("CreateCodelistValuesub", "kodeliste/{parentregister}/{registerowner}/{registername}/ny", new { controller = "CodelistValues", action = "Create"});
            routes.MapRoute("CreateCodelistValue", "kodeliste/{registername}/ny", new { controller = "CodelistValues", action = "Create"});
            routes.MapRoute("EditCodelistValuesub", "kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger", new { controller = "CodelistValues", action = "Edit"});
            routes.MapRoute("EditCodelistValue", "kodeliste/{registername}/{submitter}/{itemname}/rediger", new { controller = "CodelistValues", action = "Edit"});
            routes.MapRoute("DeleteCodelistValuesub", "kodeliste/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{itemname}/slett", new { controller = "CodelistValues", action = "Delete"});
            routes.MapRoute("DeleteCodelistValue", "kodeliste/{registername}/{organization}/{itemname}/slett", new { controller = "CodelistValues", action = "Delete"});

            // Datasett
            routes.MapRoute("CreateDatasetSub", "dataset/{parentRegister}/{registerowner}/{registername}/ny", new { controller = "Datasets", action = "Create" });
            routes.MapRoute("CreateDataset", "dataset/{registername}/ny", new { controller = "Datasets", action = "Create" });
            routes.MapRoute("CreateMunicipalDatasetSub", "dataset/{parentRegister}/{registerowner}/{registername}/{municipality}/ny", new { controller = "Datasets", action = "CreateMunicipalDataset" });
            routes.MapRoute("CreateMunicipalDataset", "dataset/{registername}/{municipality}/ny", new { controller = "Datasets", action = "CreateMunicipalDataset" });
            routes.MapRoute("EditDatasetSub", "dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger", new { controller = "Datasets", action = "Edit" });
            routes.MapRoute("EditDataset", "dataset/{registername}/{itemowner}/{itemname}/rediger", new { controller = "Datasets", action = "Edit" });
            routes.MapRoute("DeleteDatasetSub", "dataset/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{itemname}/slett", new { controller = "Datasets", action = "Delete" });
            routes.MapRoute("DeleteDataset", "dataset/{registername}/{itemowner}/{itemname}/slett", new { controller = "Datasets", action = "Delete" });







            routes.MapRoute("SignIn", "AuthServices/SignIn", new { controller = "AuthServices", action = "SignIn" });
            routes.MapRoute("LogOut", "AuthServices/LogOut", new { controller = "AuthServices", action = "LogOut" });
            routes.MapRoute("Acs", "AuthServices/Acs", new { controller = "AuthServices", action = "Acs" });
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
