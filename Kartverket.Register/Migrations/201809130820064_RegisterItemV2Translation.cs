namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegisterItemV2Translation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeodatalovDatasets", "NameEnglish", c => c.String());
            AddColumn("dbo.GeodatalovDatasets", "DescriptionEnglish", c => c.String());
            AddColumn("dbo.InspireDatasets", "NameEnglish", c => c.String());
            AddColumn("dbo.InspireDatasets", "DescriptionEnglish", c => c.String());
            AddColumn("dbo.InspireDataServices", "NameEnglish", c => c.String());
            AddColumn("dbo.InspireDataServices", "DescriptionEnglish", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDataServices", "DescriptionEnglish");
            DropColumn("dbo.InspireDataServices", "NameEnglish");
            DropColumn("dbo.InspireDatasets", "DescriptionEnglish");
            DropColumn("dbo.InspireDatasets", "NameEnglish");
            DropColumn("dbo.GeodatalovDatasets", "DescriptionEnglish");
            DropColumn("dbo.GeodatalovDatasets", "NameEnglish");
        }
    }
}
