namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersionNumber : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RegisterItems", name: "currentVersion_systemId", newName: "currentVersionId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_currentVersion_systemId", newName: "IX_currentVersionId");
            AddColumn("dbo.RegisterItems", "versionNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "versionNumber");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_currentVersionId", newName: "IX_currentVersion_systemId");
            RenameColumn(table: "dbo.RegisterItems", name: "currentVersionId", newName: "currentVersion_systemId");
        }
    }
}
