namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollectonOfInspireDatasets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "InspireDataset_SystemId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "InspireDataset_SystemId");
            AddForeignKey("dbo.RegisterItems", "InspireDataset_SystemId", "dbo.InspireDatasets", "SystemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "InspireDataset_SystemId", "dbo.InspireDatasets");
            DropIndex("dbo.RegisterItems", new[] { "InspireDataset_SystemId" });
            DropColumn("dbo.RegisterItems", "InspireDataset_SystemId");
        }
    }
}
