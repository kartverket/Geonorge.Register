using Kartverket.Register.Formatter;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi.BasicAuth;

namespace Kartverket.Register
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
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
