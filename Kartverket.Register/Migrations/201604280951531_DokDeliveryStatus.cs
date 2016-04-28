namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DokDeliveryStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DokDeliveryStatus",
                c => new
                    {
                        value = c.String(nullable: false, maxLength: 128),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.value);
            
            AddColumn("dbo.RegisterItems", "dokDeliveryMetadataStatusId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RegisterItems", "dokDeliveryMetadataStatusId");
            AddForeignKey("dbo.RegisterItems", "dokDeliveryMetadataStatusId", "dbo.DokDeliveryStatus", "value");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "dokDeliveryMetadataStatusId", "dbo.DokDeliveryStatus");
            DropIndex("dbo.RegisterItems", new[] { "dokDeliveryMetadataStatusId" });
            DropColumn("dbo.RegisterItems", "dokDeliveryMetadataStatusId");
            DropTable("dbo.DokDeliveryStatus");
        }
    }
}
