namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFairDatasetType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FairDatasetTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                        Description = c.String(),
                        FairDataset_SystemId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FairDatasets", t => t.FairDataset_SystemId)
                .Index(t => t.FairDataset_SystemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FairDatasetTypes", "FairDataset_SystemId", "dbo.FairDatasets");
            DropIndex("dbo.FairDatasetTypes", new[] { "FairDataset_SystemId" });
            DropTable("dbo.FairDatasetTypes");
        }
    }
}
