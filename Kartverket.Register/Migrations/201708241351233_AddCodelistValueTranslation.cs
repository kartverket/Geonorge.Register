namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodelistValueTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CodelistValueTranslations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegisterItemId = c.Guid(nullable: false),
                        name = c.String(),
                        CultureName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RegisterItems", t => t.RegisterItemId, cascadeDelete: true)
                .Index(t => t.RegisterItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CodelistValueTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropIndex("dbo.CodelistValueTranslations", new[] { "RegisterItemId" });
            DropTable("dbo.CodelistValueTranslations");
        }
    }
}
