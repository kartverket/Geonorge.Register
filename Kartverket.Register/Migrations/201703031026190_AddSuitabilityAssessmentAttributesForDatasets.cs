namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSuitabilityAssessmentAttributesForDatasets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "RegionalPlan", c => c.Int());
            AddColumn("dbo.RegisterItems", "RegionalPlanNote", c => c.String());
            AddColumn("dbo.RegisterItems", "MunicipalSocialPlan", c => c.Int());
            AddColumn("dbo.RegisterItems", "MunicipalSocialPlanNote", c => c.String());
            AddColumn("dbo.RegisterItems", "MunicipalLandUseElementPlan", c => c.Int());
            AddColumn("dbo.RegisterItems", "MunicipalLandUseElementPlanNote", c => c.String());
            AddColumn("dbo.RegisterItems", "ZoningPlanArea", c => c.Int());
            AddColumn("dbo.RegisterItems", "ZoningPlanAreaNote", c => c.String());
            AddColumn("dbo.RegisterItems", "ZoningPlanDetails", c => c.Int());
            AddColumn("dbo.RegisterItems", "ZoningPlanDetailsNote", c => c.String());
            AddColumn("dbo.RegisterItems", "BuildingMatter", c => c.Int());
            AddColumn("dbo.RegisterItems", "BuildingMatterNote", c => c.String());
            AddColumn("dbo.RegisterItems", "PartitionOff", c => c.Int());
            AddColumn("dbo.RegisterItems", "PartitionOffNote", c => c.String());
            AddColumn("dbo.RegisterItems", "EenvironmentalImpactAssessment", c => c.Int());
            AddColumn("dbo.RegisterItems", "EenvironmentalImpactAssessmentNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "EenvironmentalImpactAssessmentNote");
            DropColumn("dbo.RegisterItems", "EenvironmentalImpactAssessment");
            DropColumn("dbo.RegisterItems", "PartitionOffNote");
            DropColumn("dbo.RegisterItems", "PartitionOff");
            DropColumn("dbo.RegisterItems", "BuildingMatterNote");
            DropColumn("dbo.RegisterItems", "BuildingMatter");
            DropColumn("dbo.RegisterItems", "ZoningPlanDetailsNote");
            DropColumn("dbo.RegisterItems", "ZoningPlanDetails");
            DropColumn("dbo.RegisterItems", "ZoningPlanAreaNote");
            DropColumn("dbo.RegisterItems", "ZoningPlanArea");
            DropColumn("dbo.RegisterItems", "MunicipalLandUseElementPlanNote");
            DropColumn("dbo.RegisterItems", "MunicipalLandUseElementPlan");
            DropColumn("dbo.RegisterItems", "MunicipalSocialPlanNote");
            DropColumn("dbo.RegisterItems", "MunicipalSocialPlan");
            DropColumn("dbo.RegisterItems", "RegionalPlanNote");
            DropColumn("dbo.RegisterItems", "RegionalPlan");
        }
    }
}
