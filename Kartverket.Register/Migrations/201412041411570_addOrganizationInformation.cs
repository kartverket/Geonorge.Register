namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrganizationInformation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "largeLogo", c => c.String());
            AddColumn("dbo.RegisterItems", "contact", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "contact");
            DropColumn("dbo.RegisterItems", "largeLogo");
        }
    }
}
