namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollectionOfInspireThemes : DbMigration
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
            
            AddColumn("dbo.RegisterItems", "InspireDataService_SystemId", c => c.Guid());
            AddColumn("dbo.InspireDataServices", "InspireThemeId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "InspireDataService_SystemId");
            CreateIndex("dbo.InspireDataServices", "InspireThemeId");
            AddForeignKey("dbo.RegisterItems", "InspireDataService_SystemId", "dbo.InspireDataServices", "SystemId");
            AddForeignKey("dbo.InspireDataServices", "InspireThemeId", "dbo.RegisterItems", "systemId");
            DropColumn("dbo.InspireDatasets", "InspireTheme");
            DropColumn("dbo.InspireDataServices", "Theme");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "Theme", c => c.String());
            AddColumn("dbo.InspireDatasets", "InspireTheme", c => c.String());
            DropForeignKey("dbo.InspireDataServices", "InspireThemeId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "InspireDataService_SystemId", "dbo.InspireDataServices");
            DropForeignKey("dbo.InspireDatasetCodelistValues", "CodelistValue_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.InspireDatasetCodelistValues", "InspireDataset_SystemId", "dbo.InspireDatasets");
            DropIndex("dbo.InspireDataServices", new[] { "InspireThemeId" });
            DropIndex("dbo.InspireDatasetCodelistValues", new[] { "CodelistValue_systemId" });
            DropIndex("dbo.InspireDatasetCodelistValues", new[] { "InspireDataset_SystemId" });
            DropIndex("dbo.RegisterItems", new[] { "InspireDataService_SystemId" });
            DropColumn("dbo.InspireDataServices", "InspireThemeId");
            DropColumn("dbo.RegisterItems", "InspireDataService_SystemId");
            DropTable("dbo.InspireDatasetCodelistValues");
        }
    }
}
