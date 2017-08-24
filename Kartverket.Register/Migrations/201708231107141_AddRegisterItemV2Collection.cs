namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRegisterItemV2Collection : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.InspireDatasets", name: "OwnerId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.InspireDatasets", name: "SubmitterId", newName: "OwnerId");
            RenameColumn(table: "dbo.InspireDatasets", name: "__mig_tmp__0", newName: "SubmitterId");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_OwnerId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_SubmitterId", newName: "IX_OwnerId");
            RenameIndex(table: "dbo.InspireDatasets", name: "__mig_tmp__0", newName: "IX_SubmitterId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_SubmitterId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.InspireDatasets", name: "IX_OwnerId", newName: "IX_SubmitterId");
            RenameIndex(table: "dbo.InspireDatasets", name: "__mig_tmp__0", newName: "IX_OwnerId");
            RenameColumn(table: "dbo.InspireDatasets", name: "SubmitterId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.InspireDatasets", name: "OwnerId", newName: "SubmitterId");
            RenameColumn(table: "dbo.InspireDatasets", name: "__mig_tmp__0", newName: "OwnerId");
        }
    }
}
