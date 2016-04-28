namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DokDeliveryStatusDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "dokDeliveryProductSheetStatusId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "dokDeliveryWmsStatusId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "dokDeliveryWfsStatusId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "dokDeliveryDistributionStatusId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "dokDeliveryServiceAlertStatusId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "dokDeliveryGeodataLawStatusId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RegisterItems", "dokDeliveryProductSheetStatusId");
            CreateIndex("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusId");
            CreateIndex("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusId");
            CreateIndex("dbo.RegisterItems", "dokDeliveryWmsStatusId");
            CreateIndex("dbo.RegisterItems", "dokDeliveryWfsStatusId");
            CreateIndex("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusId");
            CreateIndex("dbo.RegisterItems", "dokDeliveryDistributionStatusId");
            CreateIndex("dbo.RegisterItems", "dokDeliveryServiceAlertStatusId");
            CreateIndex("dbo.RegisterItems", "dokDeliveryGeodataLawStatusId");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryDistributionStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryGeodataLawStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryProductSheetStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryServiceAlertStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryWfsStatusId", "dbo.DokDeliveryStatus", "value");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryWmsStatusId", "dbo.DokDeliveryStatus", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "dokDeliveryWmsStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.RegisterItems", "dokDeliveryWfsStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.RegisterItems", "dokDeliveryServiceAlertStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.RegisterItems", "dokDeliveryProductSheetStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.RegisterItems", "dokDeliveryGeodataLawStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.RegisterItems", "dokDeliveryDistributionStatusId", "dbo.DokDeliveryStatus");
            DropForeignKey("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusId", "dbo.DokDeliveryStatus");
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryGeodataLawStatusId" });
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryServiceAlertStatusId" });
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryDistributionStatusId" });
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryDistributionAreaStatusId" });
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryWfsStatusId" });
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryWmsStatusId" });
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryProductSpecificationStatusId" });
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryPresentationRulesStatusId" });
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryProductSheetStatusId" });
            DropColumn("dbo.RegisterItems", "dokDeliveryGeodataLawStatusId");
            DropColumn("dbo.RegisterItems", "dokDeliveryServiceAlertStatusId");
            DropColumn("dbo.RegisterItems", "dokDeliveryDistributionStatusId");
            DropColumn("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusId");
            DropColumn("dbo.RegisterItems", "dokDeliveryWfsStatusId");
            DropColumn("dbo.RegisterItems", "dokDeliveryWmsStatusId");
            DropColumn("dbo.RegisterItems", "dokDeliveryProductSpecificationStatusId");
            DropColumn("dbo.RegisterItems", "dokDeliveryPresentationRulesStatusId");
            DropColumn("dbo.RegisterItems", "dokDeliveryProductSheetStatusId");
        }
    }
}
