namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusReportFairPerCent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItemStatusReports", "FindableStatusPerCent", c => c.Double(nullable: false));
            AddColumn("dbo.RegisterItemStatusReports", "AccessibleStatusPerCent", c => c.Double(nullable: false));
            AddColumn("dbo.RegisterItemStatusReports", "InteroperableStatusPerCent", c => c.Double(nullable: false));
            AddColumn("dbo.RegisterItemStatusReports", "ReUseableStatusPerCent", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "ReUseableStatusPerCent");
            DropColumn("dbo.RegisterItemStatusReports", "InteroperableStatusPerCent");
            DropColumn("dbo.RegisterItemStatusReports", "AccessibleStatusPerCent");
            DropColumn("dbo.RegisterItemStatusReports", "FindableStatusPerCent");
        }
    }
}
