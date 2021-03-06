namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSdsFromInspireDataServiceTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InspireDataServices", "Sds");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "Sds", c => c.Boolean(nullable: false));
        }
    }
}
