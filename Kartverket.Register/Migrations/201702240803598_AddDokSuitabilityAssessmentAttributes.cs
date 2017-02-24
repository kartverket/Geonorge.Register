namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDokSuitabilityAssessmentAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoverageDatasets", "RegionalPlan", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "MunicipalSocialPlan", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "MunicipalLandUseElementPlan", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "ZoningPlanArea", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "ZoningPlanDetails", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "BuildingMatter", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "PartitionOff", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoverageDatasets", "EenvironmentalImpactAssessment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CoverageDatasets", "EenvironmentalImpactAssessment");
            DropColumn("dbo.CoverageDatasets", "PartitionOff");
            DropColumn("dbo.CoverageDatasets", "BuildingMatter");
            DropColumn("dbo.CoverageDatasets", "ZoningPlanDetails");
            DropColumn("dbo.CoverageDatasets", "ZoningPlanArea");
            DropColumn("dbo.CoverageDatasets", "MunicipalLandUseElementPlan");
            DropColumn("dbo.CoverageDatasets", "MunicipalSocialPlan");
            DropColumn("dbo.CoverageDatasets", "RegionalPlan");
        }
    }
}
