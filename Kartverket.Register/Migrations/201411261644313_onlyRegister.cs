namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onlyRegister : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Registers", "currentVersion_systemId", "dbo.Versions");
            DropForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.Registers", "parentRegister_systemId", "dbo.Registers");
            DropForeignKey("dbo.Registers", "status_value", "dbo.Status");
            DropForeignKey("dbo.RegisterItems", "currentVersion_systemId", "dbo.Versions");
            DropForeignKey("dbo.RegisterItems", "register_systemId", "dbo.Registers");
            DropForeignKey("dbo.RegisterItems", "status_value", "dbo.Status");
            DropForeignKey("dbo.RegisterItems", "submitter_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "owner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "theme_value", "dbo.DOKThemes");
            DropForeignKey("dbo.RegisterItems", "owner_systemId1", "dbo.RegisterItems");
            DropIndex("dbo.RegisterItems", new[] { "currentVersion_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "register_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "status_value" });
            DropIndex("dbo.RegisterItems", new[] { "submitter_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "parent_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "owner_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "theme_value" });
            DropIndex("dbo.RegisterItems", new[] { "owner_systemId1" });
            DropIndex("dbo.Registers", new[] { "currentVersion_systemId" });
            DropIndex("dbo.Registers", new[] { "manager_systemId" });
            DropIndex("dbo.Registers", new[] { "owner_systemId" });
            DropIndex("dbo.Registers", new[] { "parentRegister_systemId" });
            DropIndex("dbo.Registers", new[] { "status_value" });
            DropColumn("dbo.Registers", "currentVersion_systemId");
            DropColumn("dbo.Registers", "manager_systemId");
            DropColumn("dbo.Registers", "owner_systemId");
            DropColumn("dbo.Registers", "parentRegister_systemId");
            DropColumn("dbo.Registers", "status_value");
            DropTable("dbo.DOKThemes");
            DropTable("dbo.RegisterItems");
            DropTable("dbo.Versions");
            DropTable("dbo.Status");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            CreateTable(
                "dbo.Versions",
                c => new
                    {
                        systemId = c.Guid(nullable: false),
                        versionInfo = c.String(),
                    })
                .PrimaryKey(t => t.systemId);
            
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
                        number = c.String(),
                        logoFilename = c.String(),
                        value = c.String(),
                        distributionFormat = c.String(),
                        distributionArea = c.String(),
                        metadataUuid = c.String(),
                        thumbnail = c.String(),
                        epsg = c.String(),
                        sosiReferencesystem = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        currentVersion_systemId = c.Guid(),
                        register_systemId = c.Guid(),
                        status_value = c.String(maxLength: 128),
                        submitter_systemId = c.Guid(),
                        parent_systemId = c.Guid(),
                        owner_systemId = c.Guid(),
                        theme_value = c.String(maxLength: 128),
                        owner_systemId1 = c.Guid(),
                    })
                .PrimaryKey(t => t.systemId);
            
            CreateTable(
                "dbo.DOKThemes",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.Registers", "status_value", c => c.String(maxLength: 128));
            AddColumn("dbo.Registers", "parentRegister_systemId", c => c.Guid());
            AddColumn("dbo.Registers", "owner_systemId", c => c.Guid());
            AddColumn("dbo.Registers", "manager_systemId", c => c.Guid());
            AddColumn("dbo.Registers", "currentVersion_systemId", c => c.Guid());
            CreateIndex("dbo.Registers", "status_value");
            CreateIndex("dbo.Registers", "parentRegister_systemId");
            CreateIndex("dbo.Registers", "owner_systemId");
            CreateIndex("dbo.Registers", "manager_systemId");
            CreateIndex("dbo.Registers", "currentVersion_systemId");
            CreateIndex("dbo.RegisterItems", "owner_systemId1");
            CreateIndex("dbo.RegisterItems", "theme_value");
            CreateIndex("dbo.RegisterItems", "owner_systemId");
            CreateIndex("dbo.RegisterItems", "parent_systemId");
            CreateIndex("dbo.RegisterItems", "submitter_systemId");
            CreateIndex("dbo.RegisterItems", "status_value");
            CreateIndex("dbo.RegisterItems", "register_systemId");
            CreateIndex("dbo.RegisterItems", "currentVersion_systemId");
            AddForeignKey("dbo.RegisterItems", "owner_systemId1", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "theme_value", "dbo.DOKThemes", "value");
            AddForeignKey("dbo.RegisterItems", "owner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "submitter_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "status_value", "dbo.Status", "value");
            AddForeignKey("dbo.RegisterItems", "register_systemId", "dbo.Registers", "systemId");
            AddForeignKey("dbo.RegisterItems", "currentVersion_systemId", "dbo.Versions", "systemId");
            AddForeignKey("dbo.Registers", "status_value", "dbo.Status", "value");
            AddForeignKey("dbo.Registers", "parentRegister_systemId", "dbo.Registers", "systemId");
            AddForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.Registers", "currentVersion_systemId", "dbo.Versions", "systemId");
        }
    }
}
