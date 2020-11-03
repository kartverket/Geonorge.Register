namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMareanoStatusRegistry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MareanoDatasets",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        NameEnglish = c.String(),
                        Seoname = c.String(nullable: false),
                        Description = c.String(),
                        DescriptionEnglish = c.String(),
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
                        SpecificUsageEnglish = c.String(),
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
                        InspireTheme = c.Boolean(nullable: false),
                        Dok = c.Boolean(nullable: false),
                        NationalDataset = c.Boolean(nullable: false),
                        Plan = c.Boolean(nullable: false),
                        Geodatalov = c.Boolean(nullable: false),
                        Mareano = c.Boolean(nullable: false),
                        EcologicalBaseMap = c.Boolean(nullable: false),
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
                .ForeignKey("dbo.RegisterItems", t => t.SubmitterId, cascadeDelete: true)
                .ForeignKey("dbo.RegisterItems", t => t.OwnerId, cascadeDelete: true)
                .ForeignKey("dbo.Status", t => t.StatusId, cascadeDelete: true)
                .ForeignKey("dbo.Registers", t => t.RegisterId, cascadeDelete: true)
                .ForeignKey("dbo.Versions", t => t.VersioningId, cascadeDelete: true)
                .ForeignKey("dbo.DOKThemes", t => t.ThemeGroupId)
                .ForeignKey("dbo.DokStatus", t => t.DokStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.MetadataStatusId, cascadeDelete: true)
                .ForeignKey("dbo.DatasetDeliveries", t => t.ProductSpesificationStatusId, cascadeDelete: true)
                .ForeignKey("dbo.DatasetDeliveries", t => t.SosiDataStatusId, cascadeDelete: true)
                .ForeignKey("dbo.DatasetDeliveries", t => t.GmlDataStatusId, cascadeDelete: true)
                .ForeignKey("dbo.DatasetDeliveries", t => t.WmsStatusId, cascadeDelete: true)
                .ForeignKey("dbo.DatasetDeliveries", t => t.WfsStatusId, cascadeDelete: true)
                .ForeignKey("dbo.DatasetDeliveries", t => t.AtomFeedStatusId, cascadeDelete: true)
                .ForeignKey("dbo.DatasetDeliveries", t => t.CommonStatusId, cascadeDelete: true)
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
            DropForeignKey("dbo.MareanoDatasets", "CommonStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "AtomFeedStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "WfsStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "WmsStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "GmlDataStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "SosiDataStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "ProductSpesificationStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "MetadataStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "DokStatusId", "dbo.DokStatus");
            DropForeignKey("dbo.MareanoDatasets", "ThemeGroupId", "dbo.DOKThemes");
            DropForeignKey("dbo.MareanoDatasets", "VersioningId", "dbo.Versions");
            DropForeignKey("dbo.MareanoDatasets", "RegisterId", "dbo.Registers");
            DropForeignKey("dbo.MareanoDatasets", "StatusId", "dbo.Status");
            DropForeignKey("dbo.MareanoDatasets", "OwnerId", "dbo.RegisterItems");
            DropForeignKey("dbo.MareanoDatasets", "SubmitterId", "dbo.RegisterItems");
            DropIndex("dbo.MareanoDatasets", new[] { "CommonStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "AtomFeedStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "WfsStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "WmsStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "GmlDataStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "SosiDataStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "ProductSpesificationStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "MetadataStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "DokStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "ThemeGroupId" });
            DropIndex("dbo.MareanoDatasets", new[] { "VersioningId" });
            DropIndex("dbo.MareanoDatasets", new[] { "RegisterId" });
            DropIndex("dbo.MareanoDatasets", new[] { "StatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "OwnerId" });
            DropIndex("dbo.MareanoDatasets", new[] { "SubmitterId" });
            DropTable("dbo.MareanoDatasets");
        }
    }
}
