namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeoDataCollection : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GeoDataCollections",
                c => new
                    {
                        systemId = c.Guid(nullable: false),
                        Title = c.String(),
                        Link = c.String(),
                        Organization_systemId = c.Guid(),
                    })
                .PrimaryKey(t => t.systemId)
                .ForeignKey("dbo.RegisterItems", t => t.Organization_systemId)
                .Index(t => t.Organization_systemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GeoDataCollections", "Organization_systemId", "dbo.RegisterItems");
            DropIndex("dbo.GeoDataCollections", new[] { "Organization_systemId" });
            DropTable("dbo.GeoDataCollections");
        }
    }
}
