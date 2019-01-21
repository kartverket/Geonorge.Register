namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAlertTranslationRenameAlert : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ServiceAlertTranslations", newName: "AlertTranslations");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.AlertTranslations", newName: "ServiceAlertTranslations");
        }
    }
}
