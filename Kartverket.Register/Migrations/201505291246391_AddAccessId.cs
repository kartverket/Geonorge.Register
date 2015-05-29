namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccessId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registers", "accessId", c => c.Int());
            CreateIndex("dbo.Registers", "accessId");
            AddForeignKey("dbo.Registers", "accessId", "dbo.accessTypes", "accessLevel");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registers", "accessId", "dbo.accessTypes");
            DropIndex("dbo.Registers", new[] { "accessId" });
            DropColumn("dbo.Registers", "accessId");
        }
    }
}
