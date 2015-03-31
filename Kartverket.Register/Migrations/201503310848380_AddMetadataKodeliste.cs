namespace Kartverket.Register.Migrations
{
    using Kartverket.Register.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetadataKodeliste : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();


            Register dokregister = new Register
            {
                systemId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB689"),
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Metadata kodelister",
                description = "Inneholder kodelister",
                containedItemClass = "Register",
                statusId = "Valid",
                seoname = "metadata-kodelister",
                managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3")

            };

            db.Registers.AddOrUpdate(
               dokregister
          );
            db.SaveChanges();

            Guid topicId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB100");
            Register topic = new Register
            {
                systemId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Tematisk hovedkategori",
                containedItemClass = "CodelistValue",
                statusId = "Valid",
                seoname = "tematisk-hovedkategori",
                managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                parentRegisterId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB689")

            };
            db.Registers.AddOrUpdate(
              topic);
            db.SaveChanges();


            Guid topicSysId1 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB100");
            RegisterItem topics1 = new CodelistValue
            {
                systemId = topicSysId1,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Landbruk og havbruk",
                description = "Landbruk og havbruk",
                seoname = "landbruk-og-havbruk",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "farming"

            };

            db.RegisterItems.AddOrUpdate(
              topics1);
            db.SaveChanges();

            Guid topicSysId2 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB101");
            RegisterItem topics2 = new CodelistValue
            {
                systemId = topicSysId2,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Biologisk mangfold",
                description = "Biologisk mangfold",
                seoname = "biologisk-mangfold",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "biota"

            };

            db.RegisterItems.AddOrUpdate(
              topics2);
            db.SaveChanges();


            Guid topicSysId3 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB102");
            RegisterItem topics3 = new CodelistValue
            {
                systemId = topicSysId3,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Administrative grenser",
                description = "Administrative grenser",
                seoname = "administrative-grenser",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "boundaries"

            };

            db.RegisterItems.AddOrUpdate(
              topics3);
            db.SaveChanges();


            Guid topicSysId4 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB103");
            RegisterItem topics4 = new CodelistValue
            {
                systemId = topicSysId4,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Klima, meteorologi og atomsfære",
                description = "Klima, meteorologi og atomsfære",
                seoname = "Klima-meteorologi-og-atomsfære",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "climatologyMeteorologyAtmosphere"

            };

            db.RegisterItems.AddOrUpdate(
              topics4);
            db.SaveChanges();

            Guid topicSysId5 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB104");
            RegisterItem topics5 = new CodelistValue
            {
                systemId = topicSysId5,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Økonomi",
                description = "Økonomi",
                seoname = "oekonomi",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "economy"

            };

            db.RegisterItems.AddOrUpdate(
              topics5);
            db.SaveChanges();


            Guid topicSysId6 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB105");
            RegisterItem topics6 = new CodelistValue
            {
                systemId = topicSysId6,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Høydedata",
                description = "Høydedata",
                seoname = "hoeydedata",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "elevation"

            };

            db.RegisterItems.AddOrUpdate(
              topics6);
            db.SaveChanges();


            Guid topicSysId7 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB106");
            RegisterItem topics7 = new CodelistValue
            {
                systemId = topicSysId7,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Miljødata",
                description = "Miljødata",
                seoname = "miljoedata",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "environment"

            };

            db.RegisterItems.AddOrUpdate(
              topics7);
            db.SaveChanges();

            db.RegisterItems.AddOrUpdate(
              topics6);
            db.SaveChanges();


            Guid topicSysId8 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB107");
            RegisterItem topics8 = new CodelistValue
            {
                systemId = topicSysId8,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Geovitenskapelig informasjon",
                description = "Geovitenskapelig informasjon",
                seoname = "geovitenskapelig-informasjon",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "geoscientificInformation"

            };

            db.RegisterItems.AddOrUpdate(
              topics8);
            db.SaveChanges();

            Guid topicSysId9 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB108");
            RegisterItem topics9 = new CodelistValue
            {
                systemId = topicSysId9,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Helse",
                description = "Helse",
                seoname = "helse",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "health"

            };

            db.RegisterItems.AddOrUpdate(
              topics9);
            db.SaveChanges();

            Guid topicSysId10 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB109");
            RegisterItem topics10 = new CodelistValue
            {
                systemId = topicSysId10,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Basisdata",
                description = "Basisdata",
                seoname = "basisdata",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "imageryBaseMapsEarthCover"

            };

            db.RegisterItems.AddOrUpdate(
              topics10);
            db.SaveChanges();

            Guid topicSysId11 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB110");
            RegisterItem topics11 = new CodelistValue
            {
                systemId = topicSysId11,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Militære data",
                description = "Militære data",
                seoname = "militaere-data",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "intelligenceMilitary"

            };

            db.RegisterItems.AddOrUpdate(
              topics11);
            db.SaveChanges();


            Guid topicSysId12 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB111");
            RegisterItem topics12 = new CodelistValue
            {
                systemId = topicSysId12,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Innsjø og vassdrag",
                description = "Innsjø og vassdrag",
                seoname = "innsjoe-og-vassdrag",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "inlandWaters"

            };

            db.RegisterItems.AddOrUpdate(
              topics12);
            db.SaveChanges();

            Guid topicSysId13 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB112");
            RegisterItem topics13 = new CodelistValue
            {
                systemId = topicSysId13,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Posisjonsdata",
                description = "Posisjonsdata",
                seoname = "posisjonsdata",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "location"

            };

            db.RegisterItems.AddOrUpdate(
              topics13);
            db.SaveChanges();

            Guid topicSysId14 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB113");
            RegisterItem topics14 = new CodelistValue
            {
                systemId = topicSysId14,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Kyst og sjø",
                description = "Kyst og sjø",
                seoname = "kyst-og-sjoe",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "oceans"

            };

            db.RegisterItems.AddOrUpdate(
              topics14);
            db.SaveChanges();

            Guid topicSysId15 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB114");
            RegisterItem topics15 = new CodelistValue
            {
                systemId = topicSysId15,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Plan og eiendom",
                description = "Plan og eiendom",
                seoname = "plan-og-eiendom",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "planningCadastre"

            };

            db.RegisterItems.AddOrUpdate(
              topics15);
            db.SaveChanges();

            Guid topicSysId16 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB115");
            RegisterItem topics16 = new CodelistValue
            {
                systemId = topicSysId16,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Samfunn",
                description = "Samfunn",
                seoname = "samfunn",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "society"

            };

            db.RegisterItems.AddOrUpdate(
              topics16);
            db.SaveChanges();

            Guid topicSysId17 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB116");
            RegisterItem topics17 = new CodelistValue
            {
                systemId = topicSysId17,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Konstruksjoner",
                description = "Konstruksjoner",
                seoname = "konstruksjoner",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "structure"

            };

            db.RegisterItems.AddOrUpdate(
              topics17);
            db.SaveChanges();

            Guid topicSysId18 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB117");
            RegisterItem topics18 = new CodelistValue
            {
                systemId = topicSysId18,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Transport",
                description = "Transport",
                seoname = "transport",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "transportation"

            };

            db.RegisterItems.AddOrUpdate(
              topics18);
            db.SaveChanges();

            Guid topicSysId19 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB118");
            RegisterItem topics19 = new CodelistValue
            {
                systemId = topicSysId19,
                registerId = topicId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Ledningsinformasjon",
                description = "Ledningsinformasjon",
                seoname = "ledningsinformasjon",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "utilitiesCommunication"

            };

            db.RegisterItems.AddOrUpdate(
              topics19);
            db.SaveChanges();


            Guid UnitsOfDistributionId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB119");
            Register UnitsOfDistribution = new Register
            {
                systemId = UnitsOfDistributionId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Geografisk distribusjonsinndeling",
                containedItemClass = "CodelistValue",
                statusId = "Valid",
                seoname = "geografisk-distribusjonsinndeling",
                managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                parentRegisterId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB689")

            };
            db.Registers.AddOrUpdate(
              UnitsOfDistribution);
            db.SaveChanges();

            Guid UnitsOfDistributionId1 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB120");
            RegisterItem UnitsOfDistribution1 = new CodelistValue
            {
                systemId = UnitsOfDistributionId1,
                registerId = UnitsOfDistributionId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Kommunevis",
                description = "Kommunevis",
                seoname = "kommunevis",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "kommunevis"

            };

            db.RegisterItems.AddOrUpdate(
           UnitsOfDistribution1);
            db.SaveChanges();


            Guid UnitsOfDistributionId2 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB121");
            RegisterItem UnitsOfDistribution2 = new CodelistValue
            {
                systemId = UnitsOfDistributionId2,
                registerId = UnitsOfDistributionId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Fylkesvis",
                description = "Fylkesvis",
                seoname = "fylkesvis",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "fylkesvis"

            };

            db.RegisterItems.AddOrUpdate(
           UnitsOfDistribution2);
            db.SaveChanges();

            Guid UnitsOfDistributionId3 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB122");
            RegisterItem UnitsOfDistribution3 = new CodelistValue
            {
                systemId = UnitsOfDistributionId3,
                registerId = UnitsOfDistributionId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Landsfiler",
                description = "Landsfiler",
                seoname = "landsfiler",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "landsfiler"

            };

            db.RegisterItems.AddOrUpdate(
           UnitsOfDistribution3);
            db.SaveChanges();

            Guid UnitsOfDistributionId4 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB123");
            RegisterItem UnitsOfDistribution4 = new CodelistValue
            {
                systemId = UnitsOfDistributionId4,
                registerId = UnitsOfDistributionId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Regional inndeling",
                description = "Regional inndeling",
                seoname = "regional-inndeling",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "regional inndeling"

            };

            db.RegisterItems.AddOrUpdate(
           UnitsOfDistribution4);
            db.SaveChanges();

            Guid UnitsOfDistributionId5 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB124");
            RegisterItem UnitsOfDistribution5 = new CodelistValue
            {
                systemId = UnitsOfDistributionId5,
                registerId = UnitsOfDistributionId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Kartbladvis",
                description = "Kartbladvis",
                seoname = "kartbladvis",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "kartbladvis"

            };

            db.RegisterItems.AddOrUpdate(
           UnitsOfDistribution5);
            db.SaveChanges();

            Guid MaintenanceFrequencyId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB124");
            Register MaintenanceFrequency = new Register
            {
                systemId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Oppdateringshyppighet område",
                containedItemClass = "CodelistValue",
                statusId = "Valid",
                seoname = "oppdateringshyppighet-omraade",
                managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                parentRegisterId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB689")

            };
            db.Registers.AddOrUpdate(
              MaintenanceFrequency);
            db.SaveChanges();

            Guid MaintenanceFrequencyId1 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB125");
            RegisterItem MaintenanceFrequency1 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId1,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Kontinuerlig",
                description = "Kontinuerlig",
                seoname = "kontinuerlig",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "continual"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency1);
            db.SaveChanges();

            Guid MaintenanceFrequencyId2 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB126");
            RegisterItem MaintenanceFrequency2 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId2,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Daglig",
                description = "Daglig",
                seoname = "daglig",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "daily"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency2);
            db.SaveChanges();

            Guid MaintenanceFrequencyId3 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB127");
            RegisterItem MaintenanceFrequency3 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId3,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Ukentlig",
                description = "Ukentlig",
                seoname = "ukentlig",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "weekly"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency3);
            db.SaveChanges();

            Guid MaintenanceFrequencyId4 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB128");
            RegisterItem MaintenanceFrequency4 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId4,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Annenhver uke",
                description = "Annenhver uke",
                seoname = "annenhver-uke",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "fortnightly"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency4);
            db.SaveChanges();

            Guid MaintenanceFrequencyId5 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB129");
            RegisterItem MaintenanceFrequency5 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId4,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Månedlig",
                description = "Månedlig",
                seoname = "maanedlig",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "monthly"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency5);
            db.SaveChanges();

            Guid MaintenanceFrequencyId6 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB130");
            RegisterItem MaintenanceFrequency6 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId6,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Hvert kvartal",
                description = "Hvert kvartal",
                seoname = "hvert-kvartal",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "quarterly"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency6);
            db.SaveChanges();

            Guid MaintenanceFrequencyId7 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB131");
            RegisterItem MaintenanceFrequency7 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId7,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Hvert halvår",
                description = "Hvert halvår",
                seoname = "hvert-halvaar",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "biannually"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency7);
            db.SaveChanges();

            Guid MaintenanceFrequencyId8 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB132");
            RegisterItem MaintenanceFrequency8 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId8,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Årlig",
                description = "Årlig",
                seoname = "aarlig",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "annually"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency8);
            db.SaveChanges();

            Guid MaintenanceFrequencyId9 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB133");
            RegisterItem MaintenanceFrequency9 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId9,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Etter behov",
                description = "Etter behov",
                seoname = "etter-behov",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "asNeeded"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency9);
            db.SaveChanges();

            Guid MaintenanceFrequencyId10 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB134");
            RegisterItem MaintenanceFrequency10 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId10,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Ujevnt",
                description = "Ujevnt",
                seoname = "ujevnt",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "irregular"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency10);
            db.SaveChanges();

            Guid MaintenanceFrequencyId11 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB135");
            RegisterItem MaintenanceFrequency11 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId11,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Ikke planlagt",
                description = "Ikke planlagt",
                seoname = "ikke-planlagt",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "notPlanned"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency11);
            db.SaveChanges();

            Guid MaintenanceFrequencyId12 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB136");
            RegisterItem MaintenanceFrequency12 = new CodelistValue
            {
                systemId = MaintenanceFrequencyId12,
                registerId = MaintenanceFrequencyId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Ukjent",
                description = "Ukjent",
                seoname = "ukjent",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "unknown"

            };

            db.RegisterItems.AddOrUpdate(
           MaintenanceFrequency12);
            db.SaveChanges();


            Guid StatusId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB137");
            Register Status = new Register
            {
                systemId = StatusId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Status",
                containedItemClass = "CodelistValue",
                statusId = "Valid",
                seoname = "status",
                managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                parentRegisterId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB689")

            };
            db.Registers.AddOrUpdate(
              Status);
            db.SaveChanges();

            Guid StatusId1 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB138");
            RegisterItem Status1 = new CodelistValue
            {
                systemId = StatusId1,
                registerId = StatusId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Fullført",
                description = "Fullført",
                seoname = "fullfoert",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "completed"

            };

            db.RegisterItems.AddOrUpdate(
           Status1);
            db.SaveChanges();

            Guid StatusId2 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB139");
            RegisterItem Status2 = new CodelistValue
            {
                systemId = StatusId2,
                registerId = StatusId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Arkivert",
                description = "Arkivert",
                seoname = "arkivert",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "historicalArchive"

            };

            db.RegisterItems.AddOrUpdate(
           Status2);
            db.SaveChanges();

            Guid StatusId3 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB140");
            RegisterItem Status3 = new CodelistValue
            {
                systemId = StatusId3,
                registerId = StatusId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Utdatert",
                description = "Utdatert",
                seoname = "utdatert",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "obsolete"

            };

            db.RegisterItems.AddOrUpdate(
           Status3);
            db.SaveChanges();

            Guid StatusId4 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB141");
            RegisterItem Status4 = new CodelistValue
            {
                systemId = StatusId4,
                registerId = StatusId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Kontinuerlig oppdatert",
                description = "Kontinuerlig oppdatert",
                seoname = "kontinuerlig-oppdatert",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "onGoing"

            };

            db.RegisterItems.AddOrUpdate(
           Status4);
            db.SaveChanges();

            Guid StatusId5 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB142");
            RegisterItem Status5 = new CodelistValue
            {
                systemId = StatusId5,
                registerId = StatusId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Planlagt",
                description = "Planlagt",
                seoname = "planlagt",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "planned"

            };

            db.RegisterItems.AddOrUpdate(
           Status5);
            db.SaveChanges();

            Guid StatusId6 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB143");
            RegisterItem Status6 = new CodelistValue
            {
                systemId = StatusId6,
                registerId = StatusId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Må oppdateres",
                description = "Må oppdateres",
                seoname = "maa-oppdateres",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "required"

            };

            db.RegisterItems.AddOrUpdate(
           Status6);
            db.SaveChanges();

            Guid StatusId7 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB144");
            RegisterItem Status7 = new CodelistValue
            {
                systemId = StatusId7,
                registerId = StatusId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Under arbeid",
                description = "Under arbeid",
                seoname = "under arbeid",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "underDevelopment"

            };

            db.RegisterItems.AddOrUpdate(
           Status7);
            db.SaveChanges();



            Guid ClassificationId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB145");
            Register Classification = new Register
            {
                systemId = ClassificationId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Sikkerhetsnivå",
                containedItemClass = "CodelistValue",
                statusId = "Valid",
                seoname = "sikkerhetsnivaa",
                managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                parentRegisterId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB689")

            };
            db.Registers.AddOrUpdate(
              Classification);
            db.SaveChanges();

            Guid ClassificationId1 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB146");
            RegisterItem Classification1 = new CodelistValue
            {
                systemId = ClassificationId1,
                registerId = ClassificationId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Ugradert",
                description = "Ugradert",
                seoname = "ugradert",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "unclassified"

            };

            db.RegisterItems.AddOrUpdate(
           Classification1);
            db.SaveChanges();

            Guid ClassificationId2 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB147");
            RegisterItem Classification2 = new CodelistValue
            {
                systemId = ClassificationId2,
                registerId = ClassificationId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Begrenset",
                description = "Begrenset",
                seoname = "begrenset",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "restricted"

            };

            db.RegisterItems.AddOrUpdate(
           Classification2);
            db.SaveChanges();

            Guid ClassificationId3 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB148");
            RegisterItem Classification3 = new CodelistValue
            {
                systemId = ClassificationId3,
                registerId = ClassificationId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Konfidensielt",
                description = "Konfidensielt",
                seoname = "konfidensielt",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "confidential"

            };

            db.RegisterItems.AddOrUpdate(
           Classification3);
            db.SaveChanges();

            Guid ClassificationId4 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB149");
            RegisterItem Classification4 = new CodelistValue
            {
                systemId = ClassificationId4,
                registerId = ClassificationId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Hemmelig",
                description = "Hemmelig",
                seoname = "hemmelig",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "secret"

            };

            db.RegisterItems.AddOrUpdate(
           Classification4);
            db.SaveChanges();

            Guid ClassificationId5 = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB150");
            RegisterItem Classification5 = new CodelistValue
            {
                systemId = ClassificationId5,
                registerId = ClassificationId,
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Topp hemmelig",
                description = "Topp hemmelig",
                seoname = "topp-hemmelig",
                statusId = "Valid",
                submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                value = "topSecret"

            };

            db.RegisterItems.AddOrUpdate(
           Classification5);
            db.SaveChanges();

        }
        
        public override void Down()
        {
        }
    }
}
