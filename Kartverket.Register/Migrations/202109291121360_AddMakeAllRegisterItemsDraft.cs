namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMakeAllRegisterItemsDraft : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registers", "MakeAllItemsDraft", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Registers", "MakeAllItemsDraft");
        }
    }
}
