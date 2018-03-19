namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspireDataService : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InspireDataServices",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Seoname = c.String(nullable: false),
                        Description = c.String(),
                        SubmitterId = c.Guid(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        DateSubmitted = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        StatusId = c.String(nullable: false, maxLength: 128),
                        RegisterId = c.Guid(nullable: false),
                        DateAccepted = c.DateTime(),
                        DateNotAccepted = c.DateTime(),
                        DateSuperseded = c.DateTime(),
                        DateRetired = c.DateTime(),
                        VersionNumber = c.Int(nullable: false),
                        VersionName = c.String(),
                        VersioningId = c.Guid(nullable: false),
                        ServiceType = c.String(),
                        InspireDeliveryMetadataId = c.Guid(nullable: false),
                        InspireDeliveryMetadataInSearchServiceId = c.Guid(nullable: false),
                        InspireDeliveryServiceStatusId = c.Guid(nullable: false),
                        Requests = c.Int(nullable: false),
                        NetworkService = c.Boolean(nullable: false),
                        Sds = c.Boolean(nullable: false),
                        Url = c.String(),
                        Theme = c.String(),
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryMetadataId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryMetadataInSearchServiceId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.InspireDeliveryServiceStatusId, cascadeDelete: false)
                .Index(t => t.SubmitterId)
                .Index(t => t.OwnerId)
                .Index(t => t.StatusId)
                .Index(t => t.RegisterId)
                .Index(t => t.VersioningId)
                .Index(t => t.InspireDeliveryMetadataId)
                .Index(t => t.InspireDeliveryMetadataInSearchServiceId)
                .Index(t => t.InspireDeliveryServiceStatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspireDataServices", "InspireDeliveryServiceStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDataServices", "InspireDeliveryMetadataInSearchServiceId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.InspireDataServices", "InspireDeliveryMetadataId", "dbo.DatasetDeliveries");
            DropIndex("dbo.InspireDataServices", new[] { "InspireDeliveryServiceStatusId" });
            DropIndex("dbo.InspireDataServices", new[] { "InspireDeliveryMetadataInSearchServiceId" });
            DropIndex("dbo.InspireDataServices", new[] { "InspireDeliveryMetadataId" });
            DropIndex("dbo.InspireDataServices", new[] { "VersioningId" });
            DropIndex("dbo.InspireDataServices", new[] { "RegisterId" });
            DropIndex("dbo.InspireDataServices", new[] { "StatusId" });
            DropIndex("dbo.InspireDataServices", new[] { "OwnerId" });
            DropIndex("dbo.InspireDataServices", new[] { "SubmitterId" });
            DropTable("dbo.InspireDataServices");
        }
    }
}
