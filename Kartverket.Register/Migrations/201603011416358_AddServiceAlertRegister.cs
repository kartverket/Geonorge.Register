namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddServiceAlertRegister : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "AlertDate", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "EffectiveDate", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "AlertType", c => c.String());
            AddColumn("dbo.RegisterItems", "ServiceType", c => c.String());
            AddColumn("dbo.RegisterItems", "OwnerId", c => c.Guid());
            AddColumn("dbo.RegisterItems", "Note", c => c.String());
            AddColumn("dbo.RegisterItems", "ServiceMetadataUrl", c => c.String());
            AddColumn("dbo.RegisterItems", "ServiceUuid", c => c.String());
            CreateIndex("dbo.RegisterItems", "OwnerId");
            AddForeignKey("dbo.RegisterItems", "OwnerId", "dbo.RegisterItems", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "OwnerId", "dbo.RegisterItems");
            DropIndex("dbo.RegisterItems", new[] { "OwnerId" });
            DropColumn("dbo.RegisterItems", "ServiceUuid");
            DropColumn("dbo.RegisterItems", "ServiceMetadataUrl");
            DropColumn("dbo.RegisterItems", "Note");
            DropColumn("dbo.RegisterItems", "OwnerId");
            DropColumn("dbo.RegisterItems", "ServiceType");
            DropColumn("dbo.RegisterItems", "AlertType");
            DropColumn("dbo.RegisterItems", "EffectiveDate");
            DropColumn("dbo.RegisterItems", "AlertDate");
        }
    }
}
