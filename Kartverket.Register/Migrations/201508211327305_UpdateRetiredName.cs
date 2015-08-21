namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRetiredName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "DateRetired", c => c.DateTime());
            DropColumn("dbo.RegisterItems", "retired");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItems", "retired", c => c.DateTime());
            DropColumn("dbo.RegisterItems", "DateRetired");
        }
    }
}
