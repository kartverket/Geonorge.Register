using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Autofac;
using Geonorge.AuthLib.NetFull;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Kartverket.Register.Startup))]

namespace Kartverket.Register
{
    public class Startup
    {
        private static readonly Regex _staticFileRegex = new Regex(@"^(\/content\/|\/scripts\/|\/dist\/|\/fonts\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void Configuration(IAppBuilder application)
        {
            var container = DependencyConfig.Configure(new ContainerBuilder());

            application.Use((context, next) => {
                context.Request.Scheme = "https";
                return next();
            });

            application.UseAutofacMiddleware(container);
            application.UseAutofacMvc();
            application.UseGeonorgeAuthentication();

            application.Use(async (context, next) =>
            {
                if (!IsStaticFile(context.Request.Path.ToString()))
                {
                    context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                    context.Response.Headers.Append("Pragma", "no-cache");
                }

                await next();
            });

        }

        private static bool IsStaticFile(string path)
        {
            return _staticFileRegex.IsMatch(path);
        }
    }
}