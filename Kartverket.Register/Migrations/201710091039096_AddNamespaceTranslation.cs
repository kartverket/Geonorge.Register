namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNamespaceTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NamespaceTranslations",
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
            DropForeignKey("dbo.NamespaceTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropIndex("dbo.NamespaceTranslations", new[] { "RegisterItemId" });
            DropTable("dbo.NamespaceTranslations");
        }
    }
}
