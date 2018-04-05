namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameServiceTypeToInspireDataType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDataServices", "InspireDataType", c => c.String());
            DropColumn("dbo.InspireDataServices", "ServiceType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "ServiceType", c => c.String());
            DropColumn("dbo.InspireDataServices", "InspireDataType");
        }
    }
}
