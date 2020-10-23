namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeoNameGeoDataCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "SeoName", c => c.String(maxLength: 255));
            CreateIndex("dbo.GeoDataCollections", "SeoName", unique: true, name: "SeoName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.GeoDataCollections", "SeoName");
            DropColumn("dbo.GeoDataCollections", "SeoName");
        }
    }
}
