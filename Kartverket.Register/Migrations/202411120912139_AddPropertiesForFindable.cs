namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertiesForFindable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FairDatasets", "F2_d_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.FairDatasets", "F2_e_Criteria", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FairDatasets", "F2_e_Criteria");
            DropColumn("dbo.FairDatasets", "F2_d_Criteria");
        }
    }
}
