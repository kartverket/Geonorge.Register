namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CoverageDatasetSuitability : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoverageDatasets", "ZoningPlan", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "ImpactAssessmentPlanningBuildingAct", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "RiskVulnerabilityAnalysisPlanningBuildingAct", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CoverageDatasets", "RiskVulnerabilityAnalysisPlanningBuildingAct");
            DropColumn("dbo.CoverageDatasets", "ImpactAssessmentPlanningBuildingAct");
            DropColumn("dbo.CoverageDatasets", "ZoningPlan");
        }
    }
}
