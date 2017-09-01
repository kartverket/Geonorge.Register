using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Search;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Report;
using Kartverket.Register.Services.Versioning;
using Kartverket.Register.Services.Notify;
using Kartverket.Register.Services.Translation;

namespace Kartverket.Register
{
    public static class DependencyConfig
    {
        public static void Configure(ContainerBuilder builder)
        {
            // in app
            builder.RegisterType<SolrRegisterIndexer>().As<RegisterIndexer>();
            builder.RegisterType<SolrIndexer>().As<Indexer>();
            builder.RegisterType<SolrIndexDocumentCreator>().As<IndexDocumentCreator>();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterType<RegisterDbContext>().InstancePerRequest().AsSelf();
            builder.RegisterType<OrganizationsService>().As<IOrganizationService>();
            builder.RegisterType<SearchService>().As<ISearchService>();
            builder.RegisterType<SearchIndexService>().As<ISearchIndexService>();
            builder.RegisterType<RegisterService>().As<IRegisterService>();
            builder.RegisterType<RegisterItemService>().As<IRegisterItemService>();
            builder.RegisterType<VersioningService>().As<IVersioningService>();
            builder.RegisterType<DatasetService>().As<IDatasetService>();
            builder.RegisterType<AccessControlService>().As<IAccessControlService>();
            builder.RegisterType<DokReportService>().As<IDokReportService>();
            builder.RegisterType<InspireDatasetService>().As<IInspireDatasetService>();

            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterType<TranslationService>().As<ITranslationService>();

            var container = builder.Build();

            // dependency resolver for MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // dependency resolver for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }
    }
}