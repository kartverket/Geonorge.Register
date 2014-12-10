namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEpsgRequirements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Requirements",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.RegisterItems", "inspireRequirementId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "inspireRequirementDescription", c => c.String());
            AddColumn("dbo.RegisterItems", "nationalRequirementId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "nationalRequirementDescription", c => c.String());
            AddColumn("dbo.RegisterItems", "nationalSeasRequirementId", c => c.String(maxLength: 128));
            AddColumn("dbo.RegisterItems", "nationalSeasRequirementDescription", c => c.String());
            CreateIndex("dbo.RegisterItems", "inspireRequirementId");
            CreateIndex("dbo.RegisterItems", "nationalRequirementId");
            CreateIndex("dbo.RegisterItems", "nationalSeasRequirementId");
            AddForeignKey("dbo.RegisterItems", "inspireRequirementId", "dbo.Requirements", "value");
            AddForeignKey("dbo.RegisterItems", "nationalRequirementId", "dbo.Requirements", "value");
            AddForeignKey("dbo.RegisterItems", "nationalSeasRequirementId", "dbo.Requirements", "value");
            DropColumn("dbo.RegisterItems", "inspireRequirement");
            DropColumn("dbo.RegisterItems", "nasjonalRequirements");
            DropColumn("dbo.RegisterItems", "nasjonalRequirementSeas");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItems", "nasjonalRequirementSeas", c => c.String());
            AddColumn("dbo.RegisterItems", "nasjonalRequirements", c => c.String());
            AddColumn("dbo.RegisterItems", "inspireRequirement", c => c.String());
            DropForeignKey("dbo.RegisterItems", "nationalSeasRequirementId", "dbo.Requirements");
            DropForeignKey("dbo.RegisterItems", "nationalRequirementId", "dbo.Requirements");
            DropForeignKey("dbo.RegisterItems", "inspireRequirementId", "dbo.Requirements");
            DropIndex("dbo.RegisterItems", new[] { "nationalSeasRequirementId" });
            DropIndex("dbo.RegisterItems", new[] { "nationalRequirementId" });
            DropIndex("dbo.RegisterItems", new[] { "inspireRequirementId" });
            DropColumn("dbo.RegisterItems", "nationalSeasRequirementDescription");
            DropColumn("dbo.RegisterItems", "nationalSeasRequirementId");
            DropColumn("dbo.RegisterItems", "nationalRequirementDescription");
            DropColumn("dbo.RegisterItems", "nationalRequirementId");
            DropColumn("dbo.RegisterItems", "inspireRequirementDescription");
            DropColumn("dbo.RegisterItems", "inspireRequirementId");
            DropTable("dbo.Requirements");
        }
    }
}
