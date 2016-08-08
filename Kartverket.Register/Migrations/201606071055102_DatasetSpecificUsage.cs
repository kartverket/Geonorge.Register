namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatasetSpecificUsage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "SpecificUsage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "SpecificUsage");
        }
    }
}
