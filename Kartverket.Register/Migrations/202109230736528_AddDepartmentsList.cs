namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepartmentsList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AlertDepartments",
                c => new
                    {
                        AlertRefId = c.Guid(nullable: false),
                        DepartmentRefId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.AlertRefId, t.DepartmentRefId })
                .ForeignKey("dbo.RegisterItems", t => t.AlertRefId, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentRefId, cascadeDelete: true)
                .Index(t => t.AlertRefId)
                .Index(t => t.DepartmentRefId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AlertDepartments", "DepartmentRefId", "dbo.Departments");
            DropForeignKey("dbo.AlertDepartments", "AlertRefId", "dbo.RegisterItems");
            DropIndex("dbo.AlertDepartments", new[] { "DepartmentRefId" });
            DropIndex("dbo.AlertDepartments", new[] { "AlertRefId" });
            DropTable("dbo.AlertDepartments");
        }
    }
}
