namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSchematronFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "documentUrlSchematron", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "documentUrlSchematron");
        }
    }
}
