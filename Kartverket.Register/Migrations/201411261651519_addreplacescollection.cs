namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addreplacescollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Versions", "Register_systemId", c => c.Guid());
            CreateIndex("dbo.Versions", "Register_systemId");
            AddForeignKey("dbo.Versions", "Register_systemId", "dbo.Registers", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Versions", "Register_systemId", "dbo.Registers");
            DropIndex("dbo.Versions", new[] { "Register_systemId" });
            DropColumn("dbo.Versions", "Register_systemId");
        }
    }
}
