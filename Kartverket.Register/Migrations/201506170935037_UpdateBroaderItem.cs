namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateBroaderItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RegisterItems", "broaderItemId", "dbo.RegisterItems");
            AddColumn("dbo.RegisterItems", "CodelistValue_systemId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "CodelistValue_systemId");
            AddForeignKey("dbo.RegisterItems", "CodelistValue_systemId", "dbo.RegisterItems", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "CodelistValue_systemId", "dbo.RegisterItems");
            DropIndex("dbo.RegisterItems", new[] { "CodelistValue_systemId" });
            DropColumn("dbo.RegisterItems", "CodelistValue_systemId");
            AddForeignKey("dbo.RegisterItems", "broaderItemId", "dbo.RegisterItems", "systemId");
        }
    }
}
