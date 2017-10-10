namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddServiceAlertTranslation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceAlertTranslations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegisterItemId = c.Guid(nullable: false),
                        AlertType = c.String(),
                        Note = c.String(),
                        Owner = c.String(),
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
            DropForeignKey("dbo.ServiceAlertTranslations", "RegisterItemId", "dbo.RegisterItems");
            DropIndex("dbo.ServiceAlertTranslations", new[] { "RegisterItemId" });
            DropTable("dbo.ServiceAlertTranslations");
        }
    }
}
