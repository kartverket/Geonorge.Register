namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRegisterTranslation : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterTranslations", "RegisterId", "dbo.Registers");
            DropIndex("dbo.RegisterTranslations", new[] { "RegisterId" });
            DropTable("dbo.RegisterTranslations");
        }
    }
}
