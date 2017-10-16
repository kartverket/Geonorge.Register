namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentTranslations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegisterItemId = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        CultureName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RegisterItems", t => t.RegisterItemId, cascadeDelete: true)
                .Index(t => t.RegisterItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropIndex("dbo.DocumentTranslations", new[] { "RegisterItemId" });
            DropTable("dbo.DocumentTranslations");
        }
    }
}
