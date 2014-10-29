namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrganization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Number = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        LogoFilename = c.String(),
                    })
                .PrimaryKey(t => t.Number);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Organizations");
        }
    }
}
