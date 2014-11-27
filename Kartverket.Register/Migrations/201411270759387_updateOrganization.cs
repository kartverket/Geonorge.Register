namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrganization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "submitter_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "datasetowner_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "documentowner_systemId", c => c.Guid());
            AddColumn("dbo.Registers", "manager_systemId", c => c.Guid());
            AddColumn("dbo.Registers", "owner_systemId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "submitter_systemId");
            CreateIndex("dbo.RegisterItems", "datasetowner_systemId");
            CreateIndex("dbo.RegisterItems", "documentowner_systemId");
            CreateIndex("dbo.Registers", "manager_systemId");
            CreateIndex("dbo.Registers", "owner_systemId");
            AddForeignKey("dbo.RegisterItems", "submitter_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "datasetowner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "documentowner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registers", "owner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.Registers", "manager_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "documentowner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "datasetowner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "submitter_systemId", "dbo.RegisterItems");
            DropIndex("dbo.Registers", new[] { "owner_systemId" });
            DropIndex("dbo.Registers", new[] { "manager_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "documentowner_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "datasetowner_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "submitter_systemId" });
            DropColumn("dbo.Registers", "owner_systemId");
            DropColumn("dbo.Registers", "manager_systemId");
            DropColumn("dbo.RegisterItems", "documentowner_systemId");
            DropColumn("dbo.RegisterItems", "datasetowner_systemId");
            DropColumn("dbo.RegisterItems", "submitter_systemId");
        }
    }
}
