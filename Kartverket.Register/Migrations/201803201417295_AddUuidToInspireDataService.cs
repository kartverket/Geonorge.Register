namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUuidToInspireDataService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDataServices", "Uuid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDataServices", "Uuid");
        }
    }
}
