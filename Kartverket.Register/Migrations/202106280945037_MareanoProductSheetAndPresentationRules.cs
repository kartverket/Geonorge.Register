namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MareanoProductSheetAndPresentationRules : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MareanoDatasets", "ProductSheetStatusId", c => c.Guid(nullable: true));
            AddColumn("dbo.MareanoDatasets", "PresentationRulesStatusId", c => c.Guid(nullable: true));
            CreateIndex("dbo.MareanoDatasets", "ProductSheetStatusId");
            CreateIndex("dbo.MareanoDatasets", "PresentationRulesStatusId");
            AddForeignKey("dbo.MareanoDatasets", "ProductSheetStatusId", "dbo.DatasetDeliveries", "DatasetDeliveryId", cascadeDelete: false);
            AddForeignKey("dbo.MareanoDatasets", "PresentationRulesStatusId", "dbo.DatasetDeliveries", "DatasetDeliveryId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MareanoDatasets", "PresentationRulesStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "ProductSheetStatusId", "dbo.DatasetDeliveries");
            DropIndex("dbo.MareanoDatasets", new[] { "PresentationRulesStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "ProductSheetStatusId" });
            DropColumn("dbo.MareanoDatasets", "PresentationRulesStatusId");
            DropColumn("dbo.MareanoDatasets", "ProductSheetStatusId");
        }
    }
}
