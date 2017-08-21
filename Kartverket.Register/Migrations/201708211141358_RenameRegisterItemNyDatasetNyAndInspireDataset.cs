namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameRegisterItemNyDatasetNyAndInspireDataset : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.InspireDatasets", new[] { "statusId" });
            CreateIndex("dbo.InspireDatasets", "StatusId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.InspireDatasets", new[] { "StatusId" });
            CreateIndex("dbo.InspireDatasets", "statusId");
        }
    }
}
