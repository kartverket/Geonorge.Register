namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollectionOfInspireThemsForInspireDataServices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "InspireDataService_SystemId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "InspireDataService_SystemId");
            AddForeignKey("dbo.RegisterItems", "InspireDataService_SystemId", "dbo.InspireDataServices", "SystemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "InspireDataService_SystemId", "dbo.InspireDataServices");
            DropIndex("dbo.RegisterItems", new[] { "InspireDataService_SystemId" });
            DropColumn("dbo.RegisterItems", "InspireDataService_SystemId");
        }
    }
}
