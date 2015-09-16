namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteOwnerFromRegisterItems : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RegisterItems", "ownerId", "dbo.RegisterItems");
            DropIndex("dbo.RegisterItems", new[] { "ownerId" });
            DropColumn("dbo.RegisterItems", "ownerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItems", "ownerId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "ownerId");
            AddForeignKey("dbo.RegisterItems", "ownerId", "dbo.RegisterItems", "systemId");
        }
    }
}
