namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireThemeToInspireDataset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDatasets", "InspireTheme", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDatasets", "InspireTheme");
        }
    }
}
