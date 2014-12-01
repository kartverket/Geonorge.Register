namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class slettURL : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Registers", "url");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Registers", "url", c => c.String());
        }
    }
}
