namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModellbaserteVegprosjekter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeodatalovDatasets", "ModellbaserteVegprosjekter", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeodatalovDatasets", "ModellbaserteVegprosjekter");
        }
    }
}
