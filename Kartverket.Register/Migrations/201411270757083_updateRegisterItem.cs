namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRegisterItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "datasetowner_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "documentowner_systemId", "dbo.RegisterItems");
            DropIndex("dbo.RegisterItems", new[] { "parent_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "datasetowner_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "documentowner_systemId" });
            AddColumn("dbo.RegisterItems", "datasetthumbnail", c => c.String());
            AddColumn("dbo.RegisterItems", "productsheet", c => c.String());
            AddColumn("dbo.RegisterItems", "presentationRules", c => c.String());
            AddColumn("dbo.RegisterItems", "productspesification", c => c.String());
            AddColumn("dbo.RegisterItems", "metadata", c => c.String());
            AddColumn("dbo.RegisterItems", "distributionUri", c => c.String());
            AddColumn("dbo.RegisterItems", "wmsUrl", c => c.String());
            AddColumn("dbo.RegisterItems", "document", c => c.String());
            AddColumn("dbo.RegisterItems", "externalReference", c => c.String());
            DropColumn("dbo.RegisterItems", "parent_systemId");
            DropColumn("dbo.RegisterItems", "datasetowner_systemId");
            DropColumn("dbo.RegisterItems", "documentowner_systemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItems", "documentowner_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "datasetowner_systemId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "parent_systemId", c => c.Guid());
            DropColumn("dbo.RegisterItems", "externalReference");
            DropColumn("dbo.RegisterItems", "document");
            DropColumn("dbo.RegisterItems", "wmsUrl");
            DropColumn("dbo.RegisterItems", "distributionUri");
            DropColumn("dbo.RegisterItems", "metadata");
            DropColumn("dbo.RegisterItems", "productspesification");
            DropColumn("dbo.RegisterItems", "presentationRules");
            DropColumn("dbo.RegisterItems", "productsheet");
            DropColumn("dbo.RegisterItems", "datasetthumbnail");
            CreateIndex("dbo.RegisterItems", "documentowner_systemId");
            CreateIndex("dbo.RegisterItems", "datasetowner_systemId");
            CreateIndex("dbo.RegisterItems", "parent_systemId");
            AddForeignKey("dbo.RegisterItems", "documentowner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "datasetowner_systemId", "dbo.RegisterItems", "systemId");
            AddForeignKey("dbo.RegisterItems", "parent_systemId", "dbo.RegisterItems", "systemId");
        }
    }
}
