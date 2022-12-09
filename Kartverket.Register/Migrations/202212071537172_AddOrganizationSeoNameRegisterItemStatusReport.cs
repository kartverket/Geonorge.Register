namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrganizationSeoNameRegisterItemStatusReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItemStatusReports", "OrganizationSeoName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "OrganizationSeoName");
        }
    }
}
