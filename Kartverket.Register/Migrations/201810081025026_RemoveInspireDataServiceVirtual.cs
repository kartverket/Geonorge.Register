namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveInspireDataServiceVirtual : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId", "dbo.InspireDataServices");
            DropIndex("dbo.RegisterItemStatusReports", new[] { "InspireDataservice_SystemId" });
            DropColumn("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId", c => c.Guid());
            CreateIndex("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId");
            AddForeignKey("dbo.RegisterItemStatusReports", "InspireDataservice_SystemId", "dbo.InspireDataServices", "SystemId");
        }
    }
}
