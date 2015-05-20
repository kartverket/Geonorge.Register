namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateVersionTabel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Versions", "currentVersion", c => c.Guid(nullable: false));
            AddColumn("dbo.Versions", "lastVersionNumber", c => c.Int(nullable: false));
            DropColumn("dbo.Versions", "versionInfo");
        }

        public override void Down()
        {
            AddColumn("dbo.Versions", "versionInfo", c => c.String());
            DropColumn("dbo.Versions", "lastVersionNumber");
            DropColumn("dbo.Versions", "currentVersion");
        }
    }
}
