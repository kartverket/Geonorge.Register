namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertExtendedProperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                        group = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
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
            
            AddColumn("dbo.RegisterItems", "departmentId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "stationId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "Summary", c => c.String());
            AddColumn("dbo.RegisterItems", "Link", c => c.String());
            AddColumn("dbo.RegisterItems", "Image1", c => c.String());
            AddColumn("dbo.RegisterItems", "Image2", c => c.String());
            AddColumn("dbo.RegisterItems", "DateResolved", c => c.DateTime());
            CreateIndex("dbo.RegisterItems", "departmentId");
            CreateIndex("dbo.RegisterItems", "stationId");
            AddForeignKey("dbo.RegisterItems", "departmentId", "dbo.Departments", "value");
            AddForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AlertTags", "TagRefId", "dbo.Tags");
            DropForeignKey("dbo.AlertTags", "AlertRefId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations");
            DropForeignKey("dbo.RegisterItems", "departmentId", "dbo.Departments");
            DropIndex("dbo.AlertTags", new[] { "TagRefId" });
            DropIndex("dbo.AlertTags", new[] { "AlertRefId" });
            DropIndex("dbo.RegisterItems", new[] { "stationId" });
            DropIndex("dbo.RegisterItems", new[] { "departmentId" });
            DropColumn("dbo.RegisterItems", "DateResolved");
            DropColumn("dbo.RegisterItems", "Image2");
            DropColumn("dbo.RegisterItems", "Image1");
            DropColumn("dbo.RegisterItems", "Link");
            DropColumn("dbo.RegisterItems", "Summary");
            DropColumn("dbo.RegisterItems", "stationId");
            DropColumn("dbo.RegisterItems", "departmentId");
            DropTable("dbo.AlertTags");
            DropTable("dbo.Tags");
            DropTable("dbo.Stations");
            DropTable("dbo.Departments");
        }
    }
}
