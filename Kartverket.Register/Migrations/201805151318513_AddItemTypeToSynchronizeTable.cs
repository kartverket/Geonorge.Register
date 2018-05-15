namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemTypeToSynchronizeTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Synchronizes", "ItemType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Synchronizes", "ItemType");
        }
    }
}
