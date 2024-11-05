namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFairRegister : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FairDatasets",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        NameEnglish = c.String(),
                        Seoname = c.String(nullable: false),
                        Description = c.String(),
                        DescriptionEnglish = c.String(),
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
                        Uuid = c.String(),
                        Notes = c.String(),
                        SpecificUsage = c.String(),
                        SpecificUsageEnglish = c.String(),
                        ProductSheetUrl = c.String(),
                        PresentationRulesUrl = c.String(),
                        ProductSpecificationUrl = c.String(),
                        MetadataUrl = c.String(),
                        DistributionFormat = c.String(),
                        DistributionUrl = c.String(),
                        DistributionArea = c.String(),
                        WmsUrl = c.String(),
                        ThemeGroupId = c.String(maxLength: 128),
                        DatasetThumbnail = c.String(),
                        DokStatusId = c.String(maxLength: 128),
                        DokStatusDateAccepted = c.DateTime(),
                        UuidService = c.String(),
                        FindableStatusId = c.Guid(nullable: false),
                        FindableStatusPerCent = c.Double(nullable: false),
                        AccesibleStatusId = c.Guid(nullable: false),
                        AccesibleStatusPerCent = c.Double(nullable: false),
                        InteroperableStatusId = c.Guid(nullable: false),
                        InteroperableStatusPerCent = c.Double(nullable: false),
                        ReUseableStatusId = c.Guid(nullable: false),
                        ReUseableStatusPerCent = c.Double(nullable: false),
                        FAIRStatusId = c.Guid(nullable: false),
                        FAIRStatusPerCent = c.Double(nullable: false),
                        F1_a_Criteria = c.Boolean(nullable: false),
                        F2_a_Criteria = c.Boolean(nullable: false),
                        F2_b_Criteria = c.Boolean(nullable: false),
                        F2_c_Criteria = c.Boolean(nullable: false),
                        F3_a_Criteria = c.Boolean(nullable: false),
                        F4_a_Criteria = c.Boolean(nullable: false),
                        A1_a_Criteria = c.Boolean(nullable: false),
                        A1_b_Criteria = c.Boolean(nullable: false),
                        A1_c_Criteria = c.Boolean(nullable: false),
                        A1_d_Criteria = c.Boolean(nullable: false),
                        A1_e_Criteria = c.Boolean(nullable: false),
                        A1_f_Criteria = c.Boolean(nullable: false),
                        A2_a_Criteria = c.Boolean(nullable: false),
                        I1_a_Criteria = c.Boolean(nullable: false),
                        I1_b_Criteria = c.Boolean(nullable: false),
                        I1_c_Criteria = c.Boolean(),
                        I2_a_Criteria = c.Boolean(nullable: false),
                        I2_b_Criteria = c.Boolean(nullable: false),
                        I3_a_Criteria = c.Boolean(),
                        I3_b_Criteria = c.Boolean(),
                        R1_a_Criteria = c.Boolean(nullable: false),
                        R2_a_Criteria = c.Boolean(nullable: false),
                        R2_b_Criteria = c.Boolean(nullable: false),
                        R2_c_Criteria = c.Boolean(nullable: false),
                        R2_d_Criteria = c.Boolean(nullable: false),
                        R2_e_Criteria = c.Boolean(nullable: false),
                        R2_f_Criteria = c.Boolean(nullable: false),
                        R3_a_Criteria = c.Boolean(nullable: false),
                        R3_b_Criteria = c.Boolean(nullable: false),
                        MetadataStatusId = c.Guid(nullable: false),
                        ProductSpesificationStatusId = c.Guid(nullable: false),
                        ProductSheetStatusId = c.Guid(nullable: false),
                        PresentationRulesStatusId = c.Guid(nullable: false),
                        SosiDataStatusId = c.Guid(nullable: false),
                        GmlDataStatusId = c.Guid(nullable: false),
                        WmsStatusId = c.Guid(nullable: false),
                        WfsStatusId = c.Guid(nullable: false),
                        AtomFeedStatusId = c.Guid(nullable: false),
                        CommonStatusId = c.Guid(nullable: false),
                        Grade = c.Single(),
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.FAIRDeliveries", t => t.FindableStatusId, cascadeDelete: false)
                .ForeignKey("dbo.FAIRDeliveries", t => t.AccesibleStatusId, cascadeDelete: false)
                .ForeignKey("dbo.FAIRDeliveries", t => t.InteroperableStatusId, cascadeDelete: false)
                .ForeignKey("dbo.FAIRDeliveries", t => t.ReUseableStatusId, cascadeDelete: false)
                .ForeignKey("dbo.FAIRDeliveries", t => t.FAIRStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.MetadataStatusId, cascadeDelete: true)
                .ForeignKey("dbo.DatasetDeliveries", t => t.ProductSpesificationStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.ProductSheetStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.PresentationRulesStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.SosiDataStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.GmlDataStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.WmsStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.WfsStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.AtomFeedStatusId, cascadeDelete: false)
                .ForeignKey("dbo.DatasetDeliveries", t => t.CommonStatusId, cascadeDelete: false)
                .Index(t => t.SubmitterId)
                .Index(t => t.OwnerId)
                .Index(t => t.StatusId)
                .Index(t => t.RegisterId)
                .Index(t => t.VersioningId)
                .Index(t => t.ThemeGroupId)
                .Index(t => t.DokStatusId)
                .Index(t => t.FindableStatusId)
                .Index(t => t.AccesibleStatusId)
                .Index(t => t.InteroperableStatusId)
                .Index(t => t.ReUseableStatusId)
                .Index(t => t.FAIRStatusId)
                .Index(t => t.MetadataStatusId)
                .Index(t => t.ProductSpesificationStatusId)
                .Index(t => t.ProductSheetStatusId)
                .Index(t => t.PresentationRulesStatusId)
                .Index(t => t.SosiDataStatusId)
                .Index(t => t.GmlDataStatusId)
                .Index(t => t.WmsStatusId)
                .Index(t => t.WfsStatusId)
                .Index(t => t.AtomFeedStatusId)
                .Index(t => t.CommonStatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FairDatasets", "CommonStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "AtomFeedStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "WfsStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "WmsStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "GmlDataStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "SosiDataStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "PresentationRulesStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "ProductSheetStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "ProductSpesificationStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "MetadataStatusId", "dbo.DatasetDeliveries");
            DropForeignKey("dbo.FairDatasets", "FAIRStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.FairDatasets", "ReUseableStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.FairDatasets", "InteroperableStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.FairDatasets", "AccesibleStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.FairDatasets", "FindableStatusId", "dbo.FAIRDeliveries");
            DropIndex("dbo.FairDatasets", new[] { "CommonStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "AtomFeedStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "WfsStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "WmsStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "GmlDataStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "SosiDataStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "PresentationRulesStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "ProductSheetStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "ProductSpesificationStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "MetadataStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "FAIRStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "ReUseableStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "InteroperableStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "AccesibleStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "FindableStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "DokStatusId" });
            DropIndex("dbo.FairDatasets", new[] { "ThemeGroupId" });
            DropIndex("dbo.FairDatasets", new[] { "VersioningId" });
            DropIndex("dbo.FairDatasets", new[] { "RegisterId" });
            DropIndex("dbo.FairDatasets", new[] { "StatusId" });
            DropIndex("dbo.FairDatasets", new[] { "OwnerId" });
            DropIndex("dbo.FairDatasets", new[] { "SubmitterId" });
            DropTable("dbo.FairDatasets");
        }
    }
}
