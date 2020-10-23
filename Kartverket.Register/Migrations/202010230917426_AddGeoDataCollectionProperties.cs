namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeoDataCollectionProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "DatasetTitle", c => c.String());
            AddColumn("dbo.GeoDataCollections", "DatasetLink", c => c.String());
            AddColumn("dbo.GeoDataCollections", "Mapper", c => c.String());
            AddColumn("dbo.GeoDataCollections", "DataOwner", c => c.String());
            AddColumn("dbo.GeoDataCollections", "Distributor", c => c.String());
            AddColumn("dbo.GeoDataCollections", "Methodology", c => c.String());
            AddColumn("dbo.GeoDataCollections", "ProcessHistory", c => c.String());
            AddColumn("dbo.GeoDataCollections", "RegistrationRequirements", c => c.String());
            AddColumn("dbo.GeoDataCollections", "MappingRequirements", c => c.String());
            AddColumn("dbo.GeoDataCollections", "MethodologyDocumentLink", c => c.String());
            AddColumn("dbo.GeoDataCollections", "MethodologyLinkWebPage", c => c.String());
            AddColumn("dbo.GeoDataCollections", "SupportSchemes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "SupportSchemes");
            DropColumn("dbo.GeoDataCollections", "MethodologyLinkWebPage");
            DropColumn("dbo.GeoDataCollections", "MethodologyDocumentLink");
            DropColumn("dbo.GeoDataCollections", "MappingRequirements");
            DropColumn("dbo.GeoDataCollections", "RegistrationRequirements");
            DropColumn("dbo.GeoDataCollections", "ProcessHistory");
            DropColumn("dbo.GeoDataCollections", "Methodology");
            DropColumn("dbo.GeoDataCollections", "Distributor");
            DropColumn("dbo.GeoDataCollections", "DataOwner");
            DropColumn("dbo.GeoDataCollections", "Mapper");
            DropColumn("dbo.GeoDataCollections", "DatasetLink");
            DropColumn("dbo.GeoDataCollections", "DatasetTitle");
        }
    }
}
