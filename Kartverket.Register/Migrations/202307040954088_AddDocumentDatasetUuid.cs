namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentDatasetUuid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "DatasetUuid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "DatasetUuid");
        }
    }
}
