namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addForeignkeyToInspireCodeValue : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InspireDatasets", "InspireThemeId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireTheme_systemId", newName: "InspireThemeId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireTheme_systemId", newName: "IX_InspireThemeId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_InspireThemeId", newName: "IX_InspireTheme_systemId");
            RenameColumn(table: "dbo.InspireDatasets", name: "InspireThemeId", newName: "InspireTheme_systemId");
            AddColumn("dbo.InspireDatasets", "InspireThemeId", c => c.Guid());
        }
    }
}
