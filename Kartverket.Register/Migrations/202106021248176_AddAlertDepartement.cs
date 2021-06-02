namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertDepartement : DbMigration
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
            
            AddColumn("dbo.RegisterItems", "departmentId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RegisterItems", "departmentId");
            AddForeignKey("dbo.RegisterItems", "departmentId", "dbo.Departments", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "departmentId", "dbo.Departments");
            DropIndex("dbo.RegisterItems", new[] { "departmentId" });
            DropColumn("dbo.RegisterItems", "departmentId");
            DropTable("dbo.Departments");
        }
    }
}
