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

            Register produktspesifikasjon = new Register { systemId = Guid.Parse("8E726684-F216-4497-91BE-6AB2496A84D3"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Produktspesifikasjon", description = "Inneholder dokumenter for produktspesifikasjoner for kart- og geodata ", containedItemClass = "Document" };
            Register organisasjoner = new Register { systemId = Guid.Parse("FCB0685D-24EB-4156-9AC8-25FA30759094"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Organiasjoner", description = "Inneholder oversikt over organisasjoner og deres logo ", containedItemClass = "Organization" };

            context.Registers.AddOrUpdate(
              produktspesifikasjon,
              new Register { systemId = Guid.Parse("A42BC2B3-2314-4B7E-8007-71D9B10F2C04"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Produktark", description = "Inneholder organisasjoners produktark for kart og geodata", containedItemClass = "Document" },
              new Register { systemId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB689"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Kodeliste", description = "Inneholder kodelister", containedItemClass = "Register" },
              //new Register { systemId = Guid.Parse("9A82A6B6-0069-45A4-8CA8-FBB789434F9A"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Navnerom", description = "Inneholder godkjente navnerom", containedItemClass = "Document" },
              new Register { systemId = Guid.Parse("E43B65C6-452F-489D-A2E6-A5262E5740D8"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "GML applikasjoner", description = "Inneholder godkjente GML applikasjonsskjema", containedItemClass = "Document" },
              //new Register { systemId = Guid.Parse("B4BA9E24-3717-482B-B9F5-7E349194D502"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "SOSI def", description = "Inneholder godkjente SOSI definisjonsfiler ", containedItemClass = "Document" },
              new Register { systemId = Guid.Parse("8960B018-E6EC-4CF5-BDB2-0C2AE744B7E2"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Det offentlige kartgrunnlaget", description = "Inneholder oversikt over datasett som inngår i det offentlige kartgrunnlaget ", containedItemClass = "Dataset" },
              organisasjoner,
              new Register { systemId = Guid.Parse("37B9DC41-D868-4CBC-84F9-39557041FB2C"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "EPSG kode", description = "Inneholder oversikt over EPSG koder som benyttes i Norge Digitalt omtalt i rammeverksdokumentet ", containedItemClass = "EPSG" }
            );

            //context.Documents.AddOrUpdate(
              //  new Document { systemId = Guid.NewGuid(), register = produktspesifikasjon, dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Produktspesifikasjon for løsmasser", description = "Datagrunnlaget for tema jordarter er basert på innholdet i kvartærgeologiske kart (løsmassekart), som foreligger analogt i flere målestokker (fra 1:20.000 til 1:250.000), og digital kartlegging.",   }
                //);

            
            //context.Organizations.AddOrUpdate(
            //    new Organization { systemId = Guid.NewGuid(), register = organisasjoner, dateSubmitted = DateTime.Now, modified = DateTime.Now, number = "874783242", name = "Kystverket", logoFilename = "http://register.test.geonorge.no/data/organizations/874783242_kystverketlogo.jpg" },
            //    new Organization { systemId = Guid.NewGuid(), register = organisasjoner, dateSubmitted = DateTime.Now, modified = DateTime.Now, number = "970188290", name = "Norges geologiske undersøkelse", logoFilename = "http://register.test.geonorge.no/data/organizations/970188290_nguLogo.png" },
            //    new Organization { systemId = Guid.NewGuid(), register = organisasjoner, dateSubmitted = DateTime.Now, modified = DateTime.Now, number = "874783242", name = "Kartverket", logoFilename = "http://register.test.geonorge.no/data/organizations/971040238_kartverket_logo.png" },
            //    new Organization { systemId = Guid.NewGuid(), register = organisasjoner, dateSubmitted = DateTime.Now, modified = DateTime.Now, number = "999601391", name = "Miljødirektoratet", logoFilename = "http://register.test.geonorge.no/data/organizations/999601391_miljodirlogo.JPG" }
            //);
            

        }
    }
}
