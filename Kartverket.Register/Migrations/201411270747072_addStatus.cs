namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.Registers", "status_value", c => c.String(maxLength: 128));
            CreateIndex("dbo.Registers", "status_value");
            AddForeignKey("dbo.Registers", "status_value", "dbo.Status", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registers", "status_value", "dbo.Status");
            DropIndex("dbo.Registers", new[] { "status_value" });
            DropColumn("dbo.Registers", "status_value");
            DropTable("dbo.Status");
        }
    }
}
