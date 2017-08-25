namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEpsgTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EPSGTranslations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegisterItemId = c.Guid(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        inspireRequirementDescription = c.String(),
                        CultureName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RegisterItems", t => t.RegisterItemId, cascadeDelete: true)
                .Index(t => t.RegisterItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EPSGTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropIndex("dbo.EPSGTranslations", new[] { "RegisterItemId" });
            DropTable("dbo.EPSGTranslations");
        }
    }
}
