namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLatestSavedDataReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatusReports", "LatestSavedDataReport", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StatusReports", "LatestSavedDataReport");
        }
    }
}
