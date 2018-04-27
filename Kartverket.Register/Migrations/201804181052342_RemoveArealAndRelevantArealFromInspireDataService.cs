namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveArealAndRelevantArealFromInspireDataService : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InspireDataServices", "Area");
            DropColumn("dbo.InspireDataServices", "RelevantArea");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "RelevantArea", c => c.Int(nullable: false));
            AddColumn("dbo.InspireDataServices", "Area", c => c.Int(nullable: false));
        }
    }
}
