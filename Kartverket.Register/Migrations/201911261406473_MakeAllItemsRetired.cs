namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeAllItemsRetired : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registers", "MakeAllItemsRetired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Registers", "MakeAllItemsRetired");
        }
    }
}
