namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertTags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            CreateTable(
                "dbo.AlertTags",
                c => new
                    {
                        AlertRefId = c.Guid(nullable: false),
                        TagRefId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.AlertRefId, t.TagRefId })
                .ForeignKey("dbo.RegisterItems", t => t.AlertRefId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagRefId, cascadeDelete: true)
                .Index(t => t.AlertRefId)
                .Index(t => t.TagRefId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AlertTags", "TagRefId", "dbo.Tags");
            DropForeignKey("dbo.AlertTags", "AlertRefId", "dbo.RegisterItems");
            DropIndex("dbo.AlertTags", new[] { "TagRefId" });
            DropIndex("dbo.AlertTags", new[] { "AlertRefId" });
            DropTable("dbo.AlertTags");
            DropTable("dbo.Tags");
        }
    }
}
