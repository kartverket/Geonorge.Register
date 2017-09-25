namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireDataset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DatasetDeliveries",
                c => new
                    {
                        DatasetDeliveryId = c.Guid(nullable: false),
                        StatusId = c.String(maxLength: 128),
                        Note = c.String(),
                        AutoUpdate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DatasetDeliveryId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.InspireDatasets",
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
                        InspireDeliveryMetadataId = c.Guid(nullable: false),
                        InspireDeliveryMetadataServiceId = c.Guid(nullable: false),
                        InspireDeliveryDistributionId = c.Guid(nullable: false),
                        InspireDeliveryWmsId = c.Guid(nullable: false),
                        InspireDeliveryWfsId = c.Guid(nullable: false),
                        InspireDeliveryAtomFeedId = c.Guid(nullable: false),
                        InspireDeliveryWfsOrAtomId = c.Guid(nullable: false),
                        InspireDeliveryHarmonizedDataId = c.Guid(nullable: false),
                        InspireDeliverySpatialDataServiceId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.RegisterItems", t => t.SubmitterId)
                .ForeignKey("dbo.RegisterItems", t => t.OwnerId)
                .ForeignKey("dbo.Status", t => t.StatusId)
                .ForeignKey("dbo.Registers", t => t.RegisterId, cascadeDelete: true)
                .ForeignKey("dbo.DOKThemes", t => t.ThemeGroupId)
                .ForeignKey("dbo.DokStatus", t => t.DokStatusId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryMetadataId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryMetadataServiceId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryDistributionId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryWmsId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryWfsId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryAtomFeedId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryWfsOrAtomId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryHarmonizedDataId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliverySpatialDataServiceId)
                .Index(t => t.SubmitterId)
                .Index(t => t.OwnerId)
                .Index(t => t.StatusId)
                .Index(t => t.RegisterId)
                .Index(t => t.ThemeGroupId)
                .Index(t => t.DokStatusId)
                .Index(t => t.InspireDeliveryMetadataId)
                .Index(t => t.InspireDeliveryMetadataServiceId)
                .Index(t => t.InspireDeliveryDistributionId)
                .Index(t => t.InspireDeliveryWmsId)
                .Index(t => t.InspireDeliveryWfsId)
                .Index(t => t.InspireDeliveryAtomFeedId)
                .Index(t => t.InspireDeliveryWfsOrAtomId)
                .Index(t => t.InspireDeliveryHarmonizedDataId)
                .Index(t => t.InspireDeliverySpatialDataServiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryAtomFeedId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWmsId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryDistributionId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataServiceId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDatasets", "DokStatusId", "dbo.DokStatus");
            DropForeignKey("dbo.InspireDatasets", "ThemeGroupId", "dbo.DOKThemes");
            DropForeignKey("dbo.InspireDatasets", "RegisterId", "dbo.Registers");
            DropForeignKey("dbo.InspireDatasets", "StatusId", "dbo.Status");
            DropForeignKey("dbo.InspireDatasets", "OwnerId", "dbo.RegisterItems");
            DropForeignKey("dbo.InspireDatasets", "SubmitterId", "dbo.RegisterItems");
            DropForeignKey("dbo.DatasetDeliveries", "StatusId", "dbo.DokDeliveryStatus");
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliverySpatialDataServiceId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryHarmonizedDataId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWfsOrAtomId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryAtomFeedId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWfsId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWmsId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryDistributionId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryMetadataServiceId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryMetadataId" });
            DropIndex("dbo.InspireDatasets", new[] { "DokStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "ThemeGroupId" });
            DropIndex("dbo.InspireDatasets", new[] { "RegisterId" });
            DropIndex("dbo.InspireDatasets", new[] { "StatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "OwnerId" });
            DropIndex("dbo.InspireDatasets", new[] { "SubmitterId" });
            DropIndex("dbo.DatasetDeliveries", new[] { "StatusId" });
            DropTable("dbo.InspireDatasets");
            DropTable("dbo.DatasetDeliveries");
        }
    }
}
