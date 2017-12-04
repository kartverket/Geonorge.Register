namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddValidFromAndValidTo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "ValidFromDate", c => c.DateTime());
            AddColumn("dbo.RegisterItems", "ValidToDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "ValidToDate");
            DropColumn("dbo.RegisterItems", "ValidFromDate");
        }
    }
}
