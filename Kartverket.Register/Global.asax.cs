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

namespace Kartverket.Register
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RegisterDbContext, Migrations.Configuration>());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DependencyConfig.Configure(new ContainerBuilder());

            GlobalConfiguration.Configuration.Formatters.Add(new SyndicationFeedFormatter());
            //Database.SetInitializer<RegisterDbContext>(new RegisterInitializer());

            log4net.Config.XmlConfigurator.Configure();

            Startup.Init<RegisterIndexDoc>("http://localhost:8983/solr/register");

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
    }
}
