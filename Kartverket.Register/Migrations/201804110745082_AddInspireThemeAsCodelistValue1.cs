namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireThemeAsCodelistValue1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDataServices", "InspireTheme_systemId", c => c.Guid());
            AddColumn("dbo.InspireDataServices", "InspireThemeId", c => c.Guid());
            CreateIndex("dbo.InspireDataServices", "InspireTheme_systemId");
            AddForeignKey("dbo.InspireDataServices", "InspireTheme_systemId", "dbo.RegisterItems", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspireDataServices", "InspireTheme_systemId", "dbo.RegisterItems");
            DropIndex("dbo.InspireDataServices", new[] { "InspireTheme_systemId" });
            DropColumn("dbo.InspireDataServices", "InspireThemeId");
            DropColumn("dbo.InspireDataServices", "InspireTheme_systemId");
        }
    }
}
