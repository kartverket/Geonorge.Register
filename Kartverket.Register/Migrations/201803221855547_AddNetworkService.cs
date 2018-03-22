namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNetworkService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDataServices", "NetworkService", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDataServices", "NetworkService");
        }
    }
}
