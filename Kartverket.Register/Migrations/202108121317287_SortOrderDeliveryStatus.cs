namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SortOrderDeliveryStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DokDeliveryStatus", "sortorder", c => c.Int(nullable: false));
            AddColumn("dbo.FAIRDeliveryStatus", "sortorder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FAIRDeliveryStatus", "sortorder");
            DropColumn("dbo.DokDeliveryStatus", "sortorder");
        }
    }
}
