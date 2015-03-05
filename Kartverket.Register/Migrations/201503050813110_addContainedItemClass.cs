namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addContainedItemClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContainedItemClasses",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ContainedItemClasses");
        }
    }
}
