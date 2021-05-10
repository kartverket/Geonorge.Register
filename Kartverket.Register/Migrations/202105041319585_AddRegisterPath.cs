namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRegisterPath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registers", "pathOld", c => c.String(maxLength: 450));
            AddColumn("dbo.Registers", "path", c => c.String(maxLength: 450));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Registers", "path");
            DropColumn("dbo.Registers", "pathOld");
        }
    }
}
