namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentUrlAttachments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Links",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Text = c.String(),
                        RegisterItem_systemId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RegisterItems", t => t.RegisterItem_systemId)
                .Index(t => t.RegisterItem_systemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Links", "RegisterItem_systemId", "dbo.RegisterItems");
            DropIndex("dbo.Links", new[] { "RegisterItem_systemId" });
            DropTable("dbo.Links");
        }
    }
}
