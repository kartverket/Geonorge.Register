namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertProperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
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
            
            AddColumn("dbo.RegisterItems", "Department", c => c.String());
            AddColumn("dbo.RegisterItems", "State", c => c.String());
            AddColumn("dbo.RegisterItems", "stationId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "ValidTo", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "Summary", c => c.String());
            AddColumn("dbo.RegisterItems", "Link", c => c.String());
            AddColumn("dbo.RegisterItems", "Image1", c => c.String());
            AddColumn("dbo.RegisterItems", "Image2", c => c.String());
            AddColumn("dbo.RegisterItems", "DateResolved", c => c.DateTime());
            CreateIndex("dbo.RegisterItems", "stationId");
            AddForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations", "Name");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "Alert_systemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations");
            DropIndex("dbo.Tags", new[] { "Alert_systemId" });
            DropIndex("dbo.RegisterItems", new[] { "stationId" });
            DropColumn("dbo.RegisterItems", "DateResolved");
            DropColumn("dbo.RegisterItems", "Image2");
            DropColumn("dbo.RegisterItems", "Image1");
            DropColumn("dbo.RegisterItems", "Link");
            DropColumn("dbo.RegisterItems", "Summary");
            DropColumn("dbo.RegisterItems", "ValidTo");
            DropColumn("dbo.RegisterItems", "stationId");
            DropColumn("dbo.RegisterItems", "State");
            DropColumn("dbo.RegisterItems", "Department");
            DropTable("dbo.Tags");
            DropTable("dbo.Stations");
        }
    }
}
