namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeoDataCollectionFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "OtherOrganizationsInvolved", c => c.String());
            AddColumn("dbo.GeoDataCollections", "LinkToMapSolution", c => c.String());
            AddColumn("dbo.GeoDataCollections", "LinkInfoPage", c => c.String());
            AddColumn("dbo.GeoDataCollections", "LinkOtherInfo", c => c.String());
            AddColumn("dbo.GeoDataCollections", "MethodForMappingShort", c => c.String());
            AddColumn("dbo.GeoDataCollections", "OtherWebInfoAboutMappingMethodology", c => c.String());
            AddColumn("dbo.GeoDataCollections", "LinkToRequirementsForDelivery", c => c.String());
            AddColumn("dbo.GeoDataCollections", "OrganizationInfo", c => c.String());
            AddColumn("dbo.GeoDataCollections", "ContactEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "ContactEmail");
            DropColumn("dbo.GeoDataCollections", "OrganizationInfo");
            DropColumn("dbo.GeoDataCollections", "LinkToRequirementsForDelivery");
            DropColumn("dbo.GeoDataCollections", "OtherWebInfoAboutMappingMethodology");
            DropColumn("dbo.GeoDataCollections", "MethodForMappingShort");
            DropColumn("dbo.GeoDataCollections", "LinkOtherInfo");
            DropColumn("dbo.GeoDataCollections", "LinkInfoPage");
            DropColumn("dbo.GeoDataCollections", "LinkToMapSolution");
            DropColumn("dbo.GeoDataCollections", "OtherOrganizationsInvolved");
        }
    }
}
