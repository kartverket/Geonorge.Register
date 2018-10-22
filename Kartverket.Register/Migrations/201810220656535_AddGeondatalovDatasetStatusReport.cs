namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeondatalovDatasetStatusReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItemStatusReports", "UuidGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "MetadataGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "ProductSpesificationGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "SosiDataGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "GmlDataGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WmsGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "WfsGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "AtomFeedGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "CommonStatusGeodatalovDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "InspireTheme", c => c.Boolean());
            AddColumn("dbo.RegisterItemStatusReports", "Dok", c => c.Boolean());
            AddColumn("dbo.RegisterItemStatusReports", "NationalDataset", c => c.Boolean());
            AddColumn("dbo.RegisterItemStatusReports", "Plan", c => c.Boolean());
            AddColumn("dbo.RegisterItemStatusReports", "Geodatalov", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "Geodatalov");
            DropColumn("dbo.RegisterItemStatusReports", "Plan");
            DropColumn("dbo.RegisterItemStatusReports", "NationalDataset");
            DropColumn("dbo.RegisterItemStatusReports", "Dok");
            DropColumn("dbo.RegisterItemStatusReports", "InspireTheme");
            DropColumn("dbo.RegisterItemStatusReports", "CommonStatusGeodatalovDataset");
            DropColumn("dbo.RegisterItemStatusReports", "AtomFeedGeodatalovDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WfsGeodatalovDataset");
            DropColumn("dbo.RegisterItemStatusReports", "WmsGeodatalovDataset");
            DropColumn("dbo.RegisterItemStatusReports", "GmlDataGeodatalovDataset");
            DropColumn("dbo.RegisterItemStatusReports", "SosiDataGeodatalovDataset");
            DropColumn("dbo.RegisterItemStatusReports", "ProductSpesificationGeodatalovDataset");
            DropColumn("dbo.RegisterItemStatusReports", "MetadataGeodatalovDataset");
            DropColumn("dbo.RegisterItemStatusReports", "UuidGeodatalovDataset");
        }
    }
}
