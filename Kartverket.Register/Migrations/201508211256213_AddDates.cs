namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "dateNotAccepted", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "dateSuperseded", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "retired", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "retired");
            DropColumn("dbo.RegisterItems", "dateSuperseded");
            DropColumn("dbo.RegisterItems", "dateNotAccepted");
        }
    }
}
