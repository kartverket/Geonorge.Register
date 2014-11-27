namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRegisterItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegisterItems",
                c => new
                    {
                        systemId = c.Guid(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        dateSubmitted = c.DateTime(nullable: false),
                        modified = c.DateTime(nullable: false),
                        dateAccepted = c.DateTime(),
                        value = c.String(),
                        distributionFormat = c.String(),
                        distributionArea = c.String(),
                        metadataUuid = c.String(),
                        number = c.String(),
                        logoFilename = c.String(),
                        thumbnail = c.String(),
                        epsg = c.String(),
                        sosiReferencesystem = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        currentVersion_systemId = c.Guid(),
                        status_value = c.String(maxLength: 128),
                        parent_systemId = c.Guid(),
                        datasetowner_systemId = c.Guid(),
                        theme_value = c.String(maxLength: 128),
                        documentowner_systemId = c.Guid(),
                        Register_systemId = c.Guid(),
                    })
                .PrimaryKey(t => t.systemId)
                .ForeignKey("dbo.Versions", t => t.currentVersion_systemId)
                .ForeignKey("dbo.Status", t => t.status_value)
                .ForeignKey("dbo.RegisterItems", t => t.parent_systemId)
                .ForeignKey("dbo.RegisterItems", t => t.datasetowner_systemId)
                .ForeignKey("dbo.DOKThemes", t => t.theme_value)
                .ForeignKey("dbo.RegisterItems", t => t.documentowner_systemId)
                .ForeignKey("dbo.Registers", t => t.Register_systemId)
                .Index(t => t.currentVersion_systemId)
                .Index(t => t.status_value)
                .Index(t => t.parent_systemId)
                .Index(t => t.datasetowner_systemId)
                .Index(t => t.theme_value)
                .Index(t => t.documentowner_systemId)
                .Index(t => t.Register_systemId);
            
            CreateTable(
                "dbo.DOKThemes",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "Register_systemId", "dbo.Registers");
            DropForeignKey("dbo.RegisterItems", "documentowner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "theme_value", "dbo.DOKThemes");
            DropForeignKey("dbo.RegisterItems", "datasetowner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "status_value", "dbo.Status");
            DropForeignKey("dbo.RegisterItems", "currentVersion_systemId", "dbo.Versions");
            DropIndex("dbo.RegisterItems", new[] { "Register_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "documentowner_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "theme_value" });
            DropIndex("dbo.RegisterItems", new[] { "datasetowner_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "parent_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "status_value" });
            DropIndex("dbo.RegisterItems", new[] { "currentVersion_systemId" });
            DropTable("dbo.DOKThemes");
            DropTable("dbo.RegisterItems");
        }
    }
}
