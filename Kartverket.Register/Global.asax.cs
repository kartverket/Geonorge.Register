using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using System.Data.Entity;
using Kartverket.Register.Models;
using Kartverket.Register.Formatter;
using System;
using System.Web;
using SolrNet;
using System.Web.Configuration;
using System.Net.Http.Formatting;
using System.Globalization;
using System.Threading;
using Kartverket.Register.Models.Translations;

namespace Kartverket.Register
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start(){
            MvcHandler.DisableMvcResponseHeader = true;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RegisterDbContext, Migrations.Configuration>());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DependencyConfig.Configure(new ContainerBuilder());

            GlobalConfiguration.Configuration.Formatters.Add(new SyndicationFeedFormatter());
            //Database.SetInitializer<RegisterDbContext>(new RegisterInitializer());

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("json", "true", "application/json"));
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new UriPathExtensionMapping("json", "application/json"));
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(new UriPathExtensionMapping("xml", "application/xml"));
            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(new UriPathExtensionMapping("skos", "application/rdf+xml"));
            //GlobalConfiguration.Configuration.Formatters.Add(new SKOSFormatter());



            log4net.Config.XmlConfigurator.Configure();

            Startup.Init<RegisterIndexDoc>(WebConfigurationManager.AppSettings["IndexServerPath"]);

        }

        protected void Session_Start()
        {
            Session["role"] = "guest";
            Session["user"] = "guest";
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();

            Log.Error("App_Error", ex);
        }

        protected void Application_BeginRequest()
        {
            var cookie = Context.Request.Cookies["_culture"];
            if (cookie == null)
            {
                cookie = new HttpCookie("_culture", Culture.NorwegianCode);
                if (!Request.IsLocal)
                    cookie.Domain = ".geonorge.no";
                cookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                var culture = new CultureInfo(cookie.Value);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }
    }
}
