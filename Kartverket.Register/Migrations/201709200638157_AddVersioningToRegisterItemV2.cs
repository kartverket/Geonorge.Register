namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersioningToRegisterItemV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDatasets", "VersioningId", c => c.Guid(nullable: false));
            CreateIndex("dbo.InspireDatasets", "VersioningId");
            AddForeignKey("dbo.InspireDatasets", "VersioningId", "dbo.Versions", "systemId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspireDatasets", "VersioningId", "dbo.Versions");
            DropIndex("dbo.InspireDatasets", new[] { "VersioningId" });
            DropColumn("dbo.InspireDatasets", "VersioningId");
        }
    }
}
