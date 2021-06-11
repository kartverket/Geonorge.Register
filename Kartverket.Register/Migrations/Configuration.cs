namespace Kartverket.Register.Migrations
{
    using Kartverket.Register.Helpers;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RegisterDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;

        }

        internal class RegisterTranslationConfiguration : EntityTypeConfiguration<Register>
        {

            public RegisterTranslationConfiguration()
            {
                HasMany(x => x.Translations).WithRequired().HasForeignKey(x => x.RegisterId);
            }

        }

        internal class CodelistValueTranslationConfiguration : EntityTypeConfiguration<CodelistValue>
        {

            public CodelistValueTranslationConfiguration()
            {
                HasMany(x => x.Translations).WithRequired().HasForeignKey(x => x.RegisterItemId);
            }

        }

        internal class EPSGTranslationConfiguration : EntityTypeConfiguration<EPSG>
        {

            public EPSGTranslationConfiguration()
            {
                HasMany(x => x.Translations).WithRequired().HasForeignKey(x => x.RegisterItemId);
            }

        }

        internal class OrganizationTranslationConfiguration : EntityTypeConfiguration<Organization>
        {

            public OrganizationTranslationConfiguration()
            {
                HasMany(x => x.Translations).WithRequired().HasForeignKey(x => x.RegisterItemId);
            }

        }

        internal class DatasetTranslationConfiguration : EntityTypeConfiguration<Dataset>
        {
            public DatasetTranslationConfiguration()
            {
                HasMany(x => x.Translations).WithRequired().HasForeignKey(x => x.RegisterItemId);
            }
        }

        internal class DocumentTranslationConfiguration : EntityTypeConfiguration<Document>
        {
            public DocumentTranslationConfiguration()
            {
                HasMany(x => x.Translations).WithRequired().HasForeignKey(x => x.RegisterItemId);
            }
        }

        internal class NamespaceTranslationConfiguration : EntityTypeConfiguration<NameSpace>
        {
            public NamespaceTranslationConfiguration()
            {
                HasMany(x => x.Translations).WithRequired().HasForeignKey(x => x.RegisterItemId);
            }
        }

        internal class AlertTranslationConfiguration : EntityTypeConfiguration<Alert>
        {
            public AlertTranslationConfiguration()
            {
                HasMany(x => x.Translations).WithRequired().HasForeignKey(x => x.RegisterItemId);
            }
        }

        protected override void Seed(RegisterDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //

            context.Statuses.AddOrUpdate(
                new Status { value = "Submitted", description = "Sendt inn", group = "suggested" },
                new Status { value = "Draft", description = "Utkast", group = "suggested" },
                new Status { value = "Valid", description = "Gyldig", group = "current" },
                new Status { value = "Sosi-valid", description = "SOSI godkjent", group = "current" },
                new Status { value = "Superseded", description = "Erstattet", group = "historical" },
                new Status { value = "Retired", description = "Utgått", group = "historical" }
            );

            context.DokStatuses.AddOrUpdate(
                new DokStatus { value = "Proposal", description = "Forslag" },
                new DokStatus { value = "Candidate", description = "Kandidat" },
                new DokStatus { value = "InProgress", description = "I prosess" },
                new DokStatus { value = "Accepted", description = "Godkjent" }
            );

            context.DokDeliveryStatuses.AddOrUpdate(
                new Models.DokDeliveryStatus { value = "deficient", description = "Ikke levert" },
                new Models.DokDeliveryStatus { value = "useable", description = "Brukbar" },
                new Models.DokDeliveryStatus { value = "good", description = "God" },
                new Models.DokDeliveryStatus { value = "notset", description = "Ikke angitt" }
            );

            context.FAIRDeliveryStatuses.AddOrUpdate(
                new Models.FAIRDeliveryStatus { value = "deficient", description = "Dårlig" },
                new Models.FAIRDeliveryStatus { value = "satisfactory", description = "Tilfredsstillende" },
                new Models.FAIRDeliveryStatus { value = "useable", description = "Bør forbedres" },
                new Models.FAIRDeliveryStatus { value = "good", description = "God" },
                new Models.FAIRDeliveryStatus { value = "notset", description = "Ikke angitt" }
            );

            context.AccessTypes.AddOrUpdate(
                new accessType { accessLevel = 1, description = "Only admin kan create, edit or delete" },
                new accessType { accessLevel = 2, description = "Editor can create, edit or delete their owne items" },
                new accessType { accessLevel = 4, description = "Municipalities can create, update and delete their own items" }
            );

            context.SaveChanges();

            context.requirements.AddOrUpdate(
                new Requirement { value = "Mandatory", description = "Påkrevd", sortOrder = 0 },
                new Requirement { value = "Conditional", description = "Betinget", sortOrder = 2 },
                new Requirement { value = "Recommended", description = "Anbefalt", sortOrder = 1 },
                new Requirement { value = "Optional", description = "Valgfritt", sortOrder = 3 },
                new Requirement { value = "Notset", description = "Ikke angitt", sortOrder = 4 }
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
              new DOKTheme { value = "Basis geodata", description = "Basis geodata" },
              new DOKTheme { value = "Natur", description = "Natur" }
            );

            context.Sorting.AddOrUpdate(
                new Sorting { value = "name_desc", description = "Navn å-a" },
                new Sorting { value = "submitter", description = "Innsender a-å" },
                new Sorting { value = "submitter_desc", description = "Innsender å-a" },
                new Sorting { value = "status", description = "Status a-å", },
                new Sorting { value = "status_desc", description = "Status å-a" },
                new Sorting { value = "dateSubmitted_desc", description = "Innsendt dato synkende" },
                new Sorting { value = "dateSubmitted", description = "Innsendt dato stigende" },
                new Sorting { value = "modified", description = "Endret dato stigende" },
                new Sorting { value = "modified_desc", description = "Endret dato synkende" },
                new Sorting { value = "dateAccepted", description = "Godkjent dato stigende" },
                new Sorting { value = "dateAccepted_desc", description = "Godkjent dato synkende" }
            );

            context.ContainedItemClass.AddOrUpdate(
                new ContainedItemClass { value = "Register", description = "Register" },
                new ContainedItemClass { value = "CodelistValue", description = "Kodeverdier" },
                new ContainedItemClass { value = "Dataset", description = "Datasett" },
                new ContainedItemClass { value = "Document", description = "Dokumenter" },
                new ContainedItemClass { value = "EPSG", description = "EPSG koder" },
                new ContainedItemClass { value = "Organization", description = "Organisasjoner" },
                new ContainedItemClass { value = "NameSpace", description = "Navnerom" },
                new ContainedItemClass { value = "Alert", description = "Varsler" }
            );

            context.Dimensions.AddOrUpdate(
                new Models.Dimension { value = "horizontal", description = "Horisontalt" },
                new Models.Dimension { value = "vertical", description = "Vertikalt" },
                new Models.Dimension { value = "compound", description = "Sammensatt" }
            );

            context.Tags.AddOrUpdate(
                new Models.Tag { value = "SATREF", description = "SATREF" },
                new Models.Tag { value = "Matrikkel", description = "Matrikkel" },
                new Models.Tag { value = "Vannstand", description = "Vannstand" },
                new Models.Tag { value = "FKB", description = "FKB" },
                new Models.Tag { value = "Norge i bilder", description = "Norge i bilder" },
                new Models.Tag { value = "Høydedata", description = "Høydedata" },
                new Models.Tag { value = "CPOS", description = "CPOS" },
                new Models.Tag { value = "DPOS", description = "DPOS" },
                new Models.Tag { value = "EFS", description = "EFS" }
            );

            context.Departments.AddOrUpdate(
                new Models.Department { value = "Posisjonstjenester", description = "Posisjonstjenester" },
                new Models.Department { value = "Eiendom", description = "Eiendom" },
                new Models.Department { value = "Til sjøs", description = "Til sjøs" },
                new Models.Department { value = "Data, API, nettsider", description = "Data, API, nettsider" }
            );

            context.Stations.AddOrUpdate(

                //satref: http://satref.geodesi.no/
                new Models.Station { value = "Adamselv", description = "Adamselv", group = "SATREF" },
                new Models.Station { value = "Åkrahamn", description = "Åkrahamn", group = "SATREF" },
                new Models.Station { value = "Ålesund", description = "Ålesund", group = "SATREF" },
                new Models.Station { value = "Alta", description = "Alta", group = "SATREF" },
                new Models.Station { value = "Alteidet", description = "Alteidet", group = "SATREF" },
                new Models.Station { value = "Alvdal", description = "Alvdal", group = "SATREF" },
                new Models.Station { value = "Andenes", description = "Andenes", group = "SATREF" },
                new Models.Station { value = "Andøya", description = "Andøya", group = "SATREF" },
                new Models.Station { value = "Årdal", description = "Årdal", group = "SATREF" },
                new Models.Station { value = "Arendal", description = "Arendal", group = "SATREF" },
                new Models.Station { value = "Årnes", description = "Årnes", group = "SATREF" },
                new Models.Station { value = "Ås", description = "Ås", group = "SATREF" },
                new Models.Station { value = "Åsane", description = "Åsane", group = "SATREF" },
                new Models.Station { value = "Asker", description = "Asker", group = "SATREF" },
                new Models.Station { value = "Askvoll", description = "Askvoll", group = "SATREF" },
                new Models.Station { value = "Atna", description = "Atna", group = "SATREF" },
                new Models.Station { value = "Aurdal", description = "Aurdal", group = "SATREF" },
                new Models.Station { value = "Austevoll", description = "Austevoll", group = "SATREF" },
                new Models.Station { value = "Balestrand", description = "Balestrand", group = "SATREF" },
                new Models.Station { value = "Balsfjord", description = "Balsfjord", group = "SATREF" },
                new Models.Station { value = "Bardu", description = "Bardu", group = "SATREF" },
                new Models.Station { value = "Båtsfjord", description = "Båtsfjord", group = "SATREF" },
                new Models.Station { value = "Bergen", description = "Bergen", group = "SATREF" },
                new Models.Station { value = "Bergen havn", description = "Bergen havn", group = "SATREF" },
                new Models.Station { value = "Berkåk", description = "Berkåk", group = "SATREF" },
                new Models.Station { value = "Berlevåg", description = "Berlevåg", group = "SATREF" },
                new Models.Station { value = "Birkenes", description = "Birkenes", group = "SATREF" },
                new Models.Station { value = "Bjarkøy", description = "Bjarkøy", group = "SATREF" },
                new Models.Station { value = "Bjorli", description = "Bjorli", group = "SATREF" },
                new Models.Station { value = "Bjugn", description = "Bjugn", group = "SATREF" },
                new Models.Station { value = "Bjørnstad", description = "Bjørnstad", group = "SATREF" },
                new Models.Station { value = "Bleikvassli", description = "Bleikvassli", group = "SATREF" },
                new Models.Station { value = "Bodø", description = "Bodø", group = "SATREF" },
                new Models.Station { value = "Breivikbotn", description = "Breivikbotn", group = "SATREF" },
                new Models.Station { value = "Brønnøysund", description = "Brønnøysund", group = "SATREF" },
                new Models.Station { value = "Bygland", description = "Bygland", group = "SATREF" },
                new Models.Station { value = "Bø", description = "Bø", group = "SATREF" },
                new Models.Station { value = "Bømlo", description = "Bømlo", group = "SATREF" },
                new Models.Station { value = "Dagali", description = "Dagali", group = "SATREF" },
                new Models.Station { value = "Dokka", description = "Dokka", group = "SATREF" },
                new Models.Station { value = "Dombås", description = "Dombås", group = "SATREF" },
                new Models.Station { value = "Drevsjø", description = "Drevsjø", group = "SATREF" },
                new Models.Station { value = "Dønna", description = "Dønna", group = "SATREF" },
                new Models.Station { value = "Eidsvoll", description = "Eidsvoll", group = "SATREF" },
                new Models.Station { value = "Elverum", description = "Elverum", group = "SATREF" },
                new Models.Station { value = "Etne", description = "Etne", group = "SATREF" },
                new Models.Station { value = "Evenstad", description = "Evenstad", group = "SATREF" },
                new Models.Station { value = "Fauske", description = "Fauske", group = "SATREF" },
                new Models.Station { value = "Fåvang", description = "Fåvang", group = "SATREF" },
                new Models.Station { value = "Fet", description = "Fet", group = "SATREF" },
                new Models.Station { value = "Finnsnes", description = "Finnsnes", group = "SATREF" },
                new Models.Station { value = "Finnøy", description = "Finnøy", group = "SATREF" },
                new Models.Station { value = "Flisa", description = "Flisa", group = "SATREF" },
                new Models.Station { value = "Flornes", description = "Flornes", group = "SATREF" },
                new Models.Station { value = "Florø", description = "Florø", group = "SATREF" },
                new Models.Station { value = "Folldal", description = "Folldal", group = "SATREF" },
                new Models.Station { value = "Fredrikstad", description = "Fredrikstad", group = "SATREF" },
                new Models.Station { value = "Frøya", description = "Frøya", group = "SATREF" },
                new Models.Station { value = "Følling", description = "Følling", group = "SATREF" },
                new Models.Station { value = "Førde", description = "Førde", group = "SATREF" },
                new Models.Station { value = "Gjerde", description = "Gjerde", group = "SATREF" },
                new Models.Station { value = "Gjerdrum", description = "Gjerdrum", group = "SATREF" },
                new Models.Station { value = "Gjesdal", description = "Gjesdal", group = "SATREF" },
                new Models.Station { value = "Gjøra", description = "Gjøra", group = "SATREF" },
                new Models.Station { value = "Glesvær", description = "Glesvær", group = "SATREF" },
                new Models.Station { value = "Gloppen", description = "Gloppen", group = "SATREF" },
                new Models.Station { value = "Gol", description = "Gol", group = "SATREF" },
                new Models.Station { value = "Gran", description = "Gran", group = "SATREF" },
                new Models.Station { value = "Grong", description = "Grong", group = "SATREF" },
                new Models.Station { value = "Haltdalen", description = "Haltdalen", group = "SATREF" },
                new Models.Station { value = "Hamar", description = "Hamar", group = "SATREF" },
                new Models.Station { value = "Hamarøy", description = "Hamarøy", group = "SATREF" },
                new Models.Station { value = "Hammerfest", description = "Hammerfest", group = "SATREF" },
                new Models.Station { value = "Hansnes", description = "Hansnes", group = "SATREF" },
                new Models.Station { value = "Hardbakke", description = "Hardbakke", group = "SATREF" },
                new Models.Station { value = "Haukeli", description = "Haukeli", group = "SATREF" },
                new Models.Station { value = "Havøysund", description = "Havøysund", group = "SATREF" },
                new Models.Station { value = "Hedalen", description = "Hedalen", group = "SATREF" },
                new Models.Station { value = "Heggenes", description = "Heggenes", group = "SATREF" },
                new Models.Station { value = "Hellesylt", description = "Hellesylt", group = "SATREF" },
                new Models.Station { value = "Hemne", description = "Hemne", group = "SATREF" },
                new Models.Station { value = "Hitra", description = "Hitra", group = "SATREF" },
                new Models.Station { value = "Holåsen", description = "Holåsen", group = "SATREF" },
                new Models.Station { value = "Holmvassdalen", description = "Holmvassdalen", group = "SATREF" },
                new Models.Station { value = "Honningsvåg", description = "Honningsvåg", group = "SATREF" },
                new Models.Station { value = "Horten", description = "Horten", group = "SATREF" },
                new Models.Station { value = "Hurum", description = "Hurum", group = "SATREF" },
                new Models.Station { value = "Hustad", description = "Hustad", group = "SATREF" },
                new Models.Station { value = "Ibestad", description = "Ibestad", group = "SATREF" },
                new Models.Station { value = "Innfjorden", description = "Innfjorden", group = "SATREF" },
                new Models.Station { value = "Jostedalen", description = "Jostedalen", group = "SATREF" },
                new Models.Station { value = "Jørstad", description = "Jørstad", group = "SATREF" },
                new Models.Station { value = "Karasjok", description = "Karasjok", group = "SATREF" },
                new Models.Station { value = "Kautokeino", description = "Kautokeino", group = "SATREF" },
                new Models.Station { value = "Kirkenes", description = "Kirkenes", group = "SATREF" },
                new Models.Station { value = "Kjøpsvik", description = "Kjøpsvik", group = "SATREF" },
                new Models.Station { value = "Kobbelv", description = "Kobbelv", group = "SATREF" },
                new Models.Station { value = "Kongsvinger", description = "Kongsvinger", group = "SATREF" },
                new Models.Station { value = "Konsmo", description = "Konsmo", group = "SATREF" },
                new Models.Station { value = "Koppang", description = "Koppang", group = "SATREF" },
                new Models.Station { value = "Kopstad", description = "Kopstad", group = "SATREF" },
                new Models.Station { value = "Kristiansand", description = "Kristiansand", group = "SATREF" },
                new Models.Station { value = "Kristiansund", description = "Kristiansund", group = "SATREF" },
                new Models.Station { value = "Krødsherad", description = "Krødsherad", group = "SATREF" },
                new Models.Station { value = "Kvikne", description = "Kvikne", group = "SATREF" },
                new Models.Station { value = "Kvænangsbotn", description = "Kvænangsbotn", group = "SATREF" },
                new Models.Station { value = "Kyrkjebø", description = "Kyrkjebø", group = "SATREF" },
                new Models.Station { value = "Lakselv", description = "Lakselv", group = "SATREF" },
                new Models.Station { value = "Lauvsnes", description = "Lauvsnes", group = "SATREF" },
                new Models.Station { value = "Leikanger", description = "Leikanger", group = "SATREF" },
                new Models.Station { value = "Leirfjord", description = "Leirfjord", group = "SATREF" },
                new Models.Station { value = "Leknes", description = "Leknes", group = "SATREF" },
                new Models.Station { value = "Leksvik", description = "Leksvik", group = "SATREF" },
                new Models.Station { value = "Lesja", description = "Lesja", group = "SATREF" },
                new Models.Station { value = "Lierne", description = "Lierne", group = "SATREF" },
                new Models.Station { value = "Lillehammer", description = "Lillehammer", group = "SATREF" },
                new Models.Station { value = "Lindås", description = "Lindås", group = "SATREF" },
                new Models.Station { value = "Lista", description = "Lista", group = "SATREF" },
                new Models.Station { value = "Lofoten", description = "Lofoten", group = "SATREF" },
                new Models.Station { value = "Lofthus", description = "Lofthus", group = "SATREF" },
                new Models.Station { value = "Lom", description = "Lom", group = "SATREF" },
                new Models.Station { value = "Longyearbyen", description = "Longyearbyen", group = "SATREF" },
                new Models.Station { value = "Loppa", description = "Loppa", group = "SATREF" },
                new Models.Station { value = "Lund", description = "Lund", group = "SATREF" },
                new Models.Station { value = "Lurøy", description = "Lurøy", group = "SATREF" },
                new Models.Station { value = "Lyngdal", description = "Lyngdal", group = "SATREF" },
                new Models.Station { value = "Lysefjorden", description = "Lysefjorden", group = "SATREF" },
                new Models.Station { value = "Lødingen", description = "Lødingen", group = "SATREF" },
                new Models.Station { value = "Lønsdal", description = "Lønsdal", group = "SATREF" },
                new Models.Station { value = "Løten", description = "Løten", group = "SATREF" },
                new Models.Station { value = "Majavatn", description = "Majavatn", group = "SATREF" },
                new Models.Station { value = "Måløy", description = "Måløy", group = "SATREF" },
                new Models.Station { value = "Marstein", description = "Marstein", group = "SATREF" },
                new Models.Station { value = "Maurset", description = "Maurset", group = "SATREF" },
                new Models.Station { value = "Maze", description = "Maze", group = "SATREF" },
                new Models.Station { value = "Mebonden", description = "Mebonden", group = "SATREF" },
                new Models.Station { value = "Mehamn", description = "Mehamn", group = "SATREF" },
                new Models.Station { value = "Meløy", description = "Meløy", group = "SATREF" },
                new Models.Station { value = "Mo i Rana", description = "Mo i Rana", group = "SATREF" },
                new Models.Station { value = "Moelv", description = "Moelv", group = "SATREF" },
                new Models.Station { value = "Molde", description = "Molde", group = "SATREF" },
                new Models.Station { value = "Moldjord", description = "Moldjord", group = "SATREF" },
                new Models.Station { value = "Mosjøen", description = "Mosjøen", group = "SATREF" },
                new Models.Station { value = "Myre", description = "Myre", group = "SATREF" },
                new Models.Station { value = "Mysen", description = "Mysen", group = "SATREF" },
                new Models.Station { value = "Mære", description = "Mære", group = "SATREF" },
                new Models.Station { value = "Namsos", description = "Namsos", group = "SATREF" },
                new Models.Station { value = "Namsskogan", description = "Namsskogan", group = "SATREF" },
                new Models.Station { value = "Narvik", description = "Narvik", group = "SATREF" },
                new Models.Station { value = "Nordeide", description = "Nordeide", group = "SATREF" },
                new Models.Station { value = "Nordfjordeid", description = "Nordfjordeid", group = "SATREF" },
                new Models.Station { value = "Norheimsund", description = "Norheimsund", group = "SATREF" },
                new Models.Station { value = "Ny-Ålesund", description = "Ny-Ålesund", group = "SATREF" },
                new Models.Station { value = "Olderdalen", description = "Olderdalen", group = "SATREF" },
                new Models.Station { value = "Operaen", description = "Operaen", group = "SATREF" },
                new Models.Station { value = "Oslo", description = "Oslo", group = "SATREF" },
                new Models.Station { value = "Pasvik", description = "Pasvik", group = "SATREF" },
                new Models.Station { value = "Porsgrunn", description = "Porsgrunn", group = "SATREF" },
                new Models.Station { value = "Portør", description = "Portør", group = "SATREF" },
                new Models.Station { value = "Preståsen", description = "Preståsen", group = "SATREF" },
                new Models.Station { value = "Rauland", description = "Rauland", group = "SATREF" },
                new Models.Station { value = "Rena", description = "Rena", group = "SATREF" },
                new Models.Station { value = "Rindal", description = "Rindal", group = "SATREF" },
                new Models.Station { value = "Roan", description = "Roan", group = "SATREF" },
                new Models.Station { value = "Rognan", description = "Rognan", group = "SATREF" },
                new Models.Station { value = "Rosendal", description = "Rosendal", group = "SATREF" },
                new Models.Station { value = "Rustad", description = "Rustad", group = "SATREF" },
                new Models.Station { value = "Røros", description = "Røros", group = "SATREF" },
                new Models.Station { value = "Røst", description = "Røst", group = "SATREF" },
                new Models.Station { value = "Røyrvik", description = "Røyrvik", group = "SATREF" },
                new Models.Station { value = "Sandvika", description = "Sandvika", group = "SATREF" },
                new Models.Station { value = "Sarpsborg", description = "Sarpsborg", group = "SATREF" },
                new Models.Station { value = "Seljord", description = "Seljord", group = "SATREF" },
                new Models.Station { value = "Sirevåg", description = "Sirevåg", group = "SATREF" },
                new Models.Station { value = "Skaland", description = "Skaland", group = "SATREF" },
                new Models.Station { value = "Skånland", description = "Skånland", group = "SATREF" },
                new Models.Station { value = "Skibotn", description = "Skibotn", group = "SATREF" },
                new Models.Station { value = "Skjåk", description = "Skjåk", group = "SATREF" },
                new Models.Station { value = "Skjervøy", description = "Skjervøy", group = "SATREF" },
                new Models.Station { value = "Skollenborg", description = "Skollenborg", group = "SATREF" },
                new Models.Station { value = "Skreia", description = "Skreia", group = "SATREF" },
                new Models.Station { value = "Sleipnes", description = "Sleipnes", group = "SATREF" },
                new Models.Station { value = "Smøla", description = "Smøla", group = "SATREF" },
                new Models.Station { value = "Smørfjord", description = "Smørfjord", group = "SATREF" },
                new Models.Station { value = "Snåsa", description = "Snåsa", group = "SATREF" },
                new Models.Station { value = "Sommarøy", description = "Sommarøy", group = "SATREF" },
                new Models.Station { value = "Sortland", description = "Sortland", group = "SATREF" },
                new Models.Station { value = "Stad", description = "Stad", group = "SATREF" },
                new Models.Station { value = "Stavanger", description = "Stavanger", group = "SATREF" },
                new Models.Station { value = "station", description = "Steigen", group = "SATREF" },
                new Models.Station { value = "Steinkjer", description = "Steinkjer", group = "SATREF" },
                new Models.Station { value = "Stjørdal", description = "Stjørdal", group = "SATREF" },
                new Models.Station { value = "Storforshei", description = "Storforshei", group = "SATREF" },
                new Models.Station { value = "Stranda", description = "Stranda", group = "SATREF" },
                new Models.Station { value = "Støren", description = "Støren", group = "SATREF" },
                new Models.Station { value = "Sulitjelma", description = "Sulitjelma", group = "SATREF" },
                new Models.Station { value = "Svarstad", description = "Svarstad", group = "SATREF" },
                new Models.Station { value = "Sveindal", description = "Sveindal", group = "SATREF" },
                new Models.Station { value = "Svolvær", description = "Svolvær", group = "SATREF" },
                new Models.Station { value = "Tana", description = "Tana", group = "SATREF" },
                new Models.Station { value = "Terråk", description = "Terråk", group = "SATREF" },
                new Models.Station { value = "Tingvoll", description = "Tingvoll", group = "SATREF" },
                new Models.Station { value = "Tjeldstø", description = "Tjeldstø", group = "SATREF" },
                new Models.Station { value = "Tjøme", description = "Tjøme", group = "SATREF" },
                new Models.Station { value = "Tonstad", description = "Tonstad", group = "SATREF" },
                new Models.Station { value = "Toven", description = "Toven", group = "SATREF" },
                new Models.Station { value = "Tregde", description = "Tregde", group = "SATREF" },
                new Models.Station { value = "Trettnes", description = "Trettnes", group = "SATREF" },
                new Models.Station { value = "Treungen", description = "Treungen", group = "SATREF" },
                new Models.Station { value = "Trofors", description = "Trofors", group = "SATREF" },
                new Models.Station { value = "Tromsø", description = "Tromsø", group = "SATREF" },
                new Models.Station { value = "Trondheim", description = "Trondheim", group = "SATREF" },
                new Models.Station { value = "Trysil", description = "Trysil", group = "SATREF" },
                new Models.Station { value = "Tuddal", description = "Tuddal", group = "SATREF" },
                new Models.Station { value = "Tvedestrand", description = "Tvedestrand", group = "SATREF" },
                new Models.Station { value = "Tyin", description = "Tyin", group = "SATREF" },
                new Models.Station { value = "Tysvær", description = "Tysvær", group = "SATREF" },
                new Models.Station { value = "Ulefoss", description = "Ulefoss", group = "SATREF" },
                new Models.Station { value = "Ulsåk", description = "Ulsåk", group = "SATREF" },
                new Models.Station { value = "Vadsø", description = "Vadsø", group = "SATREF" },
                new Models.Station { value = "Valle", description = "Valle", group = "SATREF" },
                new Models.Station { value = "Vardø", description = "Vardø", group = "SATREF" },
                new Models.Station { value = "Vega", description = "Vega", group = "SATREF" },
                new Models.Station { value = "Veggli", description = "Veggli", group = "SATREF" },
                new Models.Station { value = "Verma", description = "Verma", group = "SATREF" },
                new Models.Station { value = "Vikersund", description = "Vikersund", group = "SATREF" },
                new Models.Station { value = "Vikna", description = "Vikna", group = "SATREF" },
                new Models.Station { value = "Vinstra", description = "Vinstra", group = "SATREF" },
                new Models.Station { value = "Volda", description = "Volda", group = "SATREF" },
                new Models.Station { value = "Voss", description = "Voss", group = "SATREF" },
                new Models.Station { value = "Ørnes", description = "Ørnes", group = "SATREF" },
                new Models.Station { value = "Østerbø", description = "Østerbø", group = "SATREF" },
                new Models.Station { value = "Øverbygd", description = "Øverbygd", group = "SATREF" },

                //Vannstandsmåler  https://www.kartverket.no/globalassets/til-sjos/illustrasjoner/permanente-vannstandsmaalere-kartverket.pdf
                new Models.Station { value = "Viker", description = "Viker", group = "Vannstandsmåler" },
                new Models.Station { value = "Tregde", description = "Tregde", group = "Vannstandsmåler" },
                new Models.Station { value = "Helgeroa", description = "Helgeroa", group = "Vannstandsmåler" },
                new Models.Station { value = "Stavanger", description = "Stavanger", group = "Vannstandsmåler" },
                new Models.Station { value = "Oscarsborg", description = "Oscarsborg", group = "Vannstandsmåler" },
                new Models.Station { value = "Oslo", description = "Oslo", group = "Vannstandsmåler" },
                new Models.Station { value = "Bergen", description = "Bergen", group = "Vannstandsmåler" },
                new Models.Station { value = "Måløy", description = "Måløy", group = "Vannstandsmåler" },
                new Models.Station { value = "Ålesund", description = "Ålesund", group = "Vannstandsmåler" },
                new Models.Station { value = "Kristiansund", description = "Kristiansund", group = "Vannstandsmåler" },
                new Models.Station { value = "Trondheim", description = "Trondheim", group = "Vannstandsmåler" },
                new Models.Station { value = "Heimsjø", description = "Heimsjø", group = "Vannstandsmåler" },
                new Models.Station { value = "Mausund", description = "Mausund", group = "Vannstandsmåler" },
                new Models.Station { value = "Rørvik", description = "Rørvik", group = "Vannstandsmåler" },
                new Models.Station { value = "Bodø", description = "Bodø", group = "Vannstandsmåler" },
                new Models.Station { value = "Kabelvåg", description = "Kabelvåg", group = "Vannstandsmåler" },
                new Models.Station { value = "Narvik", description = "Narvik", group = "Vannstandsmåler" },
                new Models.Station { value = "Harstad", description = "Harstad", group = "Vannstandsmåler" },
                new Models.Station { value = "Andenes", description = "Andenes", group = "Vannstandsmåler" },
                new Models.Station { value = "Tromsø", description = "Tromsø", group = "Vannstandsmåler" },
                new Models.Station { value = "Hammerfest", description = "Hammerfest", group = "Vannstandsmåler" },
                new Models.Station { value = "Vardø", description = "Vardø", group = "Vannstandsmåler" },
                new Models.Station { value = "Honningsvåg", description = "Honningsvåg", group = "Vannstandsmåler" }
            );

            context.Registers.AddOrUpdate(
            //new Register
            //{
            //    systemId = Guid.Parse("9a9bef28-285b-477e-85f1-504f8227ff45"),
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Inspire statusregister",
            //    seoname = "inspire-statusregister",
            //    statusId = "Valid",
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    containedItemClass = "InspireDataset",
            //    accessId = 1
            //},

            //new Register
            //    {
            //        systemId = Guid.Parse("3d9114f6-faab-4521-bdf8-19ef6211e7d2"),
            //        ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //        managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //        name = "Geodatalov statusregister",
            //        seoname = "geodatalov-statusregister",
            //        statusId = "Valid",
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        containedItemClass = "GeodatalovDataset",
            //        accessId = 1
            //    }
            //new Register
            //{
            //    systemId = Guid.Parse(GlobalVariables.MareanoRegistryId),
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Mareano statusregister",
            //    seoname = "mareano-statusregister",
            //    statusId = "Valid",
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    containedItemClass = "MareanoDataset",
            //    accessId = 1
            //    }
            );


            // *** DEFAULT REGISTERS

            //Register produktspesifikasjon = new Register
            //{
            //    systemId = Guid.Parse("8E726684-F216-4497-91BE-6AB2496A84D3"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Produktspesifikasjoner",
            //    seoname = "produktspesifikasjoner",
            //    description = "Inneholder dokumenter for produktspesifikasjoner for kart- og geodata ",
            //    containedItemClass = "Document",
            //    statusId = "Valid",
            //    accessId = 2,
            //};
            //Register organisasjoner = new Register
            //{
            //    systemId = Guid.Parse("FCB0685D-24EB-4156-9AC8-25FA30759094"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Organisasjoner",
            //    seoname = "organisasjoner",
            //    description = "Inneholder oversikt over organisasjoner og deres logo ",
            //    containedItemClass = "Organization",
            //    statusId = "Valid",
            //    accessId = 1,
            //};
            //Register produktark = new Register
            //{
            //    systemId = Guid.Parse("A42BC2B3-2314-4B7E-8007-71D9B10F2C04"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Produktark",
            //    seoname = "produktark",
            //    description = "Inneholder organisasjoners produktark for kart og geodata",
            //    containedItemClass = "Document",
            //    statusId = "Valid",
            //    accessId = 2
            //};
            //Register gmlApplikasjonsskjema = new Register
            //{
            //    systemId = Guid.Parse("E43B65C6-452F-489D-A2E6-A5262E5740D8"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "GML applikasjonsskjema",
            //    seoname = "gml-applikasjonsskjema",
            //    description = "Inneholder godkjente GML applikasjonsskjema",
            //    containedItemClass = "Document",
            //    statusId = "Valid",
            //    accessId = 2
            //};
            //Register epsgKoder = new Register
            //{
            //    systemId = Guid.Parse("37B9DC41-D868-4CBC-84F9-39557041FB2C"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "EPSG koder",
            //    seoname = "epsg-koder",
            //    description = "Inneholder oversikt over EPSG koder som benyttes i Norge Digitalt omtalt i rammeverksdokumentet ",
            //    containedItemClass = "EPSG",
            //    statusId = "Valid",
            //    accessId = 1,
            //};
            //Register tegneregler = new Register
            //{
            //    systemId = Guid.Parse("5EACB130-D61F-469D-8454-E96943491BA0"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Tegneregler",
            //    seoname = "tegneregler",
            //    description = "Inneholder dokumenter med tegneregler og kartografi",
            //    containedItemClass = "Document",
            //    statusId = "Valid",
            //    accessId = 2
            //};
            //Register veiledningsdokumenter = new Register
            //{
            //    systemId = Guid.Parse("b2e5f822-994f-47f5-ac52-cd4153d55197"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Veiledningsdokumenter",
            //    seoname = "veiledningsdokumenter",
            //    containedItemClass = "Register",
            //    accessId = 1
            //};
            //Register dokRegister = new Register
            //{
            //    systemId = Guid.Parse("CD429E8B-2533-45D8-BCAA-86BC2CBDD0DD"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Det offentlige kartgrunnlaget",
            //    seoname = "det-offentlige-kartgrunnlaget",
            //    description = "Det offentlige kartgrunnlaget beskrives i plan- og bygningslovens paragraf 2-1 og kart- og planforskriften og skal være er en samling geografiske kvalitetsdata, såkalt offentlige autoritative data. Disse skal være valgt ut og tilrettelagt for å være et egnet kunnskapsgrunnlag for de mest vesentlige behovene som følger av plan- og bygningsloven.",
            //    containedItemClass = "Dataset",
            //    statusId = "Valid",
            //    accessId = 1
            //};

            //Register dokKommunalt = new Register
            //{
            //    systemId = Guid.Parse("E807439B-2BFC-4DA5-87C0-B40E7B0CDFB8"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Det offentlige kartgrunnlaget - Kommunalt",
            //    seoname = "det-offentlige-kartgrunnlaget-kommunalt",
            //    description = "Tabellen viser kommunen sitt bekreftede DOK",
            //    containedItemClass = "Dataset",
            //    statusId = "Valid",
            //    accessId = 4
            //};

            //Register navnerom = new Register
            //{
            //    systemId = Guid.Parse("9A82A6B6-0069-45A4-8CA8-FBB789434F9A"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Navnerom",
            //    seoname = "navnerom",
            //    description = "Inneholder godkjente navnerom",
            //    containedItemClass = "NameSpace",
            //    statusId = "Valid",
            //    accessId = 2
            //};

            //Register tjenestevarsler = new Register
            //{
            //    systemId = Guid.Parse("0f428034-0b2d-4fb7-84ea-c547b872b418"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Tjenestevarsler",
            //    seoname = "tjenestevarsler",
            //    description = "Register over alle endringsvarsler for tjenester registrert i Geonorge",
            //    containedItemClass = "ServiceAlert",
            //    statusId = "Valid",
            //    accessId = 2
            //};

            //Register styrendedokumenter = new Register
            //{
            //    systemId = Guid.Parse("b2e5f822-994d-47f5-ac52-cd4153d55198"),
            //    dateSubmitted = DateTime.Now,
            //    modified = DateTime.Now,
            //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //    name = "Styrende dokumenter",
            //    seoname = "styrendedokumenter",
            //    containedItemClass = "Register",
            //    accessId = 1
            //};

            //context.Registers.AddOrUpdate(
            //    styrendedokumenter
            //);

            //context.Database.ExecuteSqlCommand("UPDATE Registers SET parentRegisterId = 'b2e5f822-994d-47f5-ac52-cd4153d55198'  WHERE  systemid='3A95CA12-4BD4-40E2-9E23-3875B68E83CD'");

            context.Database.ExecuteSqlCommand("UPDATE Registers SET name = 'Varsler', description = 'Register over alle varsler registrert i Geonorge', seoname = 'varsler'  WHERE  systemid='0f428034-0b2d-4fb7-84ea-c547b872b418'");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET AlertCategory = '" + Constants.AlertCategoryService+ "'  WHERE  [AlertCategory] is  null and Discriminator='Alert'");

            //context.Registers.AddOrUpdate(
            //    produktspesifikasjon,
            //    organisasjoner,
            //    produktark,
            //    gmlApplikasjonsskjema,
            //    epsgKoder,
            //    tegneregler,
            //    veiledningsdokumenter,
            //    dokRegister,
            //    dokKommunalt,
            //    navnerom
            //      tjenestevarsler
            //);


            //Eksempeldata

            //context.Organizations.AddOrUpdate(
            //    new Organization 
            //    { 
            //        systemId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"), 
            //        registerId = organisasjoner.systemId, 
            //        dateSubmitted = DateTime.Now, 
            //        modified = DateTime.Now,
            //        number = "971040238", 
            //        name = "Kartverket", 
            //        contact = "Lars Inge Arnevik",
            //        logoFilename = "971040238_kartverket_logo.png",
            //        largeLogo = "971040238_kartverket_logo.png"
            //    },

            //FixSubmitter
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET submitterId = '10087020-F17C-45E1-8542-02ACBCF3D8A3' WHERE  (documentownerId IS NULL)");

            // Set status
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET statusId = 'Submitted' WHERE  (statusId IS NULL)");

            //  *** VERSJONERING
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET versionNumber = 1 WHERE  (versionNumber=0)");
            context.Database.ExecuteSqlCommand("UPDATE Registers SET versionNumber = 1 WHERE  (versionNumber=0)");
            context.Database.ExecuteSqlCommand("INSERT INTO Versions (systemId, currentVersion, lastVersionNumber, containedItemClass) SELECT NEWID() as systemId, systemId as currentVersion, versionNumber as lastVersionNumber, containedItemClass as containedItemClass FROM Registers WHERE versioningId IS NULL");
            context.Database.ExecuteSqlCommand("INSERT INTO Versions (systemId, currentVersion, lastVersionNumber, containedItemClass) SELECT NEWID() as systemId, systemId as currentVersion, versionNumber as lastVersionNumber, Discriminator as containedItemClass FROM RegisterItems WHERE versioningId IS NULL");

            //Change Det offentlige kartgrunnlaget  DOK-statusregisteret
            context.Database.ExecuteSqlCommand("UPDATE Registers SET name = 'DOK-statusregisteret' WHERE  (name='Det offentlige kartgrunnlaget')");

            //Set default value for DokDeliveryStatus
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryMetadataStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryMetadataStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryProductSheetStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryProductSheetStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryPresentationRulesStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryPresentationRulesStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryProductSpecificationStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryProductSpecificationStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryWmsStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryWmsStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryWfsStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryWfsStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliverySosiRequirementsStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliverySosiRequirementsStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryDistributionStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryDistributionStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryGmlRequirementsStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryGmlRequirementsStatusId IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryAtomFeedStatusId = 'notset' WHERE  Discriminator ='Dataset' AND dokDeliveryAtomFeedStatusId IS NULL");

            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryMetadataStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryMetadataStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryProductSheetStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryProductSheetStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryPresentationRulesStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryPresentationRulesStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryProductSpecificationStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryProductSpecificationStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryWmsStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryWmsStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryWfsStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryWfsStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliverySosiStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliverySosiStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryDistributionStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryDistributionStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryGmlRequirementsStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryGmlRequirementsStatusAutoUpdate IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokDeliveryAtomFeedStatusAutoUpdate = 1 WHERE  Discriminator ='Dataset' AND dokDeliveryAtomFeedStatusAutoUpdate IS NULL");

            //Suitability
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET ZoningPlan = 0 WHERE  Discriminator ='Dataset' AND ZoningPlan IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET ImpactAssessmentPlanningBuildingAct = 0 WHERE  Discriminator ='Dataset' AND ImpactAssessmentPlanningBuildingAct IS NULL");
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET RiskVulnerabilityAnalysisPlanningBuildingAct = 0 WHERE  Discriminator ='Dataset' AND RiskVulnerabilityAnalysisPlanningBuildingAct IS NULL");



            context.Database.ExecuteSqlCommand("UPDATE Synchronizes SET Active = 0 WHERE Active = 1");


            var queryResult = from c in context.CoverageDatasets
                where c.dataset.DatasetType == "Kommunalt"
                select c.CoverageId;
            var result = queryResult.ToList();

            foreach (var coverageId in result)
            {
                context.Database.ExecuteSqlCommand("UPDATE CoverageDatasets SET ConfirmedDok = 'True' WHERE  CoverageId ='" + coverageId + "'");
                context.Database.ExecuteSqlCommand("UPDATE CoverageDatasets SET Coverage = 'True' WHERE  CoverageId ='" + coverageId + "'");
            }


            //var items = context.Database.SqlQuery<Item>
            //    (@"WITH H AS 
            //                 (
            //                SELECT systemId, parentRegisterId, name, CAST(seoname AS NVARCHAR(300)) AS path
            //                FROM Registers
            //                WHERE pathOld is null AND parentRegisterId IS NULL
            //                    UNION ALL
            //                SELECT R.systemId, R.parentRegisterId, R.name, CAST(H.path + '/' + R.seoname AS NVARCHAR(300))
            //                FROM Registers R INNER JOIN H ON R.parentRegisterId = H.systemId
            //                )
            //                SELECT systemId, path FROM H")
            //.ToList();

            //foreach(var item in items)
            //{
            //    var pathArray = item.path.Split('/');
            //    string oldPath = "";
            //    if (pathArray.Length == 1)
            //        oldPath = pathArray[0];
            //    else
            //    {
            //        var length = pathArray.Length;
            //        oldPath = pathArray[length - 2] + "/" + pathArray[length - 1];
            //    }

            //    context.Database.ExecuteSqlCommand("UPDATE Registers SET pathOld = '" + oldPath + "' WHERE  systemId ='" + item.systemId + "'");

            //}

            //items = context.Database.SqlQuery<Item>
            //    (@"WITH H AS 
            //                 (
            //                SELECT systemId, parentRegisterId, name, CAST(seoname AS NVARCHAR(300)) AS path
            //                FROM Registers
            //                WHERE path is null AND parentRegisterId IS NULL
            //                    UNION ALL
            //                SELECT R.systemId, R.parentRegisterId, R.name, CAST(H.path + '/' + R.seoname AS NVARCHAR(300))
            //                FROM Registers R INNER JOIN H ON R.parentRegisterId = H.systemId
            //                )
            //                SELECT systemId, path FROM H")
            //.ToList();

            //foreach (var item in items)
            //{

            //    context.Database.ExecuteSqlCommand("UPDATE Registers SET path = '" + item.path + "' WHERE  systemId ='" + item.systemId + "'");

            //}



        }
    }

    public class Item
    {
        public Guid systemId { get; set; }
        public string path { get; set; }
    }
}

