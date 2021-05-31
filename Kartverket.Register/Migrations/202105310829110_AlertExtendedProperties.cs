namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlertExtendedProperties : DbMigration
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
            
            AddColumn("dbo.RegisterItems", "Tag", c => c.String());
            AddColumn("dbo.RegisterItems", "Category", c => c.String());
            AddColumn("dbo.RegisterItems", "State", c => c.String());
            AddColumn("dbo.RegisterItems", "stationId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RegisterItems", "stationId");
            AddForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations", "Name");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations");
            DropIndex("dbo.RegisterItems", new[] { "stationId" });
            DropColumn("dbo.RegisterItems", "stationId");
            DropColumn("dbo.RegisterItems", "State");
            DropColumn("dbo.RegisterItems", "Category");
            DropColumn("dbo.RegisterItems", "Tag");
            DropTable("dbo.Stations");
        }
    }
}
