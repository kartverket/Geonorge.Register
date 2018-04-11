namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteThemeFromInspireDataServices : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InspireDataServices", "Theme");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "Theme", c => c.String());
        }
    }
}
