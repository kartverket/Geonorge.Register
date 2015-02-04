namespace Kartverket.Register.Migrations
{
    using Kartverket.Register.Models;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transferDokToRegister : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();
            Register dokregister = new Register
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
                ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3")

            };

            db.Registers.AddOrUpdate(
                dokregister
           );
            db.SaveChanges();

            Sql("ALTER table [kartverket_dok].[dbo].[Datasets] ADD PubliserGuid uniqueidentifier");
            Sql("Update [kartverket_dok].[dbo].[Datasets]  SET PubliserGuid = [kartverket_register].[dbo].[RegisterItems].systemID FROM [kartverket_register].[dbo].[RegisterItems] INNER JOIN [kartverket_dok].[dbo].[Datasets]  ON  [kartverket_dok].[dbo].[Datasets].Publisher = [kartverket_register].[dbo].[RegisterItems].name");
            Sql("INSERT INTO [kartverket_register].[dbo].[RegisterItems] (systemId, name, description, dateSubmitted, modified, dateAccepted, distributionFormat, distributionArea, metadataUuid, Discriminator, statusId, theme_value, registerId, datasetthumbnail, productsheet, presentationRules, productspesification, metadata, distributionUri, wmsUrl, submitterId, datasetownerId) SELECT NEWID(), dok.Name, Description, GETDATE(), GETDATE(), GETDATE(), DistributionFormat, DistributionArea, Uuid, 'Dataset', statusId, g.Name, 'CD429E8B-2533-45D8-BCAA-86BC2CBDD0DD',  ThumbnailUrl, ProductSheetUrl, PresentationRulesUrl, ProductSpecificationUrl, MetadataUrl, DistributionUrl, WmsUrl, PubliserGuid, PubliserGuid FROM [kartverket_dok].[dbo].[Datasets] dok, [kartverket_dok].[dbo].[ThemeGroups] g WHERE dok.ThemeGroupId = g.Id");
        }
        
        public override void Down()
        {
        }
    }
}
