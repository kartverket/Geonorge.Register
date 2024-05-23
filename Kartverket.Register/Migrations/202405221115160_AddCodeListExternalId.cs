namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodeListExternalId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "externalId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "externalId");
        }
    }
}
