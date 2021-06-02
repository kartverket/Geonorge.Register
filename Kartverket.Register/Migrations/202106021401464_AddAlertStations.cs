namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertStations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                        group = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.RegisterItems", "stationId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RegisterItems", "stationId");
            AddForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "stationId", "dbo.Stations");
            DropIndex("dbo.RegisterItems", new[] { "stationId" });
            DropColumn("dbo.RegisterItems", "stationId");
            DropTable("dbo.Stations");
        }
    }
}
