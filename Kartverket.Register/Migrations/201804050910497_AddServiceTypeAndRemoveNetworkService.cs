namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddServiceTypeAndRemoveNetworkService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDataServices", "ServiceType", c => c.String());
            DropColumn("dbo.InspireDataServices", "NetworkService");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "NetworkService", c => c.Boolean(nullable: false));
            DropColumn("dbo.InspireDataServices", "ServiceType");
        }
    }
}
