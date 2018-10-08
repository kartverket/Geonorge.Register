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

            // Document
            routes.MapRoute("CreateDocumentSub", "dokument/{parentRegister}/{registerowner}/{registername}/ny", new { controller = "Documents", action = "Create" });
            routes.MapRoute("CreateDocument", "dokument/{registername}/ny", new { controller = "Documents", action = "Create" });
            routes.MapRoute("CreateNewVersionDocumentSub", "dokument/versjon/{parentRegister}/{parentRegisterOwner}/{registername}/{itemOwner}/{itemname}/ny", new { controller = "Documents", action = "CreateNewVersion" });
            routes.MapRoute("CreateNewVersionDocument", "dokument/versjon/{registername}/{itemOwner}/{itemname}/ny", new { controller = "Documents", action = "CreateNewVersion" });
            routes.MapRoute("EditDocumentSub", "dokument/{parentregister}/{registerowner}/{registername}/{itemowner}/{documentname}/rediger", new { controller = "Documents", action = "Edit" });
            routes.MapRoute("EditDocument", "dokument/{registername}/{itemowner}/{documentname}/rediger", new { controller = "Documents", action = "Edit" });
            routes.MapRoute("DeleteDocumentSub", "dokument/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{documentname}/slett", new { controller = "Documents", action = "Delete" });
            routes.MapRoute("DeleteDocument", "dokument/{registername}/{organization}/{documentname}/slett", new { controller = "Documents", action = "Delete" });

            // Geodatalov
            routes.MapRoute("CreateGeodatalovDatasetSub", "geodatalov/{parentregister}/{registerowner}/{registername}/ny", new { controller = "GeodatalovDatasets", action = "Create" });
            routes.MapRoute("CreateGeodatalovDataset", "geodatalov/{registername}/ny", new { controller = "GeodatalovDatasets", action = "Create" });
            routes.MapRoute("EditGeodatalovDatasetSub", "geodatalov/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger", new { controller = "GeodatalovDatasets", action = "Edit" });
            routes.MapRoute("EditGeodatalovDataset", "geodatalov/{registername}/{itemowner}/{itemname}/rediger", new { controller = "GeodatalovDatasets", action = "Edit" });
            routes.MapRoute("DeleteGeodatalovDatasetSub", "geodatalov/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{itemname}/slett", new { controller = "GeodatalovDatasets", action = "Delete" });
            routes.MapRoute("DeleteGeodatalovDataset", "geodatalov/{registername}/{itemowner}/{itemname}/slett", new { controller = "GeodatalovDatasets", action = "Delete" });

            // Inspire data service
            routes.MapRoute("EditInspireDataService", "inspire-data-service/{registername}/{itemowner}/{itemname}/rediger", new { controller = "InspireDataService", action = "Edit" });
            routes.MapRoute("DeleteInspireDataService", "inspire-data-service/{registername}/{itemowner}/{itemname}/slett", new { controller = "InspireDataService", action = "Delete" });

            // Inspire dataset
            routes.MapRoute("CreateInspireDatasetSub", "inspire/{parentregister}/{registerowner}/{registername}/ny", new { controller = "InspireDatasets", action = "Create" });
            routes.MapRoute("CreateInspireDataset", "inspire/{registername}/ny", new { controller = "InspireDatasets", action = "Create" });
            routes.MapRoute("EditInspireDatasetSub", "inspire/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger", new { controller = "InspireDatasets", action = "Edit" });
            routes.MapRoute("EditInspireDataset", "inspire/{registername}/{itemowner}/{itemname}/rediger", new { controller = "InspireDatasets", action = "Edit" });
            routes.MapRoute("DeleteInspireDataset", "inspire/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{itemname}/slett", new { controller = "InspireDatasets", action = "Delete" });
            routes.MapRoute("DeleteInspireDatasetSub", "inspire/{registername}/{itemowner}/{itemname}/slett", new { controller = "InspireDatasets", action = "Delete" });

            // Namespace 
            routes.MapRoute("CreateNamespaceSub", "navnerom/{parentRegister}/{registerowner}/{registername}/ny", new { controller = "NameSpaces", action = "Create" });
            routes.MapRoute("CreateNamespace", "navnerom/{registername}/ny", new { controller = "NameSpaces", action = "Create" });
            routes.MapRoute("EditNamespaceSub", "navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger", new { controller = "NameSpaces", action = "Edit" });
            routes.MapRoute("EditNamespace", "navnerom/{registername}/{itemowner}/{itemname}/rediger", new { controller = "NameSpaces", action = "Edit" });
            routes.MapRoute("DeleteNamespaceSub", "navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett", new { controller = "NameSpaces", action = "Delete" });
            routes.MapRoute("DeleteNamespace", "navnerom/{registername}/{itemowner}/{itemname}/slett", new { controller = "NameSpaces", action = "Delete" });



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
