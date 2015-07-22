namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addApprovalFieldsToDocuments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "approvalDocument", c => c.String());
            AddColumn("dbo.RegisterItems", "approvalReference", c => c.String());
            AddColumn("dbo.RegisterItems", "Accepted", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "Accepted");
            DropColumn("dbo.RegisterItems", "approvalReference");
            DropColumn("dbo.RegisterItems", "approvalDocument");
        }
    }
}
