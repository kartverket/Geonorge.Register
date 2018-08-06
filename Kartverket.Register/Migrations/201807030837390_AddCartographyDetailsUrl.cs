namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCartographyDetailsUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "CartographyDetailsUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "CartographyDetailsUrl");
        }
    }
}
