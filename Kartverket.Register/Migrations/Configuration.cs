namespace Kartverket.Register.Migrations
{
    using Kartverket.Register.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Kartverket.Register.Models.RegisterDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Kartverket.Register.Models.RegisterDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            context.Statuses.AddOrUpdate(
              new Status { value = "Submitted", description = "Submitted" },
              new Status { value = "NotAccepted", description = "NotAccepted" },
              new Status { value = "Accepted", description = "Accepted" },
              new Status { value = "Valid", description = "Valid" },
              new Status { value = "Experimental", description = "Experimental" },
              new Status { value = "Deprecated", description = "Deprecated" },
              new Status { value = "Superseded", description = "Superseded" },
              new Status { value = "Retired", description = "Retired" }

            );

            context.DOKThemes.AddOrUpdate(
              new DOKTheme { value = "Samfunnssikkerhet", description = "Samfunnssikkerhet" },
              new DOKTheme { value = "Forurensning", description = "Forurensning" },
              new DOKTheme { value = "Friluftsliv", description = "Friluftsliv" },
              new DOKTheme { value = "Landskap", description = "Landskap" },
              new DOKTheme { value = "Kulturminner", description = "Kulturminner" },
              new DOKTheme { value = "Landbruk", description = "Landbruk" },
              new DOKTheme { value = "Energi", description = "Energi" },
              new DOKTheme { value = "Geologi", description = "Geologi" },
              new DOKTheme { value = "Kyst/Fiskeri", description = "Kyst/Fiskeri" },
              new DOKTheme { value = "Samferdsel", description = "Samferdsel" },
              new DOKTheme { value = "Basis geodata", description = "Basis geodata" }

            );
            
        }
    }
}
