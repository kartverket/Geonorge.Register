namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteColumnsInNameSpace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "serviceUrl", c => c.String());
            DropTable("dbo.NameSpaces");
        }
        
        public override void Down()
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
            
            DropColumn("dbo.RegisterItems", "serviceUrl");
        }
    }
}
