using Kartverket.Register.Formatter;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using Kartverket.Register.Controllers;
using WebApi.BasicAuth;

namespace Kartverket.Register
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Search
            config.Routes.MapHttpRoute("ApiSearch", "api/Search", new { controller = "ApiSearch", action = "Get" });

            // Organization api
            config.Routes.MapHttpRoute("GetOrganizationByName", "api/organisasjon/navn/{name}", new { controller = "OrganizationsApi", action = "GetOrganizationByName" });
            config.Routes.MapHttpRoute("GetOrganizationTranslatedByName", "api/organisasjon/navn/{name}/{culture}", new { controller = "OrganizationsApi", action = "GetOrganizationTranslatedByName" });
            config.Routes.MapHttpRoute("GetOrganizationByNumber", "api/organisasjon/orgnr/{number}", new { controller = "OrganizationsApi", action = "GetOrganizationByNumber" });
            config.Routes.MapHttpRoute("GetOrganizationsV2", "api/v2/organisasjoner/kommuner", new { controller = "OrganizationsApi", action = "GetOrganizationsV2" });
            config.Routes.MapHttpRoute("GetOrganizationByNameV2", "api/v2/organisasjon/navn/{name}", new { controller = "OrganizationsApi", action = "GetOrganizationByNameV2" });
            config.Routes.MapHttpRoute("GetOrganizationByNumberV2", "api/v2/organisasjon/orgnr/{number}", new { controller = "OrganizationsApi", action = "GetOrganizationByNumberV2" });

            config.Routes.MapHttpRoute("GetRegisterByIdExt", "api/kodelister/{systemid}.{ext}", new { controller = "ApiRoot", action = "GetRegisterById" });
            config.Routes.MapHttpRoute("GetRegisterById", "api/kodelister/{systemid}", new { controller = "ApiRoot", action = "GetRegisterById" });
            config.Routes.MapHttpRoute("GetRegister", "api/ApiRoot", new { controller = "ApiRoot", action = "GetRegisterById" });


            // Status reports
            config.Routes.MapHttpRoute("GetStatusReportsExt", "api/{registerName}/report.{ext}", new { controller = "ApiRoot", action = "StatusReports" });
            config.Routes.MapHttpRoute("GetStatusReports", "api/{registerName}/report", new { controller = "ApiRoot", action = "StatusReports" });
            config.Routes.MapHttpRoute("GetStatusReportsOldExt", "api/register/{registerName}/report.{ext}", new { controller = "ApiRoot", action = "StatusReports" });
            config.Routes.MapHttpRoute("GetStatusReportsOld", "api/register/{registerName}/report", new { controller = "ApiRoot", action = "StatusReports" });

            config.Routes.MapHttpRoute("ReportPost", "api/Report", new { controller = "Report", action = "Post" });

            config.MapHttpAttributeRoutes();

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            
            config.Routes.MapHttpRoute(
                   name: "SearchApi",
                   routeTemplate: "api/search/{search}",
                   defaults: new { controller = "ApiSearch", search = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            config.Formatters.Add(new SyndicationFeedFormatter());
            config.Formatters.Add(new CsvFormatter());
            config.Formatters.Add(new SKOSFormatter());
            config.Formatters.Add(new GMLFormatter());
            config.Formatters.Add(new XMLFormatter());

            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new UriPathExtensionMapping("json", "application/json"));
            config.Formatters.JsonFormatter.SerializerSettings.Re‌ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            //config.Formatters.XmlFormatter.UseXmlSerializer = true;

            config.EnableBasicAuth();

        }
    }
}
