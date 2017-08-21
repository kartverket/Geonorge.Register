namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateInspireDataset : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWmsStatusId", "dbo.DokDeliveryStatus");
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryMetadataStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryMetadataServiceStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryDistributionStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWmsStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWfsStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryAtomFeedStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWfsOrAtomStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryHarmonizedDataStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliverySpatialDataServiceStatusId" });
            CreateTable(
                "dbo.DeliveryStatus",
                c => new
                    {
                        DeliveryStatusId = c.Guid(nullable: false),
                        StatusId = c.String(maxLength: 128),
                        Note = c.String(),
                        AutoUpdate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DeliveryStatusId)
                .ForeignKey("dbo.DokDeliveryStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryWmsStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryWfsStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId", c => c.Guid(nullable: false));
            AlterColumn("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId", c => c.Guid(nullable: false));
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryWmsStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryWfsStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            AddForeignKey("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryWmsStatusId", "dbo.DeliveryStatus", "DeliveryStatusId", cascadeDelete: false);
            DropColumn("dbo.InspireDatasets", "InspireDeliveryMetadataStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryMetadataStatusAutoUpdate");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusAutoUpdate");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryDistributionStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryDistributionStatusAutoUpdate");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryWmsStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryWmsStatusAutoUpdate");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryWfsStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryWfsStatusAutoUpdate");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusAutoUpdate");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusAutoUpdate");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusAutoUpdate");
            DropColumn("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusNote");
            DropColumn("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusAutoUpdate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusNote", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusNote", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusNote", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusNote", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireDeliveryWfsStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliveryWfsStatusNote", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireDeliveryWmsStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliveryWmsStatusNote", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireDeliveryDistributionStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliveryDistributionStatusNote", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusNote", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireDeliveryMetadataStatusAutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.InspireDatasets", "InspireDeliveryMetadataStatusNote", c => c.String());
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWmsStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId", "dbo.DeliveryStatus");
            DropForeignKey("dbo.DeliveryStatus", "StatusId", "dbo.DokDeliveryStatus");
            DropIndex("dbo.DeliveryStatus", new[] { "StatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliverySpatialDataServiceStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryHarmonizedDataStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWfsOrAtomStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryAtomFeedStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWfsStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryWmsStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryDistributionStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryMetadataServiceStatusId" });
            DropIndex("dbo.InspireDatasets", new[] { "InspireDeliveryMetadataStatusId" });
            AlterColumn("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId", c => c.String(maxLength: 128));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId", c => c.String(maxLength: 128));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId", c => c.String(maxLength: 128));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId", c => c.String(maxLength: 128));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryWfsStatusId", c => c.String(maxLength: 128));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryWmsStatusId", c => c.String(maxLength: 128));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId", c => c.String(maxLength: 128));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId", c => c.String(maxLength: 128));
            AlterColumn("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId", c => c.String(maxLength: 128));
            DropTable("dbo.DeliveryStatus");
            CreateIndex("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryWfsStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryWmsStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId");
            CreateIndex("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryWmsStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryWfsOrAtomStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliverySpatialDataServiceStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryMetadataServiceStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryHarmonizedDataStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryDistributionStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.InspireDatasets", "InspireDeliveryAtomFeedStatusId", "dbo.DokDeliveryStatus", "value");
        }
    }
}
