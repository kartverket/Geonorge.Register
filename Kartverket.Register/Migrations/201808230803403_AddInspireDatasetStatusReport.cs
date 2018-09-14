namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireDatasetStatusReport : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.StatusHistories", newName: "RegisterItemStatusReports");
            AddColumn("dbo.RegisterItemStatusReports", "Uuid", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Metadata1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataService", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Distribution1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Wms1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "Wfs1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WfsOrAtom", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "AtomFeed1", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "HarmonizedData", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "SpatialDataService", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "InspireDataset_SystemId", c => c.Guid());
            CreateIndex("dbo.RegisterItemStatusReports", "InspireDataset_SystemId");
            AddForeignKey("dbo.RegisterItemStatusReports", "InspireDataset_SystemId", "dbo.InspireDatasets", "SystemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItemStatusReports", "InspireDataset_SystemId", "dbo.InspireDatasets");
            DropIndex("dbo.RegisterItemStatusReports", new[] { "InspireDataset_SystemId" });
            DropColumn("dbo.RegisterItemStatusReports", "InspireDataset_SystemId");
            DropColumn("dbo.RegisterItemStatusReports", "SpatialDataService");
            DropColumn("dbo.RegisterItemStatusReports", "HarmonizedData");
            DropColumn("dbo.RegisterItemStatusReports", "AtomFeed1");
            DropColumn("dbo.RegisterItemStatusReports", "WfsOrAtom");
            DropColumn("dbo.RegisterItemStatusReports", "Wfs1");
            DropColumn("dbo.RegisterItemStatusReports", "Wms1");
            DropColumn("dbo.RegisterItemStatusReports", "Distribution1");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataService");
            DropColumn("dbo.RegisterItemStatusReports", "Metadata1");
            DropColumn("dbo.RegisterItemStatusReports", "Uuid");
            RenameTable(name: "dbo.RegisterItemStatusReports", newName: "StatusHistories");
        }
    }
}
