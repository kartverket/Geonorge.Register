namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addReportingAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "epost", c => c.String());
            AddColumn("dbo.RegisterItems", "member", c => c.Boolean());
            AddColumn("dbo.RegisterItems", "agreementYear", c => c.Int());
            AddColumn("dbo.RegisterItems", "agreementDocumentUrl", c => c.String());
            AddColumn("dbo.RegisterItems", "priceFormDocument", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "priceFormDocument");
            DropColumn("dbo.RegisterItems", "agreementDocumentUrl");
            DropColumn("dbo.RegisterItems", "agreementYear");
            DropColumn("dbo.RegisterItems", "member");
            DropColumn("dbo.RegisterItems", "epost");
        }
    }
}
