namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResponsibleGeoDataCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GeoDataCollections", "Responsible_systemId", c => c.Guid());
            CreateIndex("dbo.GeoDataCollections", "Responsible_systemId");
            AddForeignKey("dbo.GeoDataCollections", "Responsible_systemId", "dbo.RegisterItems", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GeoDataCollections", "Responsible_systemId", "dbo.RegisterItems");
            DropIndex("dbo.GeoDataCollections", new[] { "Responsible_systemId" });
            DropColumn("dbo.GeoDataCollections", "Responsible_systemId");
        }
    }
}
