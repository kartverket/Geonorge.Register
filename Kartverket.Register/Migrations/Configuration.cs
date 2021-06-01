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

