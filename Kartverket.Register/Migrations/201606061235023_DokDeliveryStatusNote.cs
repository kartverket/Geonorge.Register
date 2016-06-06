namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DokDeliveryStatusNote : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "dokDeliveryMetadataStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryProductSheetStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryWmsStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryWfsStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryDistributionStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryServiceAlertStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryGeodataLawStatusNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "dokDeliveryGeodataLawStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryServiceAlertStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryDistributionStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryWfsStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryWmsStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryProductSheetStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryMetadataStatusNote");
        }
    }
}
