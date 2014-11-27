namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsubregister : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registers", "Register_systemId", c => c.Guid());
            CreateIndex("dbo.Registers", "Register_systemId");
            AddForeignKey("dbo.Registers", "Register_systemId", "dbo.Registers", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registers", "Register_systemId", "dbo.Registers");
            DropIndex("dbo.Registers", new[] { "Register_systemId" });
            DropColumn("dbo.Registers", "Register_systemId");
        }
    }
}
