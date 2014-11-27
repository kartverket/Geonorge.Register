using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Kartverket.Register.Models;
using Kartverket.Register.Services;

namespace Kartverket.Register
{
    public static class DependencyConfig
    {
        public static void Configure(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterType<RegisterDbContext>().InstancePerRequest().AsSelf();
            //builder.RegisterType<OrganizationsService>().As<IOrganizationService>();

            var container = builder.Build();

            // dependency resolver for MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // dependency resolver for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}