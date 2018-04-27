namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireThemeAsCollectionToInspireDataServices : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InspireDataServiceCodelistValues",
                c => new
                    {
                        InspireDataService_SystemId = c.Guid(nullable: false),
                        CodelistValue_systemId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.InspireDataService_SystemId, t.CodelistValue_systemId })
                .ForeignKey("dbo.InspireDataServices", t => t.InspireDataService_SystemId, cascadeDelete: false)
                .ForeignKey("dbo.RegisterItems", t => t.CodelistValue_systemId, cascadeDelete: false)
                .Index(t => t.InspireDataService_SystemId)
                .Index(t => t.CodelistValue_systemId);
            
            DropColumn("dbo.InspireDataServices", "InspireTheme");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "InspireTheme", c => c.String());
            DropForeignKey("dbo.InspireDataServiceCodelistValues", "CodelistValue_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.InspireDataServiceCodelistValues", "InspireDataService_SystemId", "dbo.InspireDataServices");
            DropIndex("dbo.InspireDataServiceCodelistValues", new[] { "CodelistValue_systemId" });
            DropIndex("dbo.InspireDataServiceCodelistValues", new[] { "InspireDataService_SystemId" });
            DropTable("dbo.InspireDataServiceCodelistValues");
        }
    }
}
