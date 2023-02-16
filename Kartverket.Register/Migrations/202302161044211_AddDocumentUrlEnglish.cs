namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentUrlEnglish : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "documentUrlEnglish", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "documentUrlEnglish");
        }
    }
}
