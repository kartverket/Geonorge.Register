namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatasetTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DatasetTranslations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegisterItemId = c.Guid(nullable: false),
                        ThemeGroupId = c.String(),
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
            DropForeignKey("dbo.DatasetTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropIndex("dbo.DatasetTranslations", new[] { "RegisterItemId" });
            DropTable("dbo.DatasetTranslations");
        }
    }
}
