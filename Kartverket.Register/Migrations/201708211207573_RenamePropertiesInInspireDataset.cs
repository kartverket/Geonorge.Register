namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamePropertiesInInspireDataset : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryAtomFeedStatusId", newName: "InspireDeliveryAtomFeedId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryDistributionStatusId", newName: "InspireDeliveryDistributionId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryHarmonizedDataStatusId", newName: "InspireDeliveryHarmonizedDataId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryMetadataServiceStatusId", newName: "InspireDeliveryMetadataId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryMetadataStatusId", newName: "InspireDeliveryMetadataServiceId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliverySpatialDataServiceStatusId", newName: "InspireDeliverySpatialDataServiceId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryWfsOrAtomStatusId", newName: "InspireDeliveryWfsId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryWfsStatusId", newName: "InspireDeliveryWfsOrAtomId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryWmsStatusId", newName: "InspireDeliveryWmsId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryMetadataServiceStatusId", newName: "IX_InspireDeliveryMetadataId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryMetadataStatusId", newName: "IX_InspireDeliveryMetadataServiceId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryDistributionStatusId", newName: "IX_InspireDeliveryDistributionId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryWmsStatusId", newName: "IX_InspireDeliveryWmsId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryWfsOrAtomStatusId", newName: "IX_InspireDeliveryWfsId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryAtomFeedStatusId", newName: "IX_InspireDeliveryAtomFeedId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryWfsStatusId", newName: "IX_InspireDeliveryWfsOrAtomId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryHarmonizedDataStatusId", newName: "IX_InspireDeliveryHarmonizedDataId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliverySpatialDataServiceStatusId", newName: "IX_InspireDeliverySpatialDataServiceId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliverySpatialDataServiceId", newName: "IX_InspireDeliverySpatialDataServiceStatusId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryHarmonizedDataId", newName: "IX_InspireDeliveryHarmonizedDataStatusId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryWfsOrAtomId", newName: "IX_InspireDeliveryWfsStatusId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryAtomFeedId", newName: "IX_InspireDeliveryAtomFeedStatusId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryWfsId", newName: "IX_InspireDeliveryWfsOrAtomStatusId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryWmsId", newName: "IX_InspireDeliveryWmsStatusId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryDistributionId", newName: "IX_InspireDeliveryDistributionStatusId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryMetadataServiceId", newName: "IX_InspireDeliveryMetadataStatusId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireDeliveryMetadataId", newName: "IX_InspireDeliveryMetadataServiceStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryWmsId", newName: "InspireDeliveryWmsStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryWfsOrAtomId", newName: "InspireDeliveryWfsStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryWfsId", newName: "InspireDeliveryWfsOrAtomStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliverySpatialDataServiceId", newName: "InspireDeliverySpatialDataServiceStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryMetadataServiceId", newName: "InspireDeliveryMetadataStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryMetadataId", newName: "InspireDeliveryMetadataServiceStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryHarmonizedDataId", newName: "InspireDeliveryHarmonizedDataStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryDistributionId", newName: "InspireDeliveryDistributionStatusId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireDeliveryAtomFeedId", newName: "InspireDeliveryAtomFeedStatusId");
        }
    }
}
