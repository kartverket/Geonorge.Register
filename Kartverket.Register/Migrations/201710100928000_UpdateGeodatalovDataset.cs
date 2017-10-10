namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateGeodatalovDataset : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GeodatalovDatasets", "InspireTheme", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GeodatalovDatasets", "InspireTheme", c => c.String());
        }
    }
}
