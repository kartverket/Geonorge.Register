namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnullvalues : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RegisterItems", new[] { "submitterId" });
            AlterColumn("dbo.RegisterItems", "submitterId", c => c.Guid());
            CreateIndex("dbo.RegisterItems", "submitterId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RegisterItems", new[] { "submitterId" });
            AlterColumn("dbo.RegisterItems", "submitterId", c => c.Guid(nullable: false));
            CreateIndex("dbo.RegisterItems", "submitterId");
        }
    }
}
