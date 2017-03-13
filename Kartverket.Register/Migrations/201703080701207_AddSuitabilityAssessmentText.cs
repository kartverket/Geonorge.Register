namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSuitabilityAssessmentText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoverageDatasets", "suitabilityAssessmentText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CoverageDatasets", "suitabilityAssessmentText");
        }
    }
}
