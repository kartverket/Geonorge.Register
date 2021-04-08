namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOwnerForGeodataCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "Owner", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeoDataCollections", "Owner");
        }
    }
}
