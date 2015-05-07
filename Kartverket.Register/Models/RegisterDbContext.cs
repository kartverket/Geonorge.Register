using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

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
        public virtual DbSet<DOKTheme> DOKThemes { get; set; }
        public virtual DbSet<Register> Registers { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<RegisterItem> RegisterItems { get; set; }
        public virtual DbSet<CodelistValue> CodelistValues { get; set; }
        public virtual DbSet<Dataset> Datasets { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
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
    }
}