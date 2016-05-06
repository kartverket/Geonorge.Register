namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCoverageForDataset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoverageDatasets", "Coverage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CoverageDatasets", "Coverage");
        }
    }
}
