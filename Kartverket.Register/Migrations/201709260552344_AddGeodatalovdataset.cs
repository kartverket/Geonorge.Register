namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeodatalovdataset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GeodatalovDatasets",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Seoname = c.String(nullable: false),
                        Description = c.String(),
                        SubmitterId = c.Guid(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        DateSubmitted = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        StatusId = c.String(nullable: false, maxLength: 128),
                        RegisterId = c.Guid(nullable: false),
                        DateAccepted = c.DateTime(),
                        DateNotAccepted = c.DateTime(),
                        DateSuperseded = c.DateTime(),
                        DateRetired = c.DateTime(),
                        VersionNumber = c.Int(nullable: false),
                        VersionName = c.String(),
                        VersioningId = c.Guid(nullable: false),
                        Uuid = c.String(),
                        Notes = c.String(),
                        SpecificUsage = c.String(),
                        ProductSheetUrl = c.String(),
                        PresentationRulesUrl = c.String(),
                        ProductSpecificationUrl = c.String(),
                        MetadataUrl = c.String(),
                        DistributionFormat = c.String(),
                        DistributionUrl = c.String(),
                        DistributionArea = c.String(),
                        WmsUrl = c.String(),
                        ThemeGroupId = c.String(maxLength: 128),
                        DatasetThumbnail = c.String(),
                        DokStatusId = c.String(maxLength: 128),
                        DokStatusDateAccepted = c.DateTime(),
                        UuidService = c.String(),
                        InspireTheme = c.String(),
                        Dok = c.Boolean(nullable: false),
                        NationalDataset = c.Boolean(nullable: false),
                        Plan = c.Boolean(nullable: false),
                        Geodatalov = c.Boolean(nullable: false),
                        MetadataStatusId = c.Guid(nullable: false),
                        ProductSpesificationStatusId = c.Guid(nullable: false),
                        SosiDataStatusId = c.Guid(nullable: false),
                        GmlDataStatusId = c.Guid(nullable: false),
                        WmsStatusId = c.Guid(nullable: false),
                        WfsStatusId = c.Guid(nullable: false),
                        AtomFeedStatusId = c.Guid(nullable: false),
                        CommonStatusId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.MetadataStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.ProductSpesificationStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.SosiDataStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.GmlDataStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.WmsStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.WfsStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.AtomFeedStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.CommonStatusId)
                .Index(t => t.SubmitterId)
                .Index(t => t.OwnerId)
                .Index(t => t.StatusId)
                .Index(t => t.RegisterId)
                .Index(t => t.VersioningId)
                .Index(t => t.ThemeGroupId)
                .Index(t => t.DokStatusId)
                .Index(t => t.MetadataStatusId)
                .Index(t => t.ProductSpesificationStatusId)
                .Index(t => t.SosiDataStatusId)
                .Index(t => t.GmlDataStatusId)
                .Index(t => t.WmsStatusId)
                .Index(t => t.WfsStatusId)
                .Index(t => t.AtomFeedStatusId)
                .Index(t => t.CommonStatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GeodatalovDatasets", "CommonStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.GeodatalovDatasets", "AtomFeedStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.GeodatalovDatasets", "WfsStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.GeodatalovDatasets", "WmsStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.GeodatalovDatasets", "GmlDataStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.GeodatalovDatasets", "SosiDataStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.GeodatalovDatasets", "ProductSpesificationStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.GeodatalovDatasets", "MetadataStatusId", "dbo.DatasetDeliveries");
            DropIndex("dbo.GeodatalovDatasets", new[] { "CommonStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "AtomFeedStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "WfsStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "WmsStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "GmlDataStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "SosiDataStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "ProductSpesificationStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "MetadataStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "DokStatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "ThemeGroupId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "VersioningId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "RegisterId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "StatusId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "OwnerId" });
            DropIndex("dbo.GeodatalovDatasets", new[] { "SubmitterId" });
            DropTable("dbo.GeodatalovDatasets");
        }
    }
}
