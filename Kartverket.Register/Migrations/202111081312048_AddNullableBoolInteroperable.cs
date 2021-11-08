namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNullableBoolInteroperable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MareanoDatasets", "I1_c_Criteria", c => c.Boolean());
            AlterColumn("dbo.MareanoDatasets", "I3_a_Criteria", c => c.Boolean());
            AlterColumn("dbo.MareanoDatasets", "I3_b_Criteria", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MareanoDatasets", "I3_b_Criteria", c => c.Boolean(nullable: false));
            AlterColumn("dbo.MareanoDatasets", "I3_a_Criteria", c => c.Boolean(nullable: false));
            AlterColumn("dbo.MareanoDatasets", "I1_c_Criteria", c => c.Boolean(nullable: false));
        }
    }
}
