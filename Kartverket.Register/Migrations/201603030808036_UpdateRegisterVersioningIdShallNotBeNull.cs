namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRegisterVersioningIdShallNotBeNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RegisterItems", "versioningId", "dbo.Versions");
            DropIndex("dbo.RegisterItems", new[] { "versioningId" });
            AlterColumn("dbo.RegisterItems", "versioningId", c => c.Guid(nullable: false));
            CreateIndex("dbo.RegisterItems", "versioningId");
            AddForeignKey("dbo.RegisterItems", "versioningId", "dbo.Versions", "systemId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "versioningId", "dbo.Versions");
            DropIndex("dbo.RegisterItems", new[] { "versioningId" });
            AlterColumn("dbo.RegisterItems", "versioningId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "versioningId");
            AddForeignKey("dbo.RegisterItems", "versioningId", "dbo.Versions", "systemId");
        }
    }
}
