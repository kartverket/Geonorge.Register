namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireThemeAsCollectionToInspireDataset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InspireDatasetCodelistValues",
                c => new
                    {
                        InspireDataset_SystemId = c.Guid(nullable: false),
                        CodelistValue_systemId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.InspireDataset_SystemId, t.CodelistValue_systemId })
                .ForeignKey("dbo.InspireDatasets", t => t.InspireDataset_SystemId, cascadeDelete: false)
                .ForeignKey("dbo.RegisterItems", t => t.CodelistValue_systemId, cascadeDelete: false)
                .Index(t => t.InspireDataset_SystemId)
                .Index(t => t.CodelistValue_systemId);
            
            AddColumn("dbo.InspireDataServices", "InspireTheme", c => c.String());
            DropColumn("dbo.InspireDatasets", "InspireTheme");
            DropColumn("dbo.InspireDataServices", "Theme");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "Theme", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireTheme", c => c.String());
            DropForeignKey("dbo.InspireDatasetCodelistValues", "CodelistValue_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.InspireDatasetCodelistValues", "InspireDataset_SystemId", "dbo.InspireDatasets");
            DropIndex("dbo.InspireDatasetCodelistValues", new[] { "CodelistValue_systemId" });
            DropIndex("dbo.InspireDatasetCodelistValues", new[] { "InspireDataset_SystemId" });
            DropColumn("dbo.InspireDataServices", "InspireTheme");
            DropTable("dbo.InspireDatasetCodelistValues");
        }
    }
}
