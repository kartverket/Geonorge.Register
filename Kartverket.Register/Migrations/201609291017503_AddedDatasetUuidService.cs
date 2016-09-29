namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDatasetUuidService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "UuidService", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "UuidService");
        }
    }
}
