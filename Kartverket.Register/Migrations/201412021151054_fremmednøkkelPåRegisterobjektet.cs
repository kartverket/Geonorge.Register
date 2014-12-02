namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fremmednøkkelPåRegisterobjektet : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems");
            DropIndex("dbo.Registers", new[] { "manager_systemId" });
            DropIndex("dbo.Registers", new[] { "owner_systemId" });
            RenameColumn(table: "dbo.Registers", name: "manager_systemId", newName: "managerId");
            RenameColumn(table: "dbo.Registers", name: "owner_systemId", newName: "ownerId");
            RenameColumn(table: "dbo.Registers", name: "status_value", newName: "statusId");
            RenameIndex(table: "dbo.Registers", name: "IX_status_value", newName: "IX_statusId");
            AlterColumn("dbo.Registers", "managerId", c => c.Guid(nullable: true));
            AlterColumn("dbo.Registers", "ownerId", c => c.Guid(nullable: true));
            CreateIndex("dbo.Registers", "ownerId");
            CreateIndex("dbo.Registers", "managerId");
            AddForeignKey("dbo.Registers", "managerId", "dbo.RegisterItems", "systemId", cascadeDelete: false);
            AddForeignKey("dbo.Registers", "ownerId", "dbo.RegisterItems", "systemId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registers", "ownerId", "dbo.RegisterItems");
            DropForeignKey("dbo.Registers", "managerId", "dbo.RegisterItems");
            DropIndex("dbo.Registers", new[] { "managerId" });
            DropIndex("dbo.Registers", new[] { "ownerId" });
            AlterColumn("dbo.Registers", "ownerId", c => c.Guid());
            AlterColumn("dbo.Registers", "managerId", c => c.Guid());
            RenameIndex(table: "dbo.Registers", name: "IX_statusId", newName: "IX_status_value");
            RenameColumn(table: "dbo.Registers", name: "statusId", newName: "status_value");
            RenameColumn(table: "dbo.Registers", name: "ownerId", newName: "owner_systemId");
            RenameColumn(table: "dbo.Registers", name: "managerId", newName: "manager_systemId");
            CreateIndex("dbo.Registers", "owner_systemId");
            CreateIndex("dbo.Registers", "manager_systemId");
            AddForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems", "systemId");
        }
    }
}
