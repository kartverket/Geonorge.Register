namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatasetUuid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatusHistories", "DatasetUuid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StatusHistories", "DatasetUuid");
        }
    }
}
