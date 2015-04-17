namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersionContainedItemClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Versions", "containedItemClass", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Versions", "containedItemClass");
        }
    }
}
