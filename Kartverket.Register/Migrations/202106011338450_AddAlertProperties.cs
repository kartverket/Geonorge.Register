namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertProperties : DbMigration
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
                "dbo.States",
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
                        Alert_systemId = c.Guid(),
                    })
                .PrimaryKey(t => t.value)
                .ForeignKey("dbo.RegisterItems", t => t.Alert_systemId)
                .Index(t => t.Alert_systemId);
            
            AddColumn("dbo.RegisterItems", "departmentId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "stateId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "stationId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "Summary", c => c.String());
            AddColumn("dbo.RegisterItems", "Link", c => c.String());
            AddColumn("dbo.RegisterItems", "Image1", c => c.String());
            AddColumn("dbo.RegisterItems", "Image2", c => c.String());
            AddColumn("dbo.RegisterItems", "DateResolved", c => c.DateTime());
            CreateIndex("dbo.RegisterItems", "departmentId");
            CreateIndex("dbo.RegisterItems", "stateId");
            CreateIndex("dbo.RegisterItems", "stationId");
            AddForeignKey("dbo.RegisterItems", "departmentId", "dbo.Departments", "value");
            AddForeignKey("dbo.RegisterItems", "stateId", "dbo.States", "value");
            AddForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "Alert_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations");
            DropForeignKey("dbo.RegisterItems", "stateId", "dbo.States");
            DropForeignKey("dbo.RegisterItems", "departmentId", "dbo.Departments");
            DropIndex("dbo.Tags", new[] { "Alert_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "stationId" });
            DropIndex("dbo.RegisterItems", new[] { "stateId" });
            DropIndex("dbo.RegisterItems", new[] { "departmentId" });
            DropColumn("dbo.RegisterItems", "DateResolved");
            DropColumn("dbo.RegisterItems", "Image2");
            DropColumn("dbo.RegisterItems", "Image1");
            DropColumn("dbo.RegisterItems", "Link");
            DropColumn("dbo.RegisterItems", "Summary");
            DropColumn("dbo.RegisterItems", "stationId");
            DropColumn("dbo.RegisterItems", "stateId");
            DropColumn("dbo.RegisterItems", "departmentId");
            DropTable("dbo.Tags");
            DropTable("dbo.Stations");
            DropTable("dbo.States");
            DropTable("dbo.Departments");
        }
    }
}
