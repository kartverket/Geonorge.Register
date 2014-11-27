namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateAllRegisters : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Organizations", newName: "RegisterItems");
            DropForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "submitter_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "owner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "owner_systemId1", "dbo.RegisterItems");
            DropPrimaryKey("dbo.RegisterItems");
            CreateTable(
                "dbo.DOKThemes",
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
                "dbo.Registers",
                c => new
                    {
                        systemId = c.Guid(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        dateSubmitted = c.DateTime(nullable: false),
                        modified = c.DateTime(nullable: false),
                        dateAccepted = c.DateTime(),
                        containedItemClass = c.String(),
                        currentVersion_systemId = c.Guid(),
                        manager_systemId = c.Guid(),
                        owner_systemId = c.Guid(),
                        parentRegister_systemId = c.Guid(),
                        status_value = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.systemId)
                .ForeignKey("dbo.Versions", t => t.currentVersion_systemId)
                .ForeignKey("dbo.RegisterItems", t => t.manager_systemId)
                .ForeignKey("dbo.RegisterItems", t => t.owner_systemId)
                .ForeignKey("dbo.Registers", t => t.parentRegister_systemId)
                .ForeignKey("dbo.Status", t => t.status_value)
                .Index(t => t.currentVersion_systemId)
                .Index(t => t.manager_systemId)
                .Index(t => t.owner_systemId)
                .Index(t => t.parentRegister_systemId)
                .Index(t => t.status_value);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.RegisterItems", "systemId", c => c.Guid(nullable: false));
            AddColumn("dbo.RegisterItems", "description", c => c.String());
            AddColumn("dbo.RegisterItems", "dateSubmitted", c => c.DateTime(nullable: false));
            AddColumn("dbo.RegisterItems", "modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.RegisterItems", "dateAccepted", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "value", c => c.String());
            AddColumn("dbo.RegisterItems", "distributionFormat", c => c.String());
            AddColumn("dbo.RegisterItems", "distributionArea", c => c.String());
            AddColumn("dbo.RegisterItems", "metadataUuid", c => c.String());
            AddColumn("dbo.RegisterItems", "datasetthumbnail", c => c.String());
            AddColumn("dbo.RegisterItems", "epsg", c => c.String());
            AddColumn("dbo.RegisterItems", "sosiReferencesystem", c => c.String());
            AddColumn("dbo.RegisterItems", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.RegisterItems", "currentVersion_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "register_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "status_value", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "submitter_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "parent_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "owner_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "theme_value", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "owner_systemId1", c => c.Guid());
            AlterColumn("dbo.RegisterItems", "number", c => c.String());
            AddPrimaryKey("dbo.RegisterItems", "systemId");
            CreateIndex("dbo.RegisterItems", "currentVersion_systemId");
            CreateIndex("dbo.RegisterItems", "register_systemId");
            CreateIndex("dbo.RegisterItems", "status_value");
            CreateIndex("dbo.RegisterItems", "submitter_systemId");
            CreateIndex("dbo.RegisterItems", "parent_systemId");
            CreateIndex("dbo.RegisterItems", "owner_systemId");
            CreateIndex("dbo.RegisterItems", "theme_value");
            CreateIndex("dbo.RegisterItems", "owner_systemId1");
            AddForeignKey("dbo.RegisterItems", "currentVersion_systemId", "dbo.Versions", "systemId");
            AddForeignKey("dbo.RegisterItems", "register_systemId", "dbo.Registers", "systemId");
            AddForeignKey("dbo.RegisterItems", "status_value", "dbo.Status", "value");
            AddForeignKey("dbo.RegisterItems", "submitter_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "owner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "theme_value", "dbo.DOKThemes", "value");
            AddForeignKey("dbo.RegisterItems", "owner_systemId1", "dbo.RegisterItems", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "owner_systemId1", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "theme_value", "dbo.DOKThemes");
            DropForeignKey("dbo.RegisterItems", "owner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "submitter_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "status_value", "dbo.Status");
            DropForeignKey("dbo.RegisterItems", "register_systemId", "dbo.Registers");
            DropForeignKey("dbo.RegisterItems", "currentVersion_systemId", "dbo.Versions");
            DropForeignKey("dbo.Registers", "status_value", "dbo.Status");
            DropForeignKey("dbo.Registers", "parentRegister_systemId", "dbo.Registers");
            DropForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.Registers", "currentVersion_systemId", "dbo.Versions");
            DropIndex("dbo.Registers", new[] { "status_value" });
            DropIndex("dbo.Registers", new[] { "parentRegister_systemId" });
            DropIndex("dbo.Registers", new[] { "owner_systemId" });
            DropIndex("dbo.Registers", new[] { "manager_systemId" });
            DropIndex("dbo.Registers", new[] { "currentVersion_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "owner_systemId1" });
            DropIndex("dbo.RegisterItems", new[] { "theme_value" });
            DropIndex("dbo.RegisterItems", new[] { "owner_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "parent_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "submitter_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "status_value" });
            DropIndex("dbo.RegisterItems", new[] { "register_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "currentVersion_systemId" });
            DropPrimaryKey("dbo.RegisterItems");
            AlterColumn("dbo.RegisterItems", "number", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.RegisterItems", "owner_systemId1");
            DropColumn("dbo.RegisterItems", "theme_value");
            DropColumn("dbo.RegisterItems", "owner_systemId");
            DropColumn("dbo.RegisterItems", "parent_systemId");
            DropColumn("dbo.RegisterItems", "submitter_systemId");
            DropColumn("dbo.RegisterItems", "status_value");
            DropColumn("dbo.RegisterItems", "register_systemId");
            DropColumn("dbo.RegisterItems", "currentVersion_systemId");
            DropColumn("dbo.RegisterItems", "Discriminator");
            DropColumn("dbo.RegisterItems", "sosiReferencesystem");
            DropColumn("dbo.RegisterItems", "epsg");
            DropColumn("dbo.RegisterItems", "datasetthumbnail");
            DropColumn("dbo.RegisterItems", "metadataUuid");
            DropColumn("dbo.RegisterItems", "distributionArea");
            DropColumn("dbo.RegisterItems", "distributionFormat");
            DropColumn("dbo.RegisterItems", "value");
            DropColumn("dbo.RegisterItems", "dateAccepted");
            DropColumn("dbo.RegisterItems", "modified");
            DropColumn("dbo.RegisterItems", "dateSubmitted");
            DropColumn("dbo.RegisterItems", "description");
            DropColumn("dbo.RegisterItems", "systemId");
            DropTable("dbo.Status");
            DropTable("dbo.Registers");
            DropTable("dbo.Versions");
            DropTable("dbo.DOKThemes");
            AddPrimaryKey("dbo.RegisterItems", "Number");
            AddForeignKey("dbo.RegisterItems", "owner_systemId1", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "owner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "submitter_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems", "systemId");
            RenameTable(name: "dbo.RegisterItems", newName: "Organizations");
        }
    }
}
