namespace Kartverket.Register.Migrations
{
    using Kartverket.Register.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<Kartverket.Register.Models.RegisterDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;

        }

        protected override void Seed(Kartverket.Register.Models.RegisterDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            
            context.Statuses.AddOrUpdate(
                new Status { value = "Submitted", description = "Sendt inn", group = "suggested" },
                new Status { value = "NotAccepted", description = "Ikke godkjent", group = "suggested" },
                new Status { value = "Valid", description = "Gyldig", group = "current" },
                new Status { value = "Superseded", description = "Erstattet", group = "historical"},
                new Status { value = "Retired", description = "Utgått", group = "historical" }
            );

            context.DokStatuses.AddOrUpdate(
                new DokStatus { value = "Accepted", description = "Godkjent" },
                new DokStatus { value = "Candidate", description = "Kandidat" },
                new DokStatus { value = "InProgress", description = "I prosess" },
                new DokStatus { value = "Proposal", description = "Forslag" }
            );


            context.AccessTypes.AddOrUpdate(
                new accessType { accessLevel = 1, description = "Administrator" },
                new accessType { accessLevel = 2, description = "Editor" },
                new accessType { accessLevel = 3, description = "Alle" }
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
                //new Sorting { value = "name", description = "Navn"},
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
                new ContainedItemClass { value = "NameSpace", description = "Navnerom" }
            );

            context.Dimensions.AddOrUpdate(
                new Kartverket.Register.Models.Dimension { value = "horizontal", description = "Horisontalt" },
                new Kartverket.Register.Models.Dimension { value = "vertical", description = "Vertikalt" },
                new Kartverket.Register.Models.Dimension { value = "compound", description = "Sammensatt" }
            );


            //Register produktspesifikasjon = new Register 
            //{ 
            //    systemId = Guid.Parse("8E726684-F216-4497-91BE-6AB2496A84D3"), 
            //    dateSubmitted = DateTime.Now, 
            //    modified = DateTime.Now, 
            //    name = "Produktspesifikasjoner", 
            //    description = "Inneholder dokumenter for produktspesifikasjoner for kart- og geodata ", 
            //    containedItemClass = "Document", 
            //    managerId = null, 
            //    ownerId = null, 
            //    statusId = "Valid"
            //};
            //Register organisasjoner = new Register 
            //{ 
            //    systemId = Guid.Parse("FCB0685D-24EB-4156-9AC8-25FA30759094"), 
            //    dateSubmitted = DateTime.Now, 
            //    modified = DateTime.Now, 
            //    name = "Organisasjoner", 
            //    description = "Inneholder oversikt over organisasjoner og deres logo ", 
            //    containedItemClass = "Organization", 
            //    statusId = "Valid"
            //};
            //Register produktark = new Register 
            //{ 
            //    systemId = Guid.Parse("A42BC2B3-2314-4B7E-8007-71D9B10F2C04"), 
            //    dateSubmitted = DateTime.Now, 
            //    modified = DateTime.Now, 
            //    name = "Produktark", 
            //    description = "Inneholder organisasjoners produktark for kart og geodata", 
            //    containedItemClass = "Document", 
            //    statusId = "Valid"
            //};
            //Register kodeliste = new Register 
            //{ 
            //    systemId = Guid.Parse("9A46038D-16EE-4562-96D2-8F6304AAB689"), 
            //    dateSubmitted = DateTime.Now, 
            //    modified = DateTime.Now, 
            //    name = "Kodelister", 
            //    description = "Inneholder kodelister", 
            //    containedItemClass = "Register", 
            //    statusId = "Valid"
            //};
            //Register gmlApplikasjonsskjema = new Register 
            //{ 
            //    systemId = Guid.Parse("E43B65C6-452F-489D-A2E6-A5262E5740D8"), 
            //    dateSubmitted = DateTime.Now, 
            //    modified = DateTime.Now, 
            //    name = "GML applikasjonsskjema", 
            //    description = "Inneholder godkjente GML applikasjonsskjema", 
            //    containedItemClass = "Document",
            //    statusId = "Valid"
            //};
            //Register epsgKoder =  new Register 
            //{ 
            //    systemId = Guid.Parse("37B9DC41-D868-4CBC-84F9-39557041FB2C"), 
            //    dateSubmitted = DateTime.Now, 
            //    modified = DateTime.Now, 
            //    name = "EPSG koder", 
            //    description = "Inneholder oversikt over EPSG koder som benyttes i Norge Digitalt omtalt i rammeverksdokumentet ", 
            //    containedItemClass = "EPSG",
            //    statusId = "Valid"
            //};
            //Register tegneregler = new Register 
            //{ 
            //    systemId = Guid.Parse("5EACB130-D61F-469D-8454-E96943491BA0"), 
            //    dateSubmitted = DateTime.Now, 
            //    modified = DateTime.Now, 
            //    name = "Tegneregler", 
            //    description = "Inneholder dokumenter med tegneregler og kartografi", 
            //    containedItemClass = "Document",
            //    statusId = "Valid"
            //};

            //context.Registers.AddOrUpdate(
            //    produktspesifikasjon,
            //    organisasjoner,
            //    produktark,
            //    kodeliste,
            //    gmlApplikasjonsskjema,
            //    epsgKoder,
            //    tegneregler              
            //    //new Register { systemId = Guid.Parse("9A82A6B6-0069-45A4-8CA8-FBB789434F9A"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Navnerom", description = "Inneholder godkjente navnerom", containedItemClass = "Document" },
            //    //new Register { systemId = Guid.Parse("B4BA9E24-3717-482B-B9F5-7E349194D502"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "SOSI def", description = "Inneholder godkjente SOSI definisjonsfiler ", containedItemClass = "Document" },
            //    //new Register { systemId = Guid.Parse("8960B018-E6EC-4CF5-BDB2-0C2AE744B7E2"), dateSubmitted = DateTime.Now, modified = DateTime.Now, name = "Det offentlige kartgrunnlaget", description = "Inneholder oversikt over datasett som inngår i det offentlige kartgrunnlaget ", containedItemClass = "Dataset" },                            
            //);

            //context.Organizations.AddOrUpdate(
            //    new Organization 
            //    { 
            //        systemId = Guid.Parse("CEB5E459-853E-4E2F-BB22-39DC0C09CB7B"), 
            //        registerId = organisasjoner.systemId, 
            //        dateSubmitted = DateTime.Now, 
            //        modified = DateTime.Now, 
            //        number = "874783242", 
            //        name = "Kystverket", 
            //        logoFilename = "874783242_kystverketlogo.jpg" 
            //    },
            //    new Organization 
            //    { 
            //        systemId = Guid.Parse("D7142A92-418E-487E-A6FF-0E32C6AE31D8"), 
            //        registerId = organisasjoner.systemId, 
            //        dateSubmitted = DateTime.Now, 
            //        modified = DateTime.Now,
            //        number = "970188290", 
            //        name = "Norges geologiske undersøkelse", 
            //        logoFilename = "970188290_nguLogo.png" 
            //    },
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
            //    new Organization 
            //    { 
            //        systemId = Guid.Parse("DD34C8CA-5E5C-4CFD-9B25-AFEADD1BCBFF"), 
            //        registerId = organisasjoner.systemId,  
            //        dateSubmitted = DateTime.Now, 
            //        modified = DateTime.Now, 
            //        number = "999601391", 
            //        name = "Miljødirektoratet", 
            //        logoFilename = "999601391_miljodirlogo.JPG" }
            //);


            //context.Documents.AddOrUpdate(
            //    new Document
            //    {
            //        systemId = Guid.Parse("232ED533-7F2B-410F-B783-B3F910B61174"),
            //        registerId = produktspesifikasjon.systemId,
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "Produktspesifikasjon for løsmasser",
            //        description = "Datagrunnlaget for tema jordarter er basert på innholdet i kvartærgeologiske kart (løsmassekart), som foreligger analogt i flere målestokker (fra 1:20.000 til 1:250.000), og digital kartlegging.",
            //        documentUrl = "http://www.ngu.no/upload/Aktuelt/Produktspesifikasjon_LosmasseN250_N50.pdf",
            //        documentownerId = Guid.Parse("D7142A92-418E-487E-A6FF-0E32C6AE31D8")
            //    },
            //    new Document
            //    {
            //        systemId = Guid.Parse("3F9559B7-BAA5-4D15-9481-96389087B2B6"),
            //        registerId = produktspesifikasjon.systemId,
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "Produktspesifikasjon for primærdata kystkontur",
            //        description = "Primærdata kystkontur definerer grense mellom sjø og land til bruk i alle Statens kartverks produkter",
            //        documentUrl = "http://kartverket.no/Documents/Standard/SOSI-standarden%20del%201%20og%202/Produktspesifikasjoner%20-%20SOSI%20del%203/Produktspesifikasjon_Primrdatakyst_v1.pdf",
            //        documentownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3")
            //    },
            //    new Document
            //    {
            //        systemId = Guid.Parse("E608916B-4C63-4C48-B812-9EDD543873C7"),
            //        registerId = produktark.systemId,
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "Flomsoner",
            //        description = "Flomsoner viser arealer som oversvømmes ved ulike flomstørrelser (gjentaksintervall). Det blir utarbeidet flomsoner for 20-, 200- og 1000-årsflommene. I områder der klimaendringene gir en forventet økning i vannføringen på mer enn 20 %, utarbeides det flomsone for 200-årsflommen i år 2100.",
            //        documentUrl = "http://gis3.nve.no/metadata/produktark/produktark_flomsone.pdf",
            //        documentownerId = Guid.Parse("DD34C8CA-5E5C-4CFD-9B25-AFEADD1BCBFF")
            //    },
            //    new Document
            //    {
            //        systemId = Guid.Parse("05358181-C6F9-4A1D-9F32-F6F27D6726A5"),
            //        registerId = produktark.systemId,
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "Naturvernområder",
            //        description = "Datasettet inneholder verneområder og vernede enkeltobjekt i Norge, herunder Svalbard og Jan Mayen. Datasettet gir en oversikt over hvilke områder som er vernet etter følgende lover: naturmangfoldloven av 2009, biotopvern etter viltloven av 1981, naturvernloven av 1970, lov om naturvern av 1954, lov om Jan Mayen av 1930 og lov om naturfredning av 1910. I tillegg inneholder det områder vernet etter følgende lovverk på Svalbard: Svalbardloven av 1925 og Svalbardmiljøloven av 2002. Datasettet gir også tilgang til lovforskriften som gjelder for hvert enkelt vernevedtak.",
            //        documentUrl = "http://kartverket.no/Documents/Geonorge/Produktark%20og%20presentasjonsregler/Produktark_MDIR_Naturvernomr%C3%A5der_2014.pdf",
            //        documentownerId = Guid.Parse("DD34C8CA-5E5C-4CFD-9B25-AFEADD1BCBFF")
            //    },
            //    new Document
            //    {
            //        systemId = Guid.Parse("c8e542c1-09bc-4978-be69-8896396ab49e"),
            //        registerId = tegneregler.systemId,
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "N5 raster",
            //        description = "Informasjon lik kartene i Økonomisk kartverk. Symboler, linjer og tekst med samme form som på kartene. Informasjonen forefinnes som et rasterlag pr. kartblad.Datamengde ca. 1 Mb pr. kartblad i TIFFformat i s/h.Datamengde ca. 8 Mb pr. kartblad i PNGformat i farge.",
            //        documentUrl = "http://www.kartverket.no/Documents/N5-Tegnforklaring.pdf",
            //        documentownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3")
            //    },

            //    new Document
            //    {
            //        systemId = Guid.Parse("a7cdf9b1-af3e-40e0-a229-697e3aa9c6a5"),
            //        registerId = gmlApplikasjonsskjema.systemId,
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "Stedsnavn",
            //        description = "...",
            //        documentUrl = "http://skjema.geonorge.no/SOSI/produktspesifikasjon/Stedsnavn/4.5/Stedsnavn.xsd",
            //        documentownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3")
            //    },

            //    new Document
            //    {
            //        systemId = Guid.Parse("6be71844-0866-49fc-8c07-258637d035e0"),
            //        registerId = gmlApplikasjonsskjema.systemId,
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "AdmEnheterNorge",
            //        description = "...",
            //        documentUrl = "http://skjema.geonorge.no/SOSI/produktspesifikasjon/AdmEnheterNorge/3.0/AdmEnheterNorge.xsd",
            //        documentownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3")
            //    }
            //);


            //context.EPSGs.AddOrUpdate(
            //    new EPSG
            //    {
            //        systemId = Guid.Parse("A272B7D3-EBCD-449B-8759-0A3CD46AF15F"),
            //        registerId = Guid.Parse("37B9DC41-D868-4CBC-84F9-39557041FB2C"),
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "UTM sone 29,basert på EUREF89 (ETRS89/UTM), 2d",
            //        description = " ... ",
            //        epsgcode = "EPSG::25829",
            //        sosiReferencesystem = "19",
            //        submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //        externalReference = "http://spatialreference.org/ref/epsg/etrs89-utm-zone-29n/"
            //    },
            //    new EPSG
            //    {
            //        systemId = Guid.Parse("C6EA8105-F5F5-4FE3-A3DB-BA8B2BD37AFB"),
            //        registerId = Guid.Parse("37B9DC41-D868-4CBC-84F9-39557041FB2C"),
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "UTM sone 30,basert på EUREF89 (ETRS89/UTM), 2d ",
            //        description = " ... ",
            //        epsgcode = "EPSG::25830",
            //        sosiReferencesystem = "20",
            //        submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //        externalReference = "http://spatialreference.org/ref/epsg/etrs89-utm-zone-30n/"
            //    },
            //    new EPSG
            //    {
            //        systemId = Guid.Parse("BBF6DFF6-6AF8-4EA2-BD7F-FD83BF0F07E4"),
            //        registerId = Guid.Parse("37B9DC41-D868-4CBC-84F9-39557041FB2C"),
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "UTM sone 29,basert på EUREF89 (ETRS89/UTM), 2d ",
            //        description = " ... ",
            //        epsgcode = "EPSG::25829",
            //        sosiReferencesystem = "19",
            //        submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //        externalReference = "http://spatialreference.org/ref/epsg/etrs89-utm-zone-29n/"
            //    }                  
            //);

            //context.Registers.AddOrUpdate(
            //    new Register
            //    {
            //        systemId = Guid.NewGuid(),
            //        registerId = Guid.Parse("37B9DC41-D868-4CBC-84F9-39557041FB2C"),
            //        dateSubmitted = DateTime.Now,
            //        modified = DateTime.Now,
            //        name = "UTM sone 29,basert på EUREF89 (ETRS89/UTM), 2d",
            //        description = " ... ",
            //        epsg = "EPSG::25829",
            //        sosiReferencesystem = "19",
            //        submitterId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
            //        externalReference = "http://spatialreference.org/ref/epsg/etrs89-utm-zone-29n/"
            //    }
            //);



            //Lage register for veiledningsdokumenter
            context.Registers.AddOrUpdate(
                new Register
                {
                    systemId = Guid.Parse("b2e5f822-994f-47f5-ac52-cd4153d55197"),
                    dateSubmitted = DateTime.Now,
                    modified = DateTime.Now,
                    name = "Veiledningsdokumenter",
                    //description = "Inneholder veiledningsdokumenter",
                    containedItemClass = "Register",
                    //statusId = "Submitted",
                    seoname = "veiledningsdokumenter",
                    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                    accessId = 1
                }

                //new Register
                //{
                //    systemId = Guid.Parse("29729311-ba8e-46b2-a140-f3398cc735a0"),
                //    dateSubmitted = DateTime.Now,
                //    modified = DateTime.Now,
                //    name = "Veileder",
                //    description = "Inneholder dokumenter av typen veileder",
                //    containedItemClass = "Document",
                //    statusId = "Submitted",
                //    parentRegisterId = Guid.Parse("b2e5f822-994f-47f5-ac52-cd4153d55197"),
                //    seoname = "veileder",
                //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                //    accessId = 1
                //},

                //new Register
                //{
                //    systemId = Guid.Parse("0a4fb286-30c7-48dd-ad52-feb554c1ae3e"),
                //    dateSubmitted = DateTime.Now,
                //    modified = DateTime.Now,
                //    name = "Standard",
                //    description = "inne standard veiledningsdokumenter",
                //    containedItemClass = "Document",
                //    statusId = "Submitted",
                //    seoname = "standard",
                //    parentRegisterId = Guid.Parse("b2e5f822-994f-47f5-ac52-cd4153d55197"),
                //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                //    accessId = 1
                //},

                //new Register
                //{
                //    systemId = Guid.Parse("46d50753-1d83-4901-87cf-7e7e053fbc2e"),
                //    dateSubmitted = DateTime.Now,
                //    modified = DateTime.Now,
                //    name = "Teknisk dokumentasjon",
                //    description = "Inneholder dokumenter med teknisk dokumentasjon",
                //    containedItemClass = "Document",
                //    statusId = "Submitted",
                //    seoname = "teknisk-dokumentasjon",
                //    parentRegisterId = Guid.Parse("b2e5f822-994f-47f5-ac52-cd4153d55197"),
                //    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                //    accessId = 1
                //}

                );


            //FixSubmitter
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET submitterId = '10087020-F17C-45E1-8542-02ACBCF3D8A3' WHERE  (documentownerId IS NULL)");

            //Add dok-register
            context.Registers.AddOrUpdate(
                new Register
                {
                    systemId = Guid.Parse("CD429E8B-2533-45D8-BCAA-86BC2CBDD0DD"),
                    dateSubmitted = DateTime.Now,
                    modified = DateTime.Now,
                    dateAccepted = DateTime.Now,
                    name = "Det offentlige kartgrunnlaget",
                    description = "Det offentlige kartgrunnlaget beskrives i plan- og bygningslovens paragraf 2-1 og kart- og planforskriften og skal være er en samling geografiske kvalitetsdata, såkalt offentlige autoritative data. Disse skal være valgt ut og tilrettelagt for å være et egnet kunnskapsgrunnlag for de mest vesentlige behovene som følger av plan- og bygningsloven.",
                    containedItemClass = "Dataset",
                    statusId = "Valid",
                    seoname = "det-offentlige-kartgrunnlaget",
                    managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                    ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                    accessId = 1
                }
            );

            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET statusId = 'Submitted' WHERE  (statusId IS NULL)");

            //UpdateVersionNumber
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET versionNumber = 1 WHERE  (versionNumber=0)");
            //UpdateRegisterVersionNumber
            context.Database.ExecuteSqlCommand("UPDATE Registers SET versionNumber = 1 WHERE  (versionNumber=0)");
            //AddItemsToVersionTable
            context.Database.ExecuteSqlCommand("INSERT INTO Versions (systemId, currentVersion, lastVersionNumber, containedItemClass) SELECT NEWID() as systemId, systemId as currentVersion, versionNumber as lastVersionNumber, containedItemClass as containedItemClass FROM Registers WHERE versioningId IS NULL");
            context.Database.ExecuteSqlCommand("INSERT INTO Versions (systemId, currentVersion, lastVersionNumber, containedItemClass) SELECT NEWID() as systemId, systemId as currentVersion, versionNumber as lastVersionNumber, Discriminator as containedItemClass FROM RegisterItems WHERE versioningId IS NULL");

            //UpdateRegisterItemsVersioningId
            var queryResultsVersions = from r in context.Versions
                                       select r;

            List<Kartverket.Register.Models.Version> versions = queryResultsVersions.ToList();

            var queryResultsRegisterItems = from r in context.RegisterItems
                                            select r;
            List<RegisterItem> registerItems = queryResultsRegisterItems.ToList();


            foreach (Kartverket.Register.Models.Version v in versions)
            {
                foreach (RegisterItem ri in registerItems)
                {
                    if (v.currentVersion == ri.systemId)
                    {
                        ri.versioningId = v.systemId;
                        string versjonsID = ri.versioningId.ToString();
                        string systemID = ri.systemId.ToString();

                        context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET versioningId = '" + versjonsID + "' WHERE (systemId = '" + systemID + "')");
                    }
                }
            }



            //UpdateRegistersVersioningId

            var queryResultsVersions2 = from r in context.Versions
                                        select r;

            List<Kartverket.Register.Models.Version> versions2 = queryResultsVersions2.ToList();

            var queryResultsRegisterItems2 = from r in context.Registers
                                             select r;
            List<Kartverket.Register.Models.Register> registers = queryResultsRegisterItems2.ToList();


            foreach (Kartverket.Register.Models.Version v in versions)
            {
                foreach (Kartverket.Register.Models.Register r in registers)
                {
                    if (v.currentVersion == r.systemId)
                    {
                        r.versioningId = v.systemId;
                        string versjonsID = r.versioningId.ToString();
                        string systemID = r.systemId.ToString();

                        context.Database.ExecuteSqlCommand("UPDATE Registers SET versioningId = '" + versjonsID + "' WHERE (systemId = '" + systemID + "')");
                    }
                }
            }
            
            //UpdateAccessId
            context.Database.ExecuteSqlCommand("UPDATE Registers SET accessId = 1  WHERE (name = 'Organisasjoner' OR name = 'EPSG koder')");

            //UpdateLastVersionNumber
            context.Database.ExecuteSqlCommand("UPDATE Versions SET lastVersionNumber = 1  WHERE (lastVersionNumber = 0)");

            //UpdateAccessId
            context.Database.ExecuteSqlCommand("UPDATE Registers SET accessId = 2  WHERE (AccessId IS NULL)");

            //UpdateDokStatusId
            context.Database.ExecuteSqlCommand("UPDATE RegisterItems SET dokStatusId = 'Proposal' WHERE (dokStatusId is NULL AND Discriminator = 'Dataset')");

        }
    }
}

