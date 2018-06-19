
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using Kartverket.Register.Services;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static Kartverket.Register.Migrations.Configuration;
using Kartverket.Register.Models.Translations;
using System.Data.Entity.Infrastructure;
using Kartverket.Geonorge.Utilities.LogEntry;
using System.Web.Configuration;
using System.Security.Claims;

namespace Kartverket.Register.Models
{
    public class RegisterDbContext : DbContext
    {
        private ILogEntryService _logEntryService;

        public RegisterDbContext() : base("RegisterDbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RegisterDbContext, Kartverket.Register.Migrations.Configuration>("RegisterDbContext"));
        }

        // marking DbSet with virtual makes it testable
        public virtual DbSet<Version> Versions { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<DokStatus> DokStatuses { get; set; }
        public virtual DbSet<DokDeliveryStatus> DokDeliveryStatuses { get; set; }
        public virtual DbSet<accessType> AccessTypes { get; set; }
        public virtual DbSet<DOKTheme> DOKThemes { get; set; }
        public virtual DbSet<Register> Registers { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<RegisterItem> RegisterItems { get; set; }
        public virtual DbSet<CodelistValue> CodelistValues { get; set; }
        public virtual DbSet<Dataset> Datasets { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<NameSpace> NameSpases { get; set; }
        public virtual DbSet<EPSG> EPSGs { get; set; }
        public virtual DbSet<Requirement> requirements { get; set; }
        public virtual DbSet<Dimension> Dimensions { get; set; }
        public DbSet<Kartverket.DOK.Models.DokDataset> DokDatasets { get; set; }
        public DbSet<Kartverket.DOK.Models.ThemeGroup> ThemeGroup { get; set; }
        public virtual DbSet<Sorting> Sorting { get; set; }
        public virtual DbSet<ContainedItemClass> ContainedItemClass { get; set; }
        public virtual DbSet<CoverageDataset> CoverageDatasets { get; set; }
        public virtual DbSet<ServiceAlert> ServiceAlerts { get; set; }
        public virtual DbSet<InspireDataset> InspireDatasets { get; set; }
        public virtual DbSet<GeodatalovDataset> GeodatalovDatasets { get; set; }
        public virtual DbSet<DatasetDelivery> DatasetDeliveries { get; set; }
        public virtual DbSet<InspireDataService> InspireDataServices { get; set; }
        public virtual DbSet<InspireMonitoring> InspireMonitorings { get; set; }
        public virtual DbSet<Synchronize> Synchronizes { get; set; }
        public virtual DbSet<SyncLogEntry> SyncLogEntries { get; set; }
        public virtual DbSet<StatusReport> StatusReports { get; set; }
        //public virtual DbSet<StatusHistory> StatusHistories { get; set; }


        public ILogEntryService LogEntryService
        {
            get {
                if (_logEntryService == null)
                    _logEntryService = new LogEntryService(WebConfigurationManager.AppSettings["LogApi"], WebConfigurationManager.AppSettings["LogApiKey"], new Kartverket.Geonorge.Utilities.Organization.HttpClientFactory());

                return _logEntryService;
            }
            set { _logEntryService = value; }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DOK.Models.DokDataset>().HasRequired(d => d.ThemeGroup).WithMany().WillCascadeOnDelete(true);
            modelBuilder.Entity<Dataset>().HasMany(n => n.Coverage).WithOptional().WillCascadeOnDelete();
            modelBuilder.Configurations.Add(new RegisterTranslationConfiguration());
            modelBuilder.Configurations.Add(new CodelistValueTranslationConfiguration());
            modelBuilder.Configurations.Add(new EPSGTranslationConfiguration());
            modelBuilder.Configurations.Add(new OrganizationTranslationConfiguration());
            modelBuilder.Configurations.Add(new DatasetTranslationConfiguration());
            modelBuilder.Configurations.Add(new DocumentTranslationConfiguration());
            modelBuilder.Configurations.Add(new NamespaceTranslationConfiguration());
            modelBuilder.Configurations.Add(new ServiceAlertTranslationConfiguration());

            modelBuilder.Entity<InspireDataset>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("InspireDatasets");
            });

            modelBuilder.Entity<GeodatalovDataset>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("GeodatalovDatasets");
            });

            modelBuilder.Entity<InspireDataService>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("InspireDataServices");
            });

            modelBuilder.Entity<RegisterItemV2>()
                .Property(p => p.SystemId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        }

        public override int SaveChanges()
        {
            Database.Log = s => System.Diagnostics.Trace.WriteLine(s);

            int result = 0;

            var entityList = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified).ToList();

            for (int c = 0; c < entityList.Count(); c++)
            {
                string operation = "";
                string uuid = "";
                string title = "";
                string description = "";

                var entity = entityList[c];

                var reg = entity.Entity as Register;
                var regItem = entity.Entity as RegisterItem;

                if (reg != null)
                {
                    uuid = reg.systemId.ToString();
                    title = reg.name;
                    operation = entity.State.ToString();
                    if (operation == EntityState.Unchanged.ToString())
                        operation = Operation.Modified;
                    description = "Saved register: " + title;
                    result = Save();
                    new Task(() => { Index(reg.systemId); }).Start();
                }
                else if (regItem != null)
                {
                    if(regItem.register != null)
                    { 
                        regItem.register.modified = System.DateTime.Now;
                    }

                    uuid = regItem.systemId.ToString();
                    title = regItem.name;
                    if (regItem.register != null)
                        description = "Saved register item: " + title + " in register: " + regItem.register.name;
                    else
                        description = "Saved register item: " + title;

                    operation = entity.State.ToString();
                    if (operation == EntityState.Unchanged.ToString())
                        operation = Operation.Modified;

                    result = Save();
                    new Task(() => { Index(regItem.systemId); }).Start();
                }
                else { result = Save(); }

                if (!string.IsNullOrEmpty(uuid))
                    Task.Run(() => LogEntryService.AddLogEntry(new LogEntry { ElementId = uuid, Operation = operation, User = GetSecurityClaim("username"), Description = description }));
            }

            return result;
        }


        void Index(System.Guid systemID)
        {
            string id = systemID.ToString();
            var indexer = new SolrRegisterIndexer(new SolrIndexer(), new SolrIndexDocumentCreator());
            indexer.RunIndexingOn(id);

            //notify metadataeditor
            System.Net.WebClient c = new System.Net.WebClient();
            c.DownloadString(System.Web.Configuration.WebConfigurationManager.AppSettings["EditorUrl"] + "Metadata/FlushCache");
        }
        int Save()
        {
            return base.SaveChanges();
        }

        public string GetSecurityClaim(string type)
        {
            foreach (var claim in ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    return claim.Value;
                }
            }
            return null;
        }
    }
}