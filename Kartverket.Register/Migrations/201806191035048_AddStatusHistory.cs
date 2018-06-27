namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StatusHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Metadata = c.String(),
                        ProductSheet = c.String(),
                        PresentationRules = c.String(),
                        ProductSpecification = c.String(),
                        Wms = c.String(),
                        Wfs = c.String(),
                        Distribution = c.String(),
                        SosiRequirements = c.String(),
                        GmlRequirements = c.String(),
                        AtomFeed = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Dataset_systemId = c.Guid(),
                        StatusReport_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RegisterItems", t => t.Dataset_systemId)
                .ForeignKey("dbo.StatusReports", t => t.StatusReport_Id)
                .Index(t => t.Dataset_systemId)
                .Index(t => t.StatusReport_Id);
            
            CreateTable(
                "dbo.StatusReports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StatusHistories", "StatusReport_Id", "dbo.StatusReports");
            DropForeignKey("dbo.StatusHistories", "Dataset_systemId", "dbo.RegisterItems");
            DropIndex("dbo.StatusHistories", new[] { "StatusReport_Id" });
            DropIndex("dbo.StatusHistories", new[] { "Dataset_systemId" });
            DropTable("dbo.StatusReports");
            DropTable("dbo.StatusHistories");
        }
    }
}
