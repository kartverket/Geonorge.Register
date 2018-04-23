namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAreaAndRelevantArea : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDataServices", "Area", c => c.Int(nullable: false));
            AddColumn("dbo.InspireDataServices", "RelevantArea", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDataServices", "RelevantArea");
            DropColumn("dbo.InspireDataServices", "Area");
        }
    }
}
