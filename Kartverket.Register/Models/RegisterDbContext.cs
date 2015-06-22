using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Kartverket.Register.Models
{
    public class RegisterDbContext : DbContext
    {
        public RegisterDbContext() : base("RegisterDbContext") {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RegisterDbContext, Kartverket.Register.Migrations.Configuration>("RegisterDbContext"));
        }

        // marking DbSet with virtual makes it testable
        public virtual DbSet<Version> Versions { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
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
        //public virtual DbSet<InspireRequirement> inspireRequirements { get; set; }
        //public virtual DbSet<NationalRequirement> NationalRequirements { get; set; }
        //public virtual DbSet<NationalSeasRequirement> NationalSeasRequirements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kartverket.DOK.Models.DokDataset>().HasRequired(d => d.ThemeGroup).WithMany().WillCascadeOnDelete(true);
        }

        public override int SaveChanges()
        {
            int result = 0;

            var entityList = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified);
            foreach (var entity in entityList)
            {
                var reg = entity.Entity as Register;
                var regItem = entity.Entity as RegisterItem;

                if (reg != null) {
                    result = Save();
                    Index(reg.systemId);
                }
                else if (regItem != null) 
                {
                    result = Save();
                    Index(regItem.systemId);
                }
                else { result = Save(); }
            }

            return result;
        }


        void Index(System.Guid systemID)
        {
            string id = systemID.ToString();
            string url = "http://" + System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port + "/IndexSingle/" + id;
            System.Net.WebClient c = new System.Net.WebClient();
            try 
            { 
                var data = c.DownloadString(url);
            }
            catch (System.Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        int Save() {
            return base.SaveChanges();
        }
    }
}