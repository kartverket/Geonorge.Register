namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCartographyFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "CartographyFile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "CartographyFile");
        }
    }
}
