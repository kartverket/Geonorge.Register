using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace Kartverket.Register.Helpers
{
    public class CustomDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, System.Web.Http.Description.IApiExplorer apiExplorer)
        {
            //make operations alphabetic
            var paths = swaggerDoc.paths.OrderBy(e => e.Key).ToList();
            swaggerDoc.paths = paths.ToDictionary(e => e.Key, e => e.Value);

            //controller comments do not get added to swagger docs. This is how to add them.
            RemoveControllerDescriptions(swaggerDoc, apiExplorer);

        }

        private static void RemoveControllerDescriptions(SwaggerDocument swaggerDoc, System.Web.Http.Description.IApiExplorer apiExplorer)
        {
            List<ApiDescription> removeDescriptions = new List<ApiDescription>();

            foreach(var api in apiExplorer.ApiDescriptions)
            {
                if(api.ID.Contains("POSTapi/AlertApi"))
                    removeDescriptions.Add(api);

                if (api.ID.Contains("PUTapi/AlertApi"))
                    removeDescriptions.Add(api);

                if (api.ID.Contains("GETapi/AlertApi") || api.ID == "GETapi/ApiSearch" || api.ID == "GETapi/OrganizationsApi" || api.ID.Contains("ApiRoot")
                    || api.ID.Contains("report") || api.ID.Contains(".{ext}"))
                    removeDescriptions.Add(api);
            }

            foreach (var description in removeDescriptions) { 
                apiExplorer.ApiDescriptions.Remove(description);
                var descriptionId = description.ID.Replace("GET", "/");
                swaggerDoc.paths.Remove(descriptionId);
            }

        }
        private static string GetXmlCommentsPath()
        {
            return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/XmlDocument.xml");
        }
    }
}