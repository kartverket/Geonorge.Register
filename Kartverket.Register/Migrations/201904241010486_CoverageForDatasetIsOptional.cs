namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CoverageForDatasetIsOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CoverageDatasets", "Coverage", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CoverageDatasets", "Coverage", c => c.Boolean(nullable: false));
        }
    }
}
