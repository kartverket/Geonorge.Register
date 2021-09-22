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
                new Status { value = "Retired", description = "Utg�tt", group = "historical" }
            );

            context.DokStatuses.AddOrUpdate(
                new DokStatus { value = "Proposal", description = "Forslag" },
                new DokStatus { value = "Candidate", description = "Kandidat" },
                new DokStatus { value = "InProgress", description = "I prosess" },
                new DokStatus { value = "Accepted", description = "Godkjent" }
            );

            context.DokMeasureStatuses.AddOrUpdate(
                new DokMeasureStatus { value = "Forslag til kommunens egne tiltak", description = "Forslag til kommunens egne tiltak", sortorder = 1 },
                new DokMeasureStatus { value = "Forslag til tiltak i geodataplanen", description = "Forslag til tiltak i geodataplanen", sortorder = 2 },
                new DokMeasureStatus { value = "Forslag til tiltak hos dataeier", description = "Forslag til tiltak hos dataeier", sortorder = 3 },
                new DokMeasureStatus { value = "Forslag til endringer i DOK", description = "Forslag til endringer i DOK", sortorder = 4 },
                new DokMeasureStatus { value = "Annet", description = "Annet", sortorder = 5 }
            );

            context.DokDeliveryStatuses.AddOrUpdate(
                new Models.DokDeliveryStatus { value = "deficient", description = "Ikke levert" ,sortorder = 4 },
                new Models.DokDeliveryStatus { value = "useable", description = "Brukbar", sortorder = 2 },
                new Models.DokDeliveryStatus { value = "good", description = "God", sortorder = 1 },
                new Models.DokDeliveryStatus { value = "notset", description = "Ikke angitt", sortorder = 3 }
            );

            context.FAIRDeliveryStatuses.AddOrUpdate(
                new Models.FAIRDeliveryStatus { value = "deficient", description = "D�rlig", sortorder = 5 },
                new Models.FAIRDeliveryStatus { value = "satisfactory", description = "Tilfredsstillende", sortorder = 2 },
                new Models.FAIRDeliveryStatus { value = "useable", description = "B�r forbedres", sortorder = 3 },
                new Models.FAIRDeliveryStatus { value = "good", description = "God", sortorder = 1 },
                new Models.FAIRDeliveryStatus { value = "notset", description = "Ikke angitt", sortorder = 4 }
            );

            context.AccessTypes.AddOrUpdate(
                new accessType { accessLevel = 1, description = "Only admin kan create, edit or delete" },
                new accessType { accessLevel = 2, description = "Editor can create, edit or delete their owne items" },
                new accessType { accessLevel = 4, description = "Municipalities can create, update and delete their own items" }
            );

            context.SaveChanges();

            context.requirements.AddOrUpdate(
                new Requirement { value = "Mandatory", description = "P�krevd", sortOrder = 0 },
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
                new Sorting { value = "name_desc", description = "Navn �-a" },
                new Sorting { value = "submitter", description = "Innsender a-�" },
                new Sorting { value = "submitter_desc", description = "Innsender �-a" },
                new Sorting { value = "status", description = "Status a-�", },
                new Sorting { value = "status_desc", description = "Status �-a" },
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
                new Models.Tag { value = "H�ydedata", description = "H�ydedata" },
                new Models.Tag { value = "CPOS", description = "CPOS" },
                new Models.Tag { value = "DPOS", description = "DPOS" },
                new Models.Tag { value = "EFS", description = "EFS" }
            );

            context.Departments.AddOrUpdate(
                new Models.Department { value = "Posisjonstjenester", description = "Posisjonstjenester" },
                new Models.Department { value = "Eiendom", description = "Eiendom" },
                new Models.Department { value = "Til sj�s", description = "Til sj�s" },
                new Models.Department { value = "Data, API, nettsider", description = "Data, API, nettsider" }
            );

            context.Stations.AddOrUpdate(

                //satref: http://satref.geodesi.no/
                new Models.Station { StationName = "Adamselv", Description = "Adamselv", StationType = "SATREF" },
                new Models.Station { StationName = "�krahamn", Description = "�krahamn", StationType = "SATREF" },
                new Models.Station { StationName = "�lesund", Description = "�lesund", StationType = "SATREF" },
                new Models.Station { StationName = "Alta", Description = "Alta", StationType = "SATREF" },
                new Models.Station { StationName = "Alteidet", Description = "Alteidet", StationType = "SATREF" },
                new Models.Station { StationName = "Alvdal", Description = "Alvdal", StationType = "SATREF" },
                new Models.Station { StationName = "Andenes", Description = "Andenes", StationType = "SATREF" },
                new Models.Station { StationName = "And�ya", Description = "And�ya", StationType = "SATREF" },
                new Models.Station { StationName = "�rdal", Description = "�rdal", StationType = "SATREF" },
                new Models.Station { StationName = "Arendal", Description = "Arendal", StationType = "SATREF" },
                new Models.Station { StationName = "�rnes", Description = "�rnes", StationType = "SATREF" },
                new Models.Station { StationName = "�s", Description = "�s", StationType = "SATREF" },
                new Models.Station { StationName = "�sane", Description = "�sane", StationType = "SATREF" },
                new Models.Station { StationName = "Asker", Description = "Asker", StationType = "SATREF" },
                new Models.Station { StationName = "Askvoll", Description = "Askvoll", StationType = "SATREF" },
                new Models.Station { StationName = "Atna", Description = "Atna", StationType = "SATREF" },
                new Models.Station { StationName = "Aurdal", Description = "Aurdal", StationType = "SATREF" },
                new Models.Station { StationName = "Austevoll", Description = "Austevoll", StationType = "SATREF" },
                new Models.Station { StationName = "Balestrand", Description = "Balestrand", StationType = "SATREF" },
                new Models.Station { StationName = "Balsfjord", Description = "Balsfjord", StationType = "SATREF" },
                new Models.Station { StationName = "Bardu", Description = "Bardu", StationType = "SATREF" },
                new Models.Station { StationName = "B�tsfjord", Description = "B�tsfjord", StationType = "SATREF" },
                new Models.Station { StationName = "Bergen", Description = "Bergen", StationType = "SATREF" },
                new Models.Station { StationName = "Bergen havn", Description = "Bergen havn", StationType = "SATREF" },
                new Models.Station { StationName = "Berk�k", Description = "Berk�k", StationType = "SATREF" },
                new Models.Station { StationName = "Berlev�g", Description = "Berlev�g", StationType = "SATREF" },
                new Models.Station { StationName = "Birkenes", Description = "Birkenes", StationType = "SATREF" },
                new Models.Station { StationName = "Bjark�y", Description = "Bjark�y", StationType = "SATREF" },
                new Models.Station { StationName = "Bjorli", Description = "Bjorli", StationType = "SATREF" },
                new Models.Station { StationName = "Bjugn", Description = "Bjugn", StationType = "SATREF" },
                new Models.Station { StationName = "Bj�rnstad", Description = "Bj�rnstad", StationType = "SATREF" },
                new Models.Station { StationName = "Bleikvassli", Description = "Bleikvassli", StationType = "SATREF" },
                new Models.Station { StationName = "Bod�", Description = "Bod�", StationType = "SATREF" },
                new Models.Station { StationName = "Breivikbotn", Description = "Breivikbotn", StationType = "SATREF" },
                new Models.Station { StationName = "Br�nn�ysund", Description = "Br�nn�ysund", StationType = "SATREF" },
                new Models.Station { StationName = "Bygland", Description = "Bygland", StationType = "SATREF" },
                new Models.Station { StationName = "B�", Description = "B�", StationType = "SATREF" },
                new Models.Station { StationName = "B�mlo", Description = "B�mlo", StationType = "SATREF" },
                new Models.Station { StationName = "Dagali", Description = "Dagali", StationType = "SATREF" },
                new Models.Station { StationName = "Dokka", Description = "Dokka", StationType = "SATREF" },
                new Models.Station { StationName = "Domb�s", Description = "Domb�s", StationType = "SATREF" },
                new Models.Station { StationName = "Drevsj�", Description = "Drevsj�", StationType = "SATREF" },
                new Models.Station { StationName = "D�nna", Description = "D�nna", StationType = "SATREF" },
                new Models.Station { StationName = "Eidsvoll", Description = "Eidsvoll", StationType = "SATREF" },
                new Models.Station { StationName = "Elverum", Description = "Elverum", StationType = "SATREF" },
                new Models.Station { StationName = "Etne", Description = "Etne", StationType = "SATREF" },
                new Models.Station { StationName = "Evenstad", Description = "Evenstad", StationType = "SATREF" },
                new Models.Station { StationName = "Fauske", Description = "Fauske", StationType = "SATREF" },
                new Models.Station { StationName = "F�vang", Description = "F�vang", StationType = "SATREF" },
                new Models.Station { StationName = "Fet", Description = "Fet", StationType = "SATREF" },
                new Models.Station { StationName = "Finnsnes", Description = "Finnsnes", StationType = "SATREF" },
                new Models.Station { StationName = "Finn�y", Description = "Finn�y", StationType = "SATREF" },
                new Models.Station { StationName = "Flisa", Description = "Flisa", StationType = "SATREF" },
                new Models.Station { StationName = "Flornes", Description = "Flornes", StationType = "SATREF" },
                new Models.Station { StationName = "Flor�", Description = "Flor�", StationType = "SATREF" },
                new Models.Station { StationName = "Folldal", Description = "Folldal", StationType = "SATREF" },
                new Models.Station { StationName = "Fredrikstad", Description = "Fredrikstad", StationType = "SATREF" },
                new Models.Station { StationName = "Fr�ya", Description = "Fr�ya", StationType = "SATREF" },
                new Models.Station { StationName = "F�lling", Description = "F�lling", StationType = "SATREF" },
                new Models.Station { StationName = "F�rde", Description = "F�rde", StationType = "SATREF" },
                new Models.Station { StationName = "Gjerde", Description = "Gjerde", StationType = "SATREF" },
                new Models.Station { StationName = "Gjerdrum", Description = "Gjerdrum", StationType = "SATREF" },
                new Models.Station { StationName = "Gjesdal", Description = "Gjesdal", StationType = "SATREF" },
                new Models.Station { StationName = "Gj�ra", Description = "Gj�ra", StationType = "SATREF" },
                new Models.Station { StationName = "Glesv�r", Description = "Glesv�r", StationType = "SATREF" },
                new Models.Station { StationName = "Gloppen", Description = "Gloppen", StationType = "SATREF" },
                new Models.Station { StationName = "Gol", Description = "Gol", StationType = "SATREF" },
                new Models.Station { StationName = "Gran", Description = "Gran", StationType = "SATREF" },
                new Models.Station { StationName = "Grong", Description = "Grong", StationType = "SATREF" },
                new Models.Station { StationName = "Haltdalen", Description = "Haltdalen", StationType = "SATREF" },
                new Models.Station { StationName = "Hamar", Description = "Hamar", StationType = "SATREF" },
                new Models.Station { StationName = "Hamar�y", Description = "Hamar�y", StationType = "SATREF" },
                new Models.Station { StationName = "Hammerfest", Description = "Hammerfest", StationType = "SATREF" },
                new Models.Station { StationName = "Hansnes", Description = "Hansnes", StationType = "SATREF" },
                new Models.Station { StationName = "Hardbakke", Description = "Hardbakke", StationType = "SATREF" },
                new Models.Station { StationName = "Haukeli", Description = "Haukeli", StationType = "SATREF" },
                new Models.Station { StationName = "Hav�ysund", Description = "Hav�ysund", StationType = "SATREF" },
                new Models.Station { StationName = "Hedalen", Description = "Hedalen", StationType = "SATREF" },
                new Models.Station { StationName = "Heggenes", Description = "Heggenes", StationType = "SATREF" },
                new Models.Station { StationName = "Hellesylt", Description = "Hellesylt", StationType = "SATREF" },
                new Models.Station { StationName = "Hemne", Description = "Hemne", StationType = "SATREF" },
                new Models.Station { StationName = "Hitra", Description = "Hitra", StationType = "SATREF" },
                new Models.Station { StationName = "Hol�sen", Description = "Hol�sen", StationType = "SATREF" },
                new Models.Station { StationName = "Holmvassdalen", Description = "Holmvassdalen", StationType = "SATREF" },
                new Models.Station { StationName = "Honningsv�g", Description = "Honningsv�g", StationType = "SATREF" },
                new Models.Station { StationName = "Horten", Description = "Horten", StationType = "SATREF" },
                new Models.Station { StationName = "Hurum", Description = "Hurum", StationType = "SATREF" },
                new Models.Station { StationName = "Hustad", Description = "Hustad", StationType = "SATREF" },
                new Models.Station { StationName = "Ibestad", Description = "Ibestad", StationType = "SATREF" },
                new Models.Station { StationName = "Innfjorden", Description = "Innfjorden", StationType = "SATREF" },
                new Models.Station { StationName = "Jostedalen", Description = "Jostedalen", StationType = "SATREF" },
                new Models.Station { StationName = "J�rstad", Description = "J�rstad", StationType = "SATREF" },
                new Models.Station { StationName = "Karasjok", Description = "Karasjok", StationType = "SATREF" },
                new Models.Station { StationName = "Kautokeino", Description = "Kautokeino", StationType = "SATREF" },
                new Models.Station { StationName = "Kirkenes", Description = "Kirkenes", StationType = "SATREF" },
                new Models.Station { StationName = "Kj�psvik", Description = "Kj�psvik", StationType = "SATREF" },
                new Models.Station { StationName = "Kobbelv", Description = "Kobbelv", StationType = "SATREF" },
                new Models.Station { StationName = "Kongsvinger", Description = "Kongsvinger", StationType = "SATREF" },
                new Models.Station { StationName = "Konsmo", Description = "Konsmo", StationType = "SATREF" },
                new Models.Station { StationName = "Koppang", Description = "Koppang", StationType = "SATREF" },
                new Models.Station { StationName = "Kopstad", Description = "Kopstad", StationType = "SATREF" },
                new Models.Station { StationName = "Kristiansand", Description = "Kristiansand", StationType = "SATREF" },
                new Models.Station { StationName = "Kristiansund", Description = "Kristiansund", StationType = "SATREF" },
                new Models.Station { StationName = "Kr�dsherad", Description = "Kr�dsherad", StationType = "SATREF" },
                new Models.Station { StationName = "Kvikne", Description = "Kvikne", StationType = "SATREF" },
                new Models.Station { StationName = "Kv�nangsbotn", Description = "Kv�nangsbotn", StationType = "SATREF" },
                new Models.Station { StationName = "Kyrkjeb�", Description = "Kyrkjeb�", StationType = "SATREF" },
                new Models.Station { StationName = "Lakselv", Description = "Lakselv", StationType = "SATREF" },
                new Models.Station { StationName = "Lauvsnes", Description = "Lauvsnes", StationType = "SATREF" },
                new Models.Station { StationName = "Leikanger", Description = "Leikanger", StationType = "SATREF" },
                new Models.Station { StationName = "Leirfjord", Description = "Leirfjord", StationType = "SATREF" },
                new Models.Station { StationName = "Leknes", Description = "Leknes", StationType = "SATREF" },
                new Models.Station { StationName = "Leksvik", Description = "Leksvik", StationType = "SATREF" },
                new Models.Station { StationName = "Lesja", Description = "Lesja", StationType = "SATREF" },
                new Models.Station { StationName = "Lierne", Description = "Lierne", StationType = "SATREF" },
                new Models.Station { StationName = "Lillehammer", Description = "Lillehammer", StationType = "SATREF" },
                new Models.Station { StationName = "Lind�s", Description = "Lind�s", StationType = "SATREF" },
                new Models.Station { StationName = "Lista", Description = "Lista", StationType = "SATREF" },
                new Models.Station { StationName = "Lofoten", Description = "Lofoten", StationType = "SATREF" },
                new Models.Station { StationName = "Lofthus", Description = "Lofthus", StationType = "SATREF" },
                new Models.Station { StationName = "Lom", Description = "Lom", StationType = "SATREF" },
                new Models.Station { StationName = "Longyearbyen", Description = "Longyearbyen", StationType = "SATREF" },
                new Models.Station { StationName = "Loppa", Description = "Loppa", StationType = "SATREF" },
                new Models.Station { StationName = "Lund", Description = "Lund", StationType = "SATREF" },
                new Models.Station { StationName = "Lur�y", Description = "Lur�y", StationType = "SATREF" },
                new Models.Station { StationName = "Lyngdal", Description = "Lyngdal", StationType = "SATREF" },
                new Models.Station { StationName = "Lysefjorden", Description = "Lysefjorden", StationType = "SATREF" },
                new Models.Station { StationName = "L�dingen", Description = "L�dingen", StationType = "SATREF" },
                new Models.Station { StationName = "L�nsdal", Description = "L�nsdal", StationType = "SATREF" },
                new Models.Station { StationName = "L�ten", Description = "L�ten", StationType = "SATREF" },
                new Models.Station { StationName = "Majavatn", Description = "Majavatn", StationType = "SATREF" },
                new Models.Station { StationName = "M�l�y", Description = "M�l�y", StationType = "SATREF" },
                new Models.Station { StationName = "Marstein", Description = "Marstein", StationType = "SATREF" },
                new Models.Station { StationName = "Maurset", Description = "Maurset", StationType = "SATREF" },
                new Models.Station { StationName = "Maze", Description = "Maze", StationType = "SATREF" },
                new Models.Station { StationName = "Mebonden", Description = "Mebonden", StationType = "SATREF" },
                new Models.Station { StationName = "Mehamn", Description = "Mehamn", StationType = "SATREF" },
                new Models.Station { StationName = "Mel�y", Description = "Mel�y", StationType = "SATREF" },
                new Models.Station { StationName = "Mo i Rana", Description = "Mo i Rana", StationType = "SATREF" },
                new Models.Station { StationName = "Moelv", Description = "Moelv", StationType = "SATREF" },
                new Models.Station { StationName = "Molde", Description = "Molde", StationType = "SATREF" },
                new Models.Station { StationName = "Moldjord", Description = "Moldjord", StationType = "SATREF" },
                new Models.Station { StationName = "Mosj�en", Description = "Mosj�en", StationType = "SATREF" },
                new Models.Station { StationName = "Myre", Description = "Myre", StationType = "SATREF" },
                new Models.Station { StationName = "Mysen", Description = "Mysen", StationType = "SATREF" },
                new Models.Station { StationName = "M�re", Description = "M�re", StationType = "SATREF" },
                new Models.Station { StationName = "Namsos", Description = "Namsos", StationType = "SATREF" },
                new Models.Station { StationName = "Namsskogan", Description = "Namsskogan", StationType = "SATREF" },
                new Models.Station { StationName = "Narvik", Description = "Narvik", StationType = "SATREF" },
                new Models.Station { StationName = "Nordeide", Description = "Nordeide", StationType = "SATREF" },
                new Models.Station { StationName = "Nordfjordeid", Description = "Nordfjordeid", StationType = "SATREF" },
                new Models.Station { StationName = "Norheimsund", Description = "Norheimsund", StationType = "SATREF" },
                new Models.Station { StationName = "Ny-�lesund", Description = "Ny-�lesund", StationType = "SATREF" },
                new Models.Station { StationName = "Olderdalen", Description = "Olderdalen", StationType = "SATREF" },
                new Models.Station { StationName = "Operaen", Description = "Operaen", StationType = "SATREF" },
                new Models.Station { StationName = "Oslo", Description = "Oslo", StationType = "SATREF" },
                new Models.Station { StationName = "Pasvik", Description = "Pasvik", StationType = "SATREF" },
                new Models.Station { StationName = "Porsgrunn", Description = "Porsgrunn", StationType = "SATREF" },
                new Models.Station { StationName = "Port�r", Description = "Port�r", StationType = "SATREF" },
                new Models.Station { StationName = "Prest�sen", Description = "Prest�sen", StationType = "SATREF" },
                new Models.Station { StationName = "Rauland", Description = "Rauland", StationType = "SATREF" },
                new Models.Station { StationName = "Rena", Description = "Rena", StationType = "SATREF" },
                new Models.Station { StationName = "Rindal", Description = "Rindal", StationType = "SATREF" },
                new Models.Station { StationName = "Roan", Description = "Roan", StationType = "SATREF" },
                new Models.Station { StationName = "Rognan", Description = "Rognan", StationType = "SATREF" },
                new Models.Station { StationName = "Rosendal", Description = "Rosendal", StationType = "SATREF" },
                new Models.Station { StationName = "Rustad", Description = "Rustad", StationType = "SATREF" },
                new Models.Station { StationName = "R�ros", Description = "R�ros", StationType = "SATREF" },
                new Models.Station { StationName = "R�st", Description = "R�st", StationType = "SATREF" },
                new Models.Station { StationName = "R�yrvik", Description = "R�yrvik", StationType = "SATREF" },
                new Models.Station { StationName = "Sandvika", Description = "Sandvika", StationType = "SATREF" },
                new Models.Station { StationName = "Sarpsborg", Description = "Sarpsborg", StationType = "SATREF" },
                new Models.Station { StationName = "Seljord", Description = "Seljord", StationType = "SATREF" },
                new Models.Station { StationName = "Sirev�g", Description = "Sirev�g", StationType = "SATREF" },
                new Models.Station { StationName = "Skaland", Description = "Skaland", StationType = "SATREF" },
                new Models.Station { StationName = "Sk�nland", Description = "Sk�nland", StationType = "SATREF" },
                new Models.Station { StationName = "Skibotn", Description = "Skibotn", StationType = "SATREF" },
                new Models.Station { StationName = "Skj�k", Description = "Skj�k", StationType = "SATREF" },
                new Models.Station { StationName = "Skjerv�y", Description = "Skjerv�y", StationType = "SATREF" },
                new Models.Station { StationName = "Skollenborg", Description = "Skollenborg", StationType = "SATREF" },
                new Models.Station { StationName = "Skreia", Description = "Skreia", StationType = "SATREF" },
                new Models.Station { StationName = "Sleipnes", Description = "Sleipnes", StationType = "SATREF" },
                new Models.Station { StationName = "Sm�la", Description = "Sm�la", StationType = "SATREF" },
                new Models.Station { StationName = "Sm�rfjord", Description = "Sm�rfjord", StationType = "SATREF" },
                new Models.Station { StationName = "Sn�sa", Description = "Sn�sa", StationType = "SATREF" },
                new Models.Station { StationName = "Sommar�y", Description = "Sommar�y", StationType = "SATREF" },
                new Models.Station { StationName = "Sortland", Description = "Sortland", StationType = "SATREF" },
                new Models.Station { StationName = "Stad", Description = "Stad", StationType = "SATREF" },
                new Models.Station { StationName = "Stavanger", Description = "Stavanger", StationType = "SATREF" },
                new Models.Station { StationName = "station", Description = "Steigen", StationType = "SATREF" },
                new Models.Station { StationName = "Steinkjer", Description = "Steinkjer", StationType = "SATREF" },
                new Models.Station { StationName = "Stj�rdal", Description = "Stj�rdal", StationType = "SATREF" },
                new Models.Station { StationName = "Storforshei", Description = "Storforshei", StationType = "SATREF" },
                new Models.Station { StationName = "Stranda", Description = "Stranda", StationType = "SATREF" },
                new Models.Station { StationName = "St�ren", Description = "St�ren", StationType = "SATREF" },
                new Models.Station { StationName = "Sulitjelma", Description = "Sulitjelma", StationType = "SATREF" },
                new Models.Station { StationName = "Svarstad", Description = "Svarstad", StationType = "SATREF" },
                new Models.Station { StationName = "Sveindal", Description = "Sveindal", StationType = "SATREF" },
                new Models.Station { StationName = "Svolv�r", Description = "Svolv�r", StationType = "SATREF" },
                new Models.Station { StationName = "Tana", Description = "Tana", StationType = "SATREF" },
                new Models.Station { StationName = "Terr�k", Description = "Terr�k", StationType = "SATREF" },
                new Models.Station { StationName = "Tingvoll", Description = "Tingvoll", StationType = "SATREF" },
                new Models.Station { StationName = "Tjeldst�", Description = "Tjeldst�", StationType = "SATREF" },
                new Models.Station { StationName = "Tj�me", Description = "Tj�me", StationType = "SATREF" },
                new Models.Station { StationName = "Tonstad", Description = "Tonstad", StationType = "SATREF" },
                new Models.Station { StationName = "Toven", Description = "Toven", StationType = "SATREF" },
                new Models.Station { StationName = "Tregde", Description = "Tregde", StationType = "SATREF" },
                new Models.Station { StationName = "Trettnes", Description = "Trettnes", StationType = "SATREF" },
                new Models.Station { StationName = "Treungen", Description = "Treungen", StationType = "SATREF" },
                new Models.Station { StationName = "Trofors", Description = "Trofors", StationType = "SATREF" },
                new Models.Station { StationName = "Troms�", Description = "Troms�", StationType = "SATREF" },
                new Models.Station { StationName = "Trondheim", Description = "Trondheim", StationType = "SATREF" },
                new Models.Station { StationName = "Trysil", Description = "Trysil", StationType = "SATREF" },
                new Models.Station { StationName = "Tuddal", Description = "Tuddal", StationType = "SATREF" },
                new Models.Station { StationName = "Tvedestrand", Description = "Tvedestrand", StationType = "SATREF" },
                new Models.Station { StationName = "Tyin", Description = "Tyin", StationType = "SATREF" },
                new Models.Station { StationName = "Tysv�r", Description = "Tysv�r", StationType = "SATREF" },
                new Models.Station { StationName = "Ulefoss", Description = "Ulefoss", StationType = "SATREF" },
                new Models.Station { StationName = "Uls�k", Description = "Uls�k", StationType = "SATREF" },
                new Models.Station { StationName = "Vads�", Description = "Vads�", StationType = "SATREF" },
                new Models.Station { StationName = "Valle", Description = "Valle", StationType = "SATREF" },
                new Models.Station { StationName = "Vard�", Description = "Vard�", StationType = "SATREF" },
                new Models.Station { StationName = "Vega", Description = "Vega", StationType = "SATREF" },
                new Models.Station { StationName = "Veggli", Description = "Veggli", StationType = "SATREF" },
                new Models.Station { StationName = "Verma", Description = "Verma", StationType = "SATREF" },
                new Models.Station { StationName = "Vikersund", Description = "Vikersund", StationType = "SATREF" },
                new Models.Station { StationName = "Vikna", Description = "Vikna", StationType = "SATREF" },
                new Models.Station { StationName = "Vinstra", Description = "Vinstra", StationType = "SATREF" },
                new Models.Station { StationName = "Volda", Description = "Volda", StationType = "SATREF" },
                new Models.Station { StationName = "Voss", Description = "Voss", StationType = "SATREF" },
                new Models.Station { StationName = "�rnes", Description = "�rnes", StationType = "SATREF" },
                new Models.Station { StationName = "�sterb�", Description = "�sterb�", StationType = "SATREF" },
                new Models.Station { StationName = "�verbygd", Description = "�verbygd", StationType = "SATREF" },

                //Vannstandsm�ler  https://www.kartverket.no/globalassets/til-sjos/illustrasjoner/permanente-vannstandsmaalere-kartverket.pdf
                new Models.Station { StationName = "Viker", Description = "Viker", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Tregde", Description = "Tregde", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Helgeroa", Description = "Helgeroa", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Stavanger", Description = "Stavanger", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Oscarsborg", Description = "Oscarsborg", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Oslo", Description = "Oslo", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Bergen", Description = "Bergen", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "M�l�y", Description = "M�l�y", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "�lesund", Description = "�lesund", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Kristiansund", Description = "Kristiansund", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Trondheim", Description = "Trondheim", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Heimsj�", Description = "Heimsj�", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Mausund", Description = "Mausund", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "R�rvik", Description = "R�rvik", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Bod�", Description = "Bod�", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Kabelv�g", Description = "Kabelv�g", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Narvik", Description = "Narvik", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Harstad", Description = "Harstad", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Andenes", Description = "Andenes", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Troms�", Description = "Troms�", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Hammerfest", Description = "Hammerfest", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Vard�", Description = "Vard�", StationType = "Vannstandsm�ler" },
                new Models.Station { StationName = "Honningsv�g", Description = "Honningsv�g", StationType = "Vannstandsm�ler" }
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
            //    description = "Det offentlige kartgrunnlaget beskrives i plan- og bygningslovens paragraf 2-1 og kart- og planforskriften og skal v�re er en samling geografiske kvalitetsdata, s�kalt offentlige autoritative data. Disse skal v�re valgt ut og tilrettelagt for � v�re et egnet kunnskapsgrunnlag for de mest vesentlige behovene som f�lger av plan- og bygningsloven.",
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

