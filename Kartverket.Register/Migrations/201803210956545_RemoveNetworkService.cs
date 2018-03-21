namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveNetworkService : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InspireDataServices", "NetworkService");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "NetworkService", c => c.Boolean(nullable: false));
        }
    }
}
