namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addversion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Versions",
                c => new
                    {
                        systemId = c.Guid(nullable: false),
                        versionInfo = c.String(),
                    })
                .PrimaryKey(t => t.systemId);
            
            AddColumn("dbo.Registers", "currentVersion_systemId", c => c.Guid());
            CreateIndex("dbo.Registers", "currentVersion_systemId");
            AddForeignKey("dbo.Registers", "currentVersion_systemId", "dbo.Versions", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registers", "currentVersion_systemId", "dbo.Versions");
            DropIndex("dbo.Registers", new[] { "currentVersion_systemId" });
            DropColumn("dbo.Registers", "currentVersion_systemId");
            DropTable("dbo.Versions");
        }
    }
}
