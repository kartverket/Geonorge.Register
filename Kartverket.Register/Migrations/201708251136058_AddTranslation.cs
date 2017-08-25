namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegisterTranslations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegisterId = c.Guid(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        CultureName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Registers", t => t.RegisterId, cascadeDelete: true)
                .Index(t => t.RegisterId);
            
            CreateTable(
                "dbo.EPSGTranslations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegisterItemId = c.Guid(nullable: false),
                        inspireRequirementDescription = c.String(),
                        name = c.String(),
                        description = c.String(),
                        CultureName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RegisterItems", t => t.RegisterItemId, cascadeDelete: true)
                .Index(t => t.RegisterItemId);
            
            CreateTable(
                "dbo.CodelistValueTranslations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegisterItemId = c.Guid(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        CultureName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RegisterItems", t => t.RegisterItemId, cascadeDelete: true)
                .Index(t => t.RegisterItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CodelistValueTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropForeignKey("dbo.EPSGTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropForeignKey("dbo.RegisterTranslations", "RegisterId", "dbo.Registers");
            DropIndex("dbo.CodelistValueTranslations", new[] { "RegisterItemId" });
            DropIndex("dbo.EPSGTranslations", new[] { "RegisterItemId" });
            DropIndex("dbo.RegisterTranslations", new[] { "RegisterId" });
            DropTable("dbo.CodelistValueTranslations");
            DropTable("dbo.EPSGTranslations");
            DropTable("dbo.RegisterTranslations");
        }
    }
}
