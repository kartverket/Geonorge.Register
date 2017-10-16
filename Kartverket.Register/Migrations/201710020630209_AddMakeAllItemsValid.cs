namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMakeAllItemsValid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registers", "MakeAllItemsValid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Registers", "MakeAllItemsValid");
        }
    }
}
