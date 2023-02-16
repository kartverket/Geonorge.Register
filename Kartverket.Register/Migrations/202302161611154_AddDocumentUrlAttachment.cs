namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentUrlAttachment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "documentUrlAttachment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "documentUrlAttachment");
        }
    }
}
