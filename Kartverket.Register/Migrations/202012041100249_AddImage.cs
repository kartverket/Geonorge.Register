namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "ImageFileName", c => c.String());
            AddColumn("dbo.GeoDataCollections", "ThumbnailFileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "ThumbnailFileName");
            DropColumn("dbo.GeoDataCollections", "ImageFileName");
        }
    }
}
