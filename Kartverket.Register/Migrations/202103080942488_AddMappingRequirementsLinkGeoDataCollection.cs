namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMappingRequirementsLinkGeoDataCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "MappingRequirementsLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "MappingRequirementsLink");
        }
    }
}
