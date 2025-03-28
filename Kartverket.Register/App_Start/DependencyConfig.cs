﻿using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Integration.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Search;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Report;
using Kartverket.Register.Services.Versioning;
using Kartverket.Register.Services.Notify;
using Kartverket.Register.Services.Translation;
using Kartverket.Geonorge.Utilities.Organization;
using Kartverket.Geonorge.Utilities.LogEntry;
using System.Collections.Generic;
using System.Web.Configuration;
using Autofac.Integration.WebApi;
using Geonorge.AuthLib.NetFull;

namespace Kartverket.Register
{
    public static class DependencyConfig
    {
        public static IContainer Configure(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterModule<GeonorgeAuthenticationModule>();

            ConfigureAppDependencies(builder);

            var container = builder.Build();

            // dependency resolver for MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // dependency resolver for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return container;
        }

        private static void ConfigureAppDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<SolrRegisterIndexer>().As<RegisterIndexer>();
            builder.RegisterType<SolrIndexer>().As<Indexer>();
            builder.RegisterType<SolrIndexDocumentCreator>().As<IndexDocumentCreator>();

            builder.RegisterType<RegisterDbContext>().InstancePerRequest().AsSelf();
            builder.RegisterType<OrganizationsService>().As<Services.IOrganizationService>();
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
            builder.RegisterType<DatasetDeliveryService>().As<IDatasetDeliveryService>();
            builder.RegisterType<GeodatalovDatasetService>().As<IGeodatalovDatasetService>();
            builder.RegisterType<MareanoDatasetService>().As<IMareanoDatasetService>();
            builder.RegisterType<FairDatasetService>().As<IFairDatasetService>();
            builder.RegisterType<FairService>().As<IFairService>();
            builder.RegisterType<DocumentService>().As<IDocumentService>();
            builder.RegisterType<InspireMonitoringService>().As<IInspireMonitoringService>();
            builder.RegisterType<InspireMonitoring>().As<IInspireMonitoring>();
            builder.RegisterType<SynchronizationService>().As<ISynchronizationService>();
            builder.RegisterType<StatusReportService>().As<IStatusReportService>();

            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
            builder.RegisterType<LogEntryService>().As<ILogEntryService>().WithParameters(new List<Parameter>
            {
                new NamedParameter("logUrl", WebConfigurationManager.AppSettings["LogApi"]),
                new NamedParameter("apiKey", WebConfigurationManager.AppSettings["LogApiKey"]),
                new AutowiringParameter()
            });
        }
    }
}