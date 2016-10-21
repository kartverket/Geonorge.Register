namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApplicationSchema : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "ApplicationSchema", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "ApplicationSchema");
        }
    }
}
