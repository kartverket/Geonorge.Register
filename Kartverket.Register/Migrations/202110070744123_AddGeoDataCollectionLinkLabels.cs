namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeoDataCollectionLinkLabels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "LinkLabel", c => c.String());
            AddColumn("dbo.GeoDataCollections", "ProcessHistoryLabel", c => c.String());
            AddColumn("dbo.GeoDataCollections", "MappingRequirementsLinkLabel", c => c.String());
            AddColumn("dbo.GeoDataCollections", "LinkToMapSolutionLabel", c => c.String());
            AddColumn("dbo.GeoDataCollections", "LinkInfoPageLabel", c => c.String());
            AddColumn("dbo.GeoDataCollections", "OtherWebInfoAboutMappingMethodologyLabel", c => c.String());
            AddColumn("dbo.GeoDataCollections", "LinkToRequirementsForDeliveryLabel", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "LinkToRequirementsForDeliveryLabel");
            DropColumn("dbo.GeoDataCollections", "OtherWebInfoAboutMappingMethodologyLabel");
            DropColumn("dbo.GeoDataCollections", "LinkInfoPageLabel");
            DropColumn("dbo.GeoDataCollections", "LinkToMapSolutionLabel");
            DropColumn("dbo.GeoDataCollections", "MappingRequirementsLinkLabel");
            DropColumn("dbo.GeoDataCollections", "ProcessHistoryLabel");
            DropColumn("dbo.GeoDataCollections", "LinkLabel");
        }
    }
}
