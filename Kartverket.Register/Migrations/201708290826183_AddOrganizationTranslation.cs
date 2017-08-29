namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrganizationTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganizationTranslations",
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
            DropForeignKey("dbo.OrganizationTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropIndex("dbo.OrganizationTranslations", new[] { "RegisterItemId" });
            DropTable("dbo.OrganizationTranslations");
        }
    }
}
