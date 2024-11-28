namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFairR2_i_Criteria : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FairDatasets", "R2_i_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_i_Criteria", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MareanoDatasets", "R2_i_Criteria");
            DropColumn("dbo.FairDatasets", "R2_i_Criteria");
        }
    }
}
