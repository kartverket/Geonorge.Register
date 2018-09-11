namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireDataServiceStatusReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItemStatusReports", "UuidInspireDataService", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataInspireDataService", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataInSearchServiceInspireDataService", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "ServiceStatusInspireDataService", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "UuidInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataServiceInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "DistributionInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WmsInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WfsInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WfsOrAtomInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "AtomFeedInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "HarmonizedDataInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "SpatialDataServiceInspireDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId", c => c.Guid());
            CreateIndex("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId");
            AddForeignKey("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId", "dbo.InspireDataServices", "SystemId");
            DropColumn("dbo.RegisterItemStatusReports", "Uuid");
            DropColumn("dbo.RegisterItemStatusReports", "Metadata1");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataService");
            DropColumn("dbo.RegisterItemStatusReports", "Distribution1");
            DropColumn("dbo.RegisterItemStatusReports", "Wms1");
            DropColumn("dbo.RegisterItemStatusReports", "Wfs1");
            DropColumn("dbo.RegisterItemStatusReports", "WfsOrAtom");
            DropColumn("dbo.RegisterItemStatusReports", "AtomFeed1");
            DropColumn("dbo.RegisterItemStatusReports", "HarmonizedData");
            DropColumn("dbo.RegisterItemStatusReports", "SpatialDataService");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItemStatusReports", "SpatialDataService", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "HarmonizedData", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "AtomFeed1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WfsOrAtom", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Wfs1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Wms1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Distribution1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataService", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Metadata1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Uuid", c => c.String());
            DropForeignKey("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId", "dbo.InspireDataServices");
            DropIndex("dbo.RegisterItemStatusReports", new[] { "InspireDataservice_SystemId" });
            DropColumn("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId");
            DropColumn("dbo.RegisterItemStatusReports", "SpatialDataServiceInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "HarmonizedDataInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "AtomFeedInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WfsOrAtomInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WfsInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WmsInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "DistributionInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataServiceInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "UuidInspireDataset");
            DropColumn("dbo.RegisterItemStatusReports", "ServiceStatusInspireDataService");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataInSearchServiceInspireDataService");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataInspireDataService");
            DropColumn("dbo.RegisterItemStatusReports", "UuidInspireDataService");
        }
    }
}
