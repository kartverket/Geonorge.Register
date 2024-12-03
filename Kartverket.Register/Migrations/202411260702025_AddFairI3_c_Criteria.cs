namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFairI3_c_Criteria : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FairDatasets", "I3_c_Criteria", c => c.Boolean());
            AddColumn("dbo.MareanoDatasets", "I3_c_Criteria", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MareanoDatasets", "I3_c_Criteria");
            DropColumn("dbo.FairDatasets", "I3_c_Criteria");
        }
    }
}
