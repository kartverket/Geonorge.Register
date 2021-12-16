namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMareanoProductSheetAndPresentationRule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItemStatusReports", "ProductSheetMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "PresentationRulesMareanoDataset", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "PresentationRulesMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "ProductSheetMareanoDataset");
        }
    }
}
