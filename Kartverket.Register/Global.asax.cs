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
using System.Xml.Serialization;
using Kartverket.Register.Models.Translations;
using System.Collections.Specialized;
using Kartverket.Register.Helpers;

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
            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.SetSerializer<Eu.Europa.Ec.Jrc.Inspire.Monitoring>(new XmlSerializer(typeof(Eu.Europa.Ec.Jrc.Inspire.Monitoring)));
            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(new UriPathExtensionMapping("skos", "application/rdf+xml"));
            //GlobalConfiguration.Configuration.Formatters.Add(new SKOSFormatter());
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);


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
            ValidateReturnUrl(Context.Request.QueryString);

            var cookie = Context.Request.Cookies["_culture"];

            var lang = Context.Request.QueryString["lang"];
            if (!string.IsNullOrEmpty(lang))
                cookie = null;

            if (cookie == null)
            {
                var cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                    Request.UserLanguages[0] : null;

                if (!string.IsNullOrEmpty(lang))
                    cultureName = lang;

                cultureName = CultureHelper.GetImplementedCulture(cultureName);
                if (CultureHelper.IsNorwegian(cultureName))
                    cookie = new HttpCookie("_culture", Culture.NorwegianCode);
                else
                    cookie = new HttpCookie("_culture", Culture.EnglishCode);

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

        void ValidateReturnUrl(NameValueCollection queryString)
        {
            if (queryString != null)
            {
                var returnUrl = queryString.Get("returnUrl");
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = returnUrl.Replace("http://", "");
                    returnUrl = returnUrl.Replace("https://", "");

                    if (!returnUrl.StartsWith(Request.Url.Host))
                        HttpContext.Current.Response.StatusCode = 400;
                }
            }
        }
    }
}
