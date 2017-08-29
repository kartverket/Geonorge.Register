using System.ComponentModel.DataAnnotations.Schema;
using Kartverket.Register.Services;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static Kartverket.Register.Migrations.Configuration;
using Kartverket.Register.Models.Translations;

namespace Kartverket.Register.Models
{
    public class RegisterDbContext : DbContext
    {
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DOK.Models.DokDataset>().HasRequired(d => d.ThemeGroup).WithMany().WillCascadeOnDelete(true);
            modelBuilder.Entity<Dataset>().HasMany(n => n.Coverage).WithOptional().WillCascadeOnDelete();
            modelBuilder.Configurations.Add(new RegisterTranslationConfiguration());
            modelBuilder.Configurations.Add(new CodelistValueTranslationConfiguration());
            modelBuilder.Configurations.Add(new EPSGTranslationConfiguration());
        }

        public override int SaveChanges()
        {
            int result = 0;

            var entityList = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified).ToList();
            for (int c = 0; c < entityList.Count(); c++)
            {
                var entity = entityList[c];
                if(entity.State != EntityState.Unchanged)
                {
                    var reg = entity.Entity as Register;
                    var regItem = entity.Entity as RegisterItem;

                    if (reg != null)
                    {
                        result = Save();
                        new Task(() => { Index(reg.systemId); }).Start();
                    }
                    else if (regItem != null)
                    {
                        if(regItem.register != null)
                            regItem.register.modified = System.DateTime.Now;
                        result = Save();
                        new Task(() => { Index(regItem.systemId); }).Start();
                    }
                    else { result = Save(); }
                }

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
    }
}