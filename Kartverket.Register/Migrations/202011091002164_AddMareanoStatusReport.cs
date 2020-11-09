namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMareanoStatusReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItemStatusReports", "UuidMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "ProductSpesificationMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "SosiDataMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "GmlDataMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WmsMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WfsMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "AtomFeedMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "CommonStatusMareanoDataset", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "CommonStatusMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "AtomFeedMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WfsMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WmsMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "GmlDataMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "SosiDataMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "ProductSpesificationMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "UuidMareanoDataset");
        }
    }
}
