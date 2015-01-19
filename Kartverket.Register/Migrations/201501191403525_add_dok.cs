namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_dok : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DokDatasets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Uuid = c.String(),
                        Name = c.String(),
                        Publisher = c.String(),
                        Notes = c.String(),
                        Description = c.String(),
                        ProductSheetUrl = c.String(),
                        PresentationRulesUrl = c.String(),
                        ProductSpecificationUrl = c.String(),
                        MetadataUrl = c.String(),
                        DistributionFormat = c.String(),
                        DistributionUrl = c.String(),
                        DistributionArea = c.String(),
                        WmsUrl = c.String(),
                        ThemeGroupId = c.Int(nullable: false),
                        ThumbnailUrl = c.String(),
                        statusId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Status", t => t.statusId)
                .ForeignKey("dbo.ThemeGroups", t => t.ThemeGroupId, cascadeDelete: true)
                .Index(t => t.ThemeGroupId)
                .Index(t => t.statusId);
            
            CreateTable(
                "dbo.ThemeGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DokDatasets", "ThemeGroupId", "dbo.ThemeGroups");
            DropForeignKey("dbo.DokDatasets", "statusId", "dbo.Status");
            DropIndex("dbo.DokDatasets", new[] { "statusId" });
            DropIndex("dbo.DokDatasets", new[] { "ThemeGroupId" });
            DropTable("dbo.ThemeGroups");
            DropTable("dbo.DokDatasets");
        }
    }
}
