namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNumberOfDatasetsByAnnex : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireMonitorings", "NumberOfDatasetsByAnnex", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireMonitorings", "NumberOfDatasetsByAnnex");
        }
    }
}
