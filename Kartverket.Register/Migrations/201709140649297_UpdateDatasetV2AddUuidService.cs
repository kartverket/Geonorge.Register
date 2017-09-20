namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatasetV2AddUuidService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDatasets", "UuidService", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDatasets", "UuidService");
        }
    }
}
