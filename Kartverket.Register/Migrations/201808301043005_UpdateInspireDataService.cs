namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateInspireDataService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItemStatusReports", "Sds", c => c.Boolean());
            AddColumn("dbo.RegisterItemStatusReports", "NetworkService", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "NetworkService");
            DropColumn("dbo.RegisterItemStatusReports", "Sds");
        }
    }
}
