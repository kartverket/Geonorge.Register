namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccessType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.accessTypes",
                c => new
                    {
                        accessLevel = c.Int(nullable: false, identity: true),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.accessLevel);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.accessTypes");
        }
    }
}
