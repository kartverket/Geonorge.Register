namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOtherInfoGeoDataCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "OtherInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "OtherInfo");
        }
    }
}
