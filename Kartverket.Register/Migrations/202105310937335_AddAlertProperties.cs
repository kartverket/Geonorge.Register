namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "ValidFrom", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "ValidTo", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "Summary", c => c.String());
            AddColumn("dbo.RegisterItems", "Link", c => c.String());
            AddColumn("dbo.RegisterItems", "Image1", c => c.String());
            AddColumn("dbo.RegisterItems", "Image2", c => c.String());
            AddColumn("dbo.RegisterItems", "DateResolved", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "DateResolved");
            DropColumn("dbo.RegisterItems", "Image2");
            DropColumn("dbo.RegisterItems", "Image1");
            DropColumn("dbo.RegisterItems", "Link");
            DropColumn("dbo.RegisterItems", "Summary");
            DropColumn("dbo.RegisterItems", "ValidTo");
            DropColumn("dbo.RegisterItems", "ValidFrom");
        }
    }
}
