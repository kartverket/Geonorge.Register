namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAlertRenameAlert : DbMigration
    {
        public override void Up()
        {
            Sql("update [dbo].[Registers] set containedItemClass = 'Alert' where containedItemClass = 'ServiceAlert'");
            Sql("update [dbo].[RegisterItems] set Discriminator = 'Alert' where Discriminator = 'ServiceAlert'");

            RenameColumn("dbo.RegisterItems", "ServiceType", "Type");
            RenameColumn("dbo.RegisterItems", "ServiceMetadataUrl", "UrlExternal");
            RenameColumn("dbo.RegisterItems", "ServiceUuid", "UuidExternal");

            //AddColumn("dbo.RegisterItems", "Type", c => c.String());
            //AddColumn("dbo.RegisterItems", "UrlExternal", c => c.String());
            //AddColumn("dbo.RegisterItems", "UuidExternal", c => c.String());
            //DropColumn("dbo.RegisterItems", "ServiceType");
            //DropColumn("dbo.RegisterItems", "ServiceMetadataUrl");
            //DropColumn("dbo.RegisterItems", "ServiceUuid");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.RegisterItems", "Type", "ServiceType");
            RenameColumn("dbo.RegisterItems", "UrlExternal", "ServiceMetadataUrl");
            RenameColumn("dbo.RegisterItems", "UuidExternal", "ServiceUuid");

            Sql("update [dbo].[Registers] set containedItemClass = 'ServiceAlert' where containedItemClass = 'Alert'");
            Sql("update [dbo].[RegisterItems] set Discriminator = 'ServiceAlert' where Discriminator = 'Alert'");

            //AddColumn("dbo.RegisterItems", "ServiceUuid", c => c.String());
            //AddColumn("dbo.RegisterItems", "ServiceMetadataUrl", c => c.String());
            //AddColumn("dbo.RegisterItems", "ServiceType", c => c.String());
            //DropColumn("dbo.RegisterItems", "UuidExternal");
            //DropColumn("dbo.RegisterItems", "UrlExternal");
            //DropColumn("dbo.RegisterItems", "Type");
        }
    }
}
