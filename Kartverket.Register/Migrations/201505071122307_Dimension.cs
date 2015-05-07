namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dimension : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dimensions",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.RegisterItems", "dimensionId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RegisterItems", "dimensionId");
            AddForeignKey("dbo.RegisterItems", "dimensionId", "dbo.Dimensions", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "dimensionId", "dbo.Dimensions");
            DropIndex("dbo.RegisterItems", new[] { "dimensionId" });
            DropColumn("dbo.RegisterItems", "dimensionId");
            DropTable("dbo.Dimensions");
        }
    }
}
