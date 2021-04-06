namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAidAndSubsidiesGeoDataCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "AidAndSubsidies", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "AidAndSubsidies");
        }
    }
}
