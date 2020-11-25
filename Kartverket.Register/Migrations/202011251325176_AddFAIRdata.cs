namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFAIRdata : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FAIRDeliveries",
                c => new
                    {
                        FAIRDeliveryId = c.Guid(nullable: false),
                        StatusId = c.String(maxLength: 128),
                        Note = c.String(),
                        AutoUpdate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FAIRDeliveryId)
                .ForeignKey("dbo.FAIRDeliveryStatus", t => t.StatusId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.FAIRDeliveryStatus",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.MareanoDatasets", "FindableStatusId", c => c.Guid(nullable: true));
            AddColumn("dbo.MareanoDatasets", "FindableStatusPerCent", c => c.Double(nullable: false));
            AddColumn("dbo.MareanoDatasets", "AccesibleStatusId", c => c.Guid(nullable: true));
            AddColumn("dbo.MareanoDatasets", "AccesibleStatusPerCent", c => c.Double(nullable: false));
            AddColumn("dbo.MareanoDatasets", "InteroperableStatusId", c => c.Guid(nullable: true));
            AddColumn("dbo.MareanoDatasets", "InteroperableStatusPerCent", c => c.Double(nullable: false));
            AddColumn("dbo.MareanoDatasets", "ReUseableStatusId", c => c.Guid(nullable: true));
            AddColumn("dbo.MareanoDatasets", "ReUseableStatusPerCent", c => c.Double(nullable: false));
            AddColumn("dbo.MareanoDatasets", "FAIRStatusId", c => c.Guid(nullable: true));
            AddColumn("dbo.MareanoDatasets", "FAIRStatusPerCent", c => c.Double(nullable: false));
            AddColumn("dbo.MareanoDatasets", "F1_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "F2_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "F2_b_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "F2_c_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "F3_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "F4_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "A1_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "A1_b_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "A1_c_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "A1_d_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "A1_e_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "A1_f_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "A2_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "I1_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "I1_b_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "I1_c_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "I2_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "I2_b_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "I3_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "I3_b_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R1_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_b_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_c_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_d_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_e_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R2_f_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R3_a_Criteria", c => c.Boolean(nullable: false));
            AddColumn("dbo.MareanoDatasets", "R3_b_Criteria", c => c.Boolean(nullable: false));
            CreateIndex("dbo.MareanoDatasets", "FindableStatusId");
            CreateIndex("dbo.MareanoDatasets", "AccesibleStatusId");
            CreateIndex("dbo.MareanoDatasets", "InteroperableStatusId");
            CreateIndex("dbo.MareanoDatasets", "ReUseableStatusId");
            CreateIndex("dbo.MareanoDatasets", "FAIRStatusId");
            AddForeignKey("dbo.MareanoDatasets", "FindableStatusId", "dbo.FAIRDeliveries", "FAIRDeliveryId", cascadeDelete: false);
            AddForeignKey("dbo.MareanoDatasets", "AccesibleStatusId", "dbo.FAIRDeliveries", "FAIRDeliveryId", cascadeDelete: false);
            AddForeignKey("dbo.MareanoDatasets", "InteroperableStatusId", "dbo.FAIRDeliveries", "FAIRDeliveryId", cascadeDelete: false);
            AddForeignKey("dbo.MareanoDatasets", "ReUseableStatusId", "dbo.FAIRDeliveries", "FAIRDeliveryId", cascadeDelete: false);
            AddForeignKey("dbo.MareanoDatasets", "FAIRStatusId", "dbo.FAIRDeliveries", "FAIRDeliveryId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MareanoDatasets", "FAIRStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "ReUseableStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "InteroperableStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "AccesibleStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.MareanoDatasets", "FindableStatusId", "dbo.FAIRDeliveries");
            DropForeignKey("dbo.FAIRDeliveries", "StatusId", "dbo.FAIRDeliveryStatus");
            DropIndex("dbo.MareanoDatasets", new[] { "FAIRStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "ReUseableStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "InteroperableStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "AccesibleStatusId" });
            DropIndex("dbo.MareanoDatasets", new[] { "FindableStatusId" });
            DropIndex("dbo.FAIRDeliveries", new[] { "StatusId" });
            DropColumn("dbo.MareanoDatasets", "R3_b_Criteria");
            DropColumn("dbo.MareanoDatasets", "R3_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "R2_f_Criteria");
            DropColumn("dbo.MareanoDatasets", "R2_e_Criteria");
            DropColumn("dbo.MareanoDatasets", "R2_d_Criteria");
            DropColumn("dbo.MareanoDatasets", "R2_c_Criteria");
            DropColumn("dbo.MareanoDatasets", "R2_b_Criteria");
            DropColumn("dbo.MareanoDatasets", "R2_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "R1_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "I3_b_Criteria");
            DropColumn("dbo.MareanoDatasets", "I3_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "I2_b_Criteria");
            DropColumn("dbo.MareanoDatasets", "I2_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "I1_c_Criteria");
            DropColumn("dbo.MareanoDatasets", "I1_b_Criteria");
            DropColumn("dbo.MareanoDatasets", "I1_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "A2_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "A1_f_Criteria");
            DropColumn("dbo.MareanoDatasets", "A1_e_Criteria");
            DropColumn("dbo.MareanoDatasets", "A1_d_Criteria");
            DropColumn("dbo.MareanoDatasets", "A1_c_Criteria");
            DropColumn("dbo.MareanoDatasets", "A1_b_Criteria");
            DropColumn("dbo.MareanoDatasets", "A1_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "F4_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "F3_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "F2_c_Criteria");
            DropColumn("dbo.MareanoDatasets", "F2_b_Criteria");
            DropColumn("dbo.MareanoDatasets", "F2_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "F1_a_Criteria");
            DropColumn("dbo.MareanoDatasets", "FAIRStatusPerCent");
            DropColumn("dbo.MareanoDatasets", "FAIRStatusId");
            DropColumn("dbo.MareanoDatasets", "ReUseableStatusPerCent");
            DropColumn("dbo.MareanoDatasets", "ReUseableStatusId");
            DropColumn("dbo.MareanoDatasets", "InteroperableStatusPerCent");
            DropColumn("dbo.MareanoDatasets", "InteroperableStatusId");
            DropColumn("dbo.MareanoDatasets", "AccesibleStatusPerCent");
            DropColumn("dbo.MareanoDatasets", "AccesibleStatusId");
            DropColumn("dbo.MareanoDatasets", "FindableStatusPerCent");
            DropColumn("dbo.MareanoDatasets", "FindableStatusId");
            DropTable("dbo.FAIRDeliveryStatus");
            DropTable("dbo.FAIRDeliveries");
        }
    }
}
