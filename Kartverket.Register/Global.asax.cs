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

namespace Kartverket.Register
{
    public class MvcApplication : System.Web.HttpApplication
    {
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
        }

        protected void Session_Start()
        {
            setAccessRole();
        }


        private string HasAccessToRegister()
        {
            string role = GetSecurityClaim("role");

            bool isAdmin = !string.IsNullOrWhiteSpace(role) && role.Equals("nd.metadata_admin");
            bool isEditor = !string.IsNullOrWhiteSpace(role) && role.Equals("nd.metadata"); //nd.metadata_editor

            if (isAdmin)
            {
                return "admin";
            }
            else if (isEditor)
            {
                return "editor";
            }
            else
            {
                return "guest";
            }
        }

        private string GetSecurityClaim(string type)
        {
            string result = null;
            foreach (var claim in System.Security.Claims.ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result = claim.Value;
                    break;
                }
            }

            // bad hack, must fix BAAT
            if (!string.IsNullOrWhiteSpace(result) && type.Equals("organization") && result.Equals("Statens kartverk"))
            {
                result = "Kartverket";
            }

            return result;
        }

        private void setAccessRole()
        {
            string organization = GetSecurityClaim("organization");

            string role = HasAccessToRegister();
            if (role == "admin")
            {
                Session["role"] = "admin";
                Session["user"] = organization;
            }
            else if (role == "editor")
            {
                Session["role"] = "editor";
                Session["user"] = organization;
            }
            else
            {
                Session["role"] = "guest";
                Session["user"] = "guest";
            }


        }

    }
}
