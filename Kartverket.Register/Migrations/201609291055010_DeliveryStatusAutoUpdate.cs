namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeliveryStatusAutoUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "dokDeliveryMetadataStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliveryProductSheetStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliveryWmsStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliveryWfsStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliveryDistributionStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliverySosiStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliveryGmlRequirementsStatusAutoUpdate", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "dokDeliveryAtomFeedStatusAutoUpdate", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "dokDeliveryAtomFeedStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliveryGmlRequirementsStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliverySosiStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliveryDistributionStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliveryWfsStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliveryWmsStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliveryProductSheetStatusAutoUpdate");
            DropColumn("dbo.RegisterItems", "dokDeliveryMetadataStatusAutoUpdate");
        }
    }
}
