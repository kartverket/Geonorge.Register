namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetServiceAlertOwnerAsString : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RegisterItems", "OwnerId", "dbo.RegisterItems");
            DropIndex("dbo.RegisterItems", new[] { "OwnerId" });
            AddColumn("dbo.RegisterItems", "Owner", c => c.String());
            DropColumn("dbo.RegisterItems", "OwnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItems", "OwnerId", c => c.Guid());
            DropColumn("dbo.RegisterItems", "Owner");
            CreateIndex("dbo.RegisterItems", "OwnerId");
            AddForeignKey("dbo.RegisterItems", "OwnerId", "dbo.RegisterItems", "systemId");
        }
    }
}
