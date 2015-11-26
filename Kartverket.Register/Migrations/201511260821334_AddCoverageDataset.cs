namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCoverageDataset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CoverageDatasets",
                c => new
                    {
                        CoverageId = c.Guid(nullable: false),
                        MunicipalityId = c.Guid(nullable: false),
                        ConfirmedDok = c.Boolean(nullable: false),
                        DatasetId = c.Guid(nullable: false),
                        Note = c.String(),
                        CoverageDOKStatusId = c.String(maxLength: 128),
                        Dataset_systemId = c.Guid(),
                    })
                .PrimaryKey(t => t.CoverageId)
                .ForeignKey("dbo.DokStatus", t => t.CoverageDOKStatusId)
                .ForeignKey("dbo.RegisterItems", t => t.DatasetId, cascadeDelete: false)
                .ForeignKey("dbo.RegisterItems", t => t.MunicipalityId, cascadeDelete: false)
                .ForeignKey("dbo.RegisterItems", t => t.Dataset_systemId, cascadeDelete: false)
                .Index(t => t.MunicipalityId)
                .Index(t => t.DatasetId)
                .Index(t => t.CoverageDOKStatusId)
                .Index(t => t.Dataset_systemId);
            
            AddColumn("dbo.RegisterItems", "DatasetType", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CoverageDatasets", "Dataset_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.CoverageDatasets", "MunicipalityId", "dbo.RegisterItems");
            DropForeignKey("dbo.CoverageDatasets", "DatasetId", "dbo.RegisterItems");
            DropForeignKey("dbo.CoverageDatasets", "CoverageDOKStatusId", "dbo.DokStatus");
            DropIndex("dbo.CoverageDatasets", new[] { "Dataset_systemId" });
            DropIndex("dbo.CoverageDatasets", new[] { "CoverageDOKStatusId" });
            DropIndex("dbo.CoverageDatasets", new[] { "DatasetId" });
            DropIndex("dbo.CoverageDatasets", new[] { "MunicipalityId" });
            DropColumn("dbo.RegisterItems", "DatasetType");
            DropTable("dbo.CoverageDatasets");
        }
    }
}
