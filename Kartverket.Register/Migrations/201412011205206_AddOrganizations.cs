namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrganizations : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RegisterItems", new[] { "Register_systemId" });
            CreateIndex("dbo.RegisterItems", "register_systemId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RegisterItems", new[] { "register_systemId" });
            CreateIndex("dbo.RegisterItems", "Register_systemId");
        }
    }
}
