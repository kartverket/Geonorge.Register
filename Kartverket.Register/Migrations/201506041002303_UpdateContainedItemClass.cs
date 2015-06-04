namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateContainedItemClass : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Registers", "containedItemClass", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Registers", "containedItemClass", c => c.String());
        }
    }
}
