namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addForeignKeyRegisteritem : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RegisterItems", new[] { "submitter_systemId" });
            RenameColumn(table: "dbo.RegisterItems", name: "submitter_systemId", newName: "submitterId");
            RenameColumn(table: "dbo.RegisterItems", name: "datasetowner_systemId", newName: "datasetownerId");
            RenameColumn(table: "dbo.RegisterItems", name: "documentowner_systemId", newName: "documentownerId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_datasetowner_systemId", newName: "IX_datasetownerId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_documentowner_systemId", newName: "IX_documentownerId");
            AlterColumn("dbo.RegisterItems", "submitterId", c => c.Guid(nullable: true));
            CreateIndex("dbo.RegisterItems", "submitterId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RegisterItems", new[] { "submitterId" });
            AlterColumn("dbo.RegisterItems", "submitterId", c => c.Guid());
            RenameIndex(table: "dbo.RegisterItems", name: "IX_documentownerId", newName: "IX_documentowner_systemId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_datasetownerId", newName: "IX_datasetowner_systemId");
            RenameColumn(table: "dbo.RegisterItems", name: "documentownerId", newName: "documentowner_systemId");
            RenameColumn(table: "dbo.RegisterItems", name: "datasetownerId", newName: "datasetowner_systemId");
            RenameColumn(table: "dbo.RegisterItems", name: "submitterId", newName: "submitter_systemId");
            CreateIndex("dbo.RegisterItems", "submitter_systemId");
        }
    }
}
