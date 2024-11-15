namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMareanoProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MareanoDatasets", "F2_d_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "F2_e_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R1_b_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_g_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_h_Criteria", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MareanoDatasets", "R2_h_Criteria");
            DropColumn("dbo.MareanoDatasets", "R2_g_Criteria");
            DropColumn("dbo.MareanoDatasets", "R1_b_Criteria");
            DropColumn("dbo.MareanoDatasets", "F2_e_Criteria");
            DropColumn("dbo.MareanoDatasets", "F2_d_Criteria");
        }
    }
}
