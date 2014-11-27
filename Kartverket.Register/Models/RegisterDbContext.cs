using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Kartverket.Register.Models
{
    public class RegisterDbContext : DbContext
    {
        public RegisterDbContext() : base("RegisterDbContext") { }

        // marking DbSet with virtual makes it testable
        //public virtual DbSet<Version> Versions { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<DOKTheme> DOKThemes { get; set; }
        public virtual DbSet<Register> Registers { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<RegisterItem> RegisterItems { get; set; }
        public virtual DbSet<CodelistValue> CodelistValues { get; set; }
        public virtual DbSet<Dataset> Datasets { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<EPSG> EPSGs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //TPT 
            //modelBuilder.Entity<VersionedThing>().ToTable("VersionedThing");
            //modelBuilder.Entity<Register>().ToTable("Register");
            //modelBuilder.Entity<RegisterItem>().ToTable("RegisterItem");
            //modelBuilder.Entity<Organization>().ToTable("Organization");
            //modelBuilder.Entity<CodelistValue>().ToTable("CodelistValue");
            //modelBuilder.Entity<Dataset>().ToTable("Dataset");
            //modelBuilder.Entity<Document>().ToTable("Document");
            //modelBuilder.Entity<EPSG>().ToTable("EPSG");

            //TPC aproach - not allowed associations

           // modelBuilder.Entity<VersionedThing>()
           // .Property(c => c.systemId)
           // .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

           // modelBuilder.Entity<RegisterItem>()
           //.Property(c => c.systemId)
           //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

           // modelBuilder.Entity<Register>().Map(m =>
           // {
           //     m.MapInheritedProperties();
           //     m.ToTable("Register");
           // });

           // modelBuilder.Entity<Organization>().Map(m =>
           // {
           //     m.MapInheritedProperties();
           //     m.ToTable("Organization");
           // });

           // modelBuilder.Entity<CodelistValue>().Map(m =>
           // {
           //     m.MapInheritedProperties();
           //     m.ToTable("CodelistValue");
           // });

           // modelBuilder.Entity<Dataset>().Map(m =>
           // {
           //     m.MapInheritedProperties();
           //     m.ToTable("Dataset");
           // });
           // modelBuilder.Entity<Document>().Map(m =>
           // {
           //     m.MapInheritedProperties();
           //     m.ToTable("Document");
           // });

           // modelBuilder.Entity<EPSG>().Map(m =>
           // {
           //     m.MapInheritedProperties();
           //     m.ToTable("EPSG");
           // });

            base.OnModelCreating(modelBuilder);
        }
    }
}