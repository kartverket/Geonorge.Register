namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertiesForReusable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FairDatasets", "R1_b_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.FairDatasets", "R2_g_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.FairDatasets", "R2_h_Criteria", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FairDatasets", "R2_h_Criteria");
            DropColumn("dbo.FairDatasets", "R2_g_Criteria");
            DropColumn("dbo.FairDatasets", "R1_b_Criteria");
        }
    }
}
