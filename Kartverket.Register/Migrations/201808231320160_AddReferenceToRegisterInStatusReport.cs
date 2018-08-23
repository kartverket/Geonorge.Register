namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReferenceToRegisterInStatusReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatusReports", "Register_systemId", c => c.Guid());
            CreateIndex("dbo.StatusReports", "Register_systemId");
            AddForeignKey("dbo.StatusReports", "Register_systemId", "dbo.Registers", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StatusReports", "Register_systemId", "dbo.Registers");
            DropIndex("dbo.StatusReports", new[] { "Register_systemId" });
            DropColumn("dbo.StatusReports", "Register_systemId");
        }
    }
}
