namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveInspireDatasetVirtual : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RegisterItemStatusReports", "InspireDataset_SystemId", "dbo.InspireDatasets");
            DropIndex("dbo.RegisterItemStatusReports", new[] { "InspireDataset_SystemId" });
            DropColumn("dbo.RegisterItemStatusReports", "InspireDataset_SystemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItemStatusReports", "InspireDataset_SystemId", c => c.Guid());
            CreateIndex("dbo.RegisterItemStatusReports", "InspireDataset_SystemId");
            AddForeignKey("dbo.RegisterItemStatusReports", "InspireDataset_SystemId", "dbo.InspireDatasets", "SystemId");
        }
    }
}
