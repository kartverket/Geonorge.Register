namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDokMeasureStatuses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DokMeasureStatus",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.CoverageDatasets", "MeasureDOKStatusId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CoverageDatasets", "MeasureDOKStatusId");
            AddForeignKey("dbo.CoverageDatasets", "MeasureDOKStatusId", "dbo.DokMeasureStatus", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CoverageDatasets", "MeasureDOKStatusId", "dbo.DokMeasureStatus");
            DropIndex("dbo.CoverageDatasets", new[] { "MeasureDOKStatusId" });
            DropColumn("dbo.CoverageDatasets", "MeasureDOKStatusId");
            DropTable("dbo.DokMeasureStatus");
        }
    }
}
