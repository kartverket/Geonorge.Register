namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApprovalDatesOnRegisterItemsV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDatasets", "DateAccepted", c => c.DateTime());
            AddColumn("dbo.InspireDatasets", "DateNotAccepted", c => c.DateTime());
            AddColumn("dbo.InspireDatasets", "DateSuperseded", c => c.DateTime());
            AddColumn("dbo.InspireDatasets", "DateRetired", c => c.DateTime());
            AddColumn("dbo.InspireDatasets", "VersionNumber", c => c.Int(nullable: false));
            AddColumn("dbo.InspireDatasets", "VersionName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDatasets", "VersionName");
            DropColumn("dbo.InspireDatasets", "VersionNumber");
            DropColumn("dbo.InspireDatasets", "DateRetired");
            DropColumn("dbo.InspireDatasets", "DateSuperseded");
            DropColumn("dbo.InspireDatasets", "DateNotAccepted");
            DropColumn("dbo.InspireDatasets", "DateAccepted");
        }
    }
}
