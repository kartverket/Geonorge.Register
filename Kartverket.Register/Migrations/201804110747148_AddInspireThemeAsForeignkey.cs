namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireThemeAsForeignkey : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InspireDataServices", "InspireThemeId");
            RenameColumn(table: "dbo.InspireDataServices", name: "InspireTheme_systemId", newName: "InspireThemeId");
            RenameIndex(table: "dbo.InspireDataServices", name: "IX_InspireTheme_systemId", newName: "IX_InspireThemeId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.InspireDataServices", name: "IX_InspireThemeId", newName: "IX_InspireTheme_systemId");
            RenameColumn(table: "dbo.InspireDataServices", name: "InspireThemeId", newName: "InspireTheme_systemId");
            AddColumn("dbo.InspireDataServices", "InspireThemeId", c => c.Guid());
        }
    }
}
