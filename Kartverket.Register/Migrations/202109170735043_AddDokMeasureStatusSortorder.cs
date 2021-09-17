namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDokMeasureStatusSortorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DokMeasureStatus", "sortorder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DokMeasureStatus", "sortorder");
        }
    }
}
