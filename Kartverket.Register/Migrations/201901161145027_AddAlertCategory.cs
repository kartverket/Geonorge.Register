namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "AlertCategory", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "AlertCategory");
        }
    }
}
