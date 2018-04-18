namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArealAndRelevantArealToInspireDataset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDatasets", "Area", c => c.Int(nullable: false));
            AddColumn("dbo.InspireDatasets", "RelevantArea", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDatasets", "RelevantArea");
            DropColumn("dbo.InspireDatasets", "Area");
        }
    }
}
