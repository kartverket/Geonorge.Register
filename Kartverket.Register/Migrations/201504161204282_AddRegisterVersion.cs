namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddRegisterVersion : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Registers", name: "currentVersion_systemId", newName: "versioningId");
            RenameColumn(table: "dbo.RegisterItems", name: "currentVersionId", newName: "versioningId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_currentVersionId", newName: "IX_versioningId");
            RenameIndex(table: "dbo.Registers", name: "IX_currentVersion_systemId", newName: "IX_versioningId");
            AddColumn("dbo.Registers", "versionNumber", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Registers", "versionNumber");
            RenameIndex(table: "dbo.Registers", name: "IX_versioningId", newName: "IX_currentVersion_systemId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_versioningId", newName: "IX_currentVersionId");
            RenameColumn(table: "dbo.RegisterItems", name: "versioningId", newName: "currentVersionId");
            RenameColumn(table: "dbo.Registers", name: "versioningId", newName: "currentVersion_systemId");
        }
    }
}
