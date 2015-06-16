namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBroaderAndNarrowerCodelistItems : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "broaderItemId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "broaderItemId");
            AddForeignKey("dbo.RegisterItems", "broaderItemId", "dbo.RegisterItems", "systemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "broaderItemId", "dbo.RegisterItems");
            DropIndex("dbo.RegisterItems", new[] { "broaderItemId" });
            DropColumn("dbo.RegisterItems", "broaderItemId");
        }
    }
}
