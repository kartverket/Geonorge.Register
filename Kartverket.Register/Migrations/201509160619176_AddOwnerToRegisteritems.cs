namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOwnerToRegisteritems : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RegisterItems", new[] { "ownerId" });
            AlterColumn("dbo.RegisterItems", "ownerId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "ownerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RegisterItems", new[] { "ownerId" });
            AlterColumn("dbo.RegisterItems", "ownerId", c => c.Guid(nullable: false));
            CreateIndex("dbo.RegisterItems", "ownerId");
        }
    }
}
