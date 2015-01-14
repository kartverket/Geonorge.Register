namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSeoname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "seoname", c => c.String());
            AddColumn("dbo.Registers", "seoname", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "seoname");
            DropColumn("dbo.Registers", "seoname");
        }
    }
}
