namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireDatasetTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InspireDatasets",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        InspireDeliveryMetadataStatusId = c.String(maxLength: 128),
                        InspireDeliveryMetadataStatusNote = c.String(),
                        InspireDeliveryMetadataStatusAutoUpdate = c.Boolean(nullable: false),
                        InspireDeliveryMetadataServiceStatusId = c.String(maxLength: 128),
                        InspireDeliveryMetadataServiceStatusNote = c.String(),
                        InspireDeliveryMetadataServiceStatusAutoUpdate = c.Boolean(nullable: false),
                        InspireDeliveryDistributionStatusId = c.String(maxLength: 128),
                        InspireDeliveryDistributionStatusNote = c.String(),
                        InspireDeliveryDistributionStatusAutoUpdate = c.Boolean(nullable: false),
                        InspireDeliveryWmsStatusId = c.String(maxLength: 128),
                        InspireDeliveryWmsStatusNote = c.String(),
                        InspireDeliveryWmsStatusAutoUpdate = c.Boolean(nullable: false),
                        InspireDeliveryWfsStatusId = c.String(maxLength: 128),
                        InspireDeliveryWfsStatusNote = c.String(),
                        InspireDeliveryWfsStatusAutoUpdate = c.Boolean(nullable: false),
                        InspireDeliveryAtomFeedStatusId = c.String(maxLength: 128),
                        InspireDeliveryAtomFeedStatusNote = c.String(),
                        InspireDeliveryAtomFeedStatusAutoUpdate = c.Boolean(nullable: false),
                        InspireDeliveryWfsOrAtomStatusId = c.String(maxLength: 128),
                        InspireDeliveryWfsOrAtomStatusNote = c.String(),
                        InspireDeliveryWfsOrAtomStatusAutoUpdate = c.Boolean(nullable: false),
                        InspireDeliveryHarmonizedDataStatusId = c.String(maxLength: 128),
                        InspireDeliveryHarmonizedDataStatusNote = c.String(),
                        InspireDeliveryHarmonizedDataStatusAutoUpdate = c.Boolean(nullable: false),
                        InspireDeliverySpatialDataServiceStatusId = c.String(maxLength: 128),
                        InspireDeliverySpatialDataServiceStatusNote = c.String(),
                        InspireDeliverySpatialDataServiceStatusAutoUpdate = c.Boolean(nullable: false),
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
                        datasetthumbnail = c.String(),
                        DokStatusId = c.String(maxLength: 128),
                        DokStatusDateAccepted = c.DateTime(),
                        Name = c.String(),
                        Seoname = c.String(),
                        Description = c.String(),
                        SubmitterId = c.Guid(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        DateSubmitted = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        statusId = c.String(maxLength: 128),
                        RegisterId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.DokStatus", t => t.DokStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliveryAtomFeedStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliveryDistributionStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliveryHarmonizedDataStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliveryMetadataServiceStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliveryMetadataStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliverySpatialDataServiceStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliveryWfsOrAtomStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliveryWfsStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.InspireDeliveryWmsStatusId)
                .ForeignKey("dbo.RegisterItems", t => t.OwnerId, cascadeDelete: false)
                .ForeignKey("dbo.Registers", t => t.RegisterId, cascadeDelete: false)
                .ForeignKey("dbo.Status", t => t.statusId)
                .ForeignKey("dbo.RegisterItems", t => t.SubmitterId, cascadeDelete: false)
                .ForeignKey("dbo.DOKThemes", t => t.ThemeGroupId)
                .Index(t => t.InspireDeliveryMetadataStatusId)
                .Index(t => t.InspireDeliveryMetadataServiceStatusId)
                .Index(t => t.InspireDeliveryDistributionStatusId)
                .Index(t => t.InspireDeliveryWmsStatusId)
                .Index(t => t.InspireDeliveryWfsStatusId)
                .Index(t => t.InspireDeliveryAtomFeedStatusId)
                .Index(t => t.InspireDeliveryWfsOrAtomStatusId)
                .Index(t => t.InspireDeliveryHarmonizedDataStatusId)
                .Index(t => t.InspireDeliverySpatialDataServiceStatusId)
                .Index(t => t.ThemeGroupId)
                .Index(t => t.DokStatusId)
                .Index(t => t.SubmitterId)
                .Index(t => t.OwnerId)
                .Index(t => t.statusId)
                .Index(t => t.RegisterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspireDatasets", "ThemeGroupId", "dbo.DOKThemes");
            DropForeignKey("dbo.InspireDatasets", "SubmitterId", "dbo.RegisterItems");
            DropForeignKey("dbo.InspireDatasets", "statusId", "dbo.Status");
            DropForeignKey("dbo.InspireDatasets", "RegisterId", "dbo.Registers");
            DropForeignKey("dbo.InspireDatasets", "OwnerId", "dbo.RegisterItems");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWmsStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "DokStatusId", "dbo.DokStatus");
            DropIndex("dbo.InspireDatasets", new[] { "RegisterId" });
            DropIndex("dbo.InspireDatasets", new[] { "statusId" });
            DropIndex("dbo.InspireDatasets", new[] { "OwnerId" });
            DropIndex("dbo.InspireDatasets", new[] { "SubmitterId" });
            DropIndex("dbo.InspireDatasets", new[] { "DokStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "ThemeGroupId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliverySpatialDataServiceStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryHarmonizedDataStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWfsOrAtomStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryAtomFeedStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWfsStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWmsStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryDistributionStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryMetadataServiceStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryMetadataStatusId" });
            DropTable("dbo.InspireDatasets");
        }
    }
}
