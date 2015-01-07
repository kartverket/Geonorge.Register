namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addShortname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "shortname", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "shortname");
        }
    }
}
