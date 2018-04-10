namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireThemeAsCodelistValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDatasets", "InspireTheme_systemId", c => c.Guid());
            AddColumn("dbo.InspireDatasets", "InspireThemeId", c => c.Guid());
            CreateIndex("dbo.InspireDatasets", "InspireTheme_systemId");
            AddForeignKey("dbo.InspireDatasets", "InspireTheme_systemId", "dbo.RegisterItems", "systemId");
            DropColumn("dbo.InspireDatasets", "InspireTheme");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDatasets", "InspireTheme", c => c.String());
            DropForeignKey("dbo.InspireDatasets", "InspireTheme_systemId", "dbo.RegisterItems");
            DropIndex("dbo.InspireDatasets", new[] { "InspireTheme_systemId" });
            DropColumn("dbo.InspireDatasets", "InspireThemeId");
            DropColumn("dbo.InspireDatasets", "InspireTheme_systemId");
        }
    }
}
