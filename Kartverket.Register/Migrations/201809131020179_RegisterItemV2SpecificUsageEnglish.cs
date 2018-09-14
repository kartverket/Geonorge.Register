namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegisterItemV2SpecificUsageEnglish : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeodatalovDatasets", "SpecificUsageEnglish", c => c.String());
            AddColumn("dbo.InspireDatasets", "SpecificUsageEnglish", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDatasets", "SpecificUsageEnglish");
            DropColumn("dbo.GeodatalovDatasets", "SpecificUsageEnglish");
        }
    }
}
