namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMunicipalityFieldsToOrganization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "OrganizationType", c => c.String());
            AddColumn("dbo.RegisterItems", "MunicipalityCode", c => c.String());
            AddColumn("dbo.RegisterItems", "GeographicCenterX", c => c.String());
            AddColumn("dbo.RegisterItems", "GeographicCenterY", c => c.String());
            AddColumn("dbo.RegisterItems", "BoundingBoxNorth", c => c.String());
            AddColumn("dbo.RegisterItems", "BoundingBoxSouth", c => c.String());
            AddColumn("dbo.RegisterItems", "BoundingBoxEast", c => c.String());
            AddColumn("dbo.RegisterItems", "BoundingBoxWest", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "BoundingBoxWest");
            DropColumn("dbo.RegisterItems", "BoundingBoxEast");
            DropColumn("dbo.RegisterItems", "BoundingBoxSouth");
            DropColumn("dbo.RegisterItems", "BoundingBoxNorth");
            DropColumn("dbo.RegisterItems", "GeographicCenterY");
            DropColumn("dbo.RegisterItems", "GeographicCenterX");
            DropColumn("dbo.RegisterItems", "MunicipalityCode");
            DropColumn("dbo.RegisterItems", "OrganizationType");
        }
    }
}
