namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeodataCollectionPurpose : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "Purpose", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "Purpose");
        }
    }
}
