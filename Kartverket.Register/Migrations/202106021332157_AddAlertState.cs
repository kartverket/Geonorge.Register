namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertState : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.States",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.RegisterItems", "stateId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RegisterItems", "stateId");
            AddForeignKey("dbo.RegisterItems", "stateId", "dbo.States", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "stateId", "dbo.States");
            DropIndex("dbo.RegisterItems", new[] { "stateId" });
            DropColumn("dbo.RegisterItems", "stateId");
            DropTable("dbo.States");
        }
    }
}
