namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeodatalovDatasetKeywords : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeodatalovDatasets", "Mareano", c => c.Boolean(nullable: false));
            AddColumn("dbo.GeodatalovDatasets", "EcologicalBaseMap", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeodatalovDatasets", "EcologicalBaseMap");
            DropColumn("dbo.GeodatalovDatasets", "Mareano");
        }
    }
}
