namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNamespace : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NameSpaces",
                c => new
                    {
                        nameSpace = c.String(nullable: false, maxLength: 128),
                        owner = c.String(),
                        content = c.String(),
                        serviceUrl = c.String(),
                    })
                .PrimaryKey(t => t.nameSpace);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NameSpaces");
        }
    }
}
