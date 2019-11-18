using Autofac;
using Geonorge.AuthLib.NetFull;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Kartverket.Register.Startup))]

namespace Kartverket.Register
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Use Autofac as an Owin middleware
            var container = DependencyConfig.Configure(new ContainerBuilder());
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();  // requires Autofac.Mvc5.Owin nuget package installed
            
            app.UseGeonorgeAuthentication();
        }
       
    }
}