namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFairReport : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RegisterItemStatusReports", name: "Grade", newName: "Grade1");
            AddColumn("dbo.RegisterItemStatusReports", "UuidFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "FindableFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "AccesibleFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "InteroperableFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "ReUsableFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "ProductSpesificationFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "ProductSheetFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "PresentationRulesFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "SosiDataFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "GmlDataFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WmsFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WfsFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "AtomFeedFairDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "CommonStatusFairDataset", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "CommonStatusFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "AtomFeedFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WfsFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WmsFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "GmlDataFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "SosiDataFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "PresentationRulesFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "ProductSheetFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "ProductSpesificationFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "ReUsableFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "InteroperableFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "AccesibleFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "FindableFairDataset");
            DropColumn("dbo.RegisterItemStatusReports", "UuidFairDataset");
            RenameColumn(table: "dbo.RegisterItemStatusReports", name: "Grade1", newName: "Grade");
        }
    }
}
