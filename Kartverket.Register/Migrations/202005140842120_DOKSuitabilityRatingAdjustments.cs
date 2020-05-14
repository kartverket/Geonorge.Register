namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DOKSuitabilityRatingAdjustments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "ZoningPlan", c => c.Int());
            AddColumn("dbo.RegisterItems", "ZoningPlanNote", c => c.String());
            AddColumn("dbo.RegisterItems", "ImpactAssessmentPlanningBuildingAct", c => c.Int());
            AddColumn("dbo.RegisterItems", "ImpactAssessmentPlanningBuildingActNote", c => c.String());
            AddColumn("dbo.RegisterItems", "RiskVulnerabilityAnalysisPlanningBuildingAct", c => c.Int());
            AddColumn("dbo.RegisterItems", "RiskVulnerabilityAnalysisPlanningBuildingActNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "RiskVulnerabilityAnalysisPlanningBuildingActNote");
            DropColumn("dbo.RegisterItems", "RiskVulnerabilityAnalysisPlanningBuildingAct");
            DropColumn("dbo.RegisterItems", "ImpactAssessmentPlanningBuildingActNote");
            DropColumn("dbo.RegisterItems", "ImpactAssessmentPlanningBuildingAct");
            DropColumn("dbo.RegisterItems", "ZoningPlanNote");
            DropColumn("dbo.RegisterItems", "ZoningPlan");
        }
    }
}
