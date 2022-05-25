namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNamespaceDataset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NamespaceDatasets",
                c => new
                    {
                        SystemId = c.Guid(nullable: false),
                        NameSpaceId = c.Guid(nullable: false),
                        MetadataUuid = c.String(),
                        MetadataNavn = c.String(),
                        Organisasjon = c.String(),
                        DatasettId = c.String(),
                        RedirectUrl = c.String(),
                    })
                .PrimaryKey(t => t.SystemId)
                .ForeignKey("dbo.RegisterItems", t => t.NameSpaceId, cascadeDelete: true)
                .Index(t => t.NameSpaceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NamespaceDatasets", "NameSpaceId", "dbo.RegisterItems");
            DropIndex("dbo.NamespaceDatasets", new[] { "NameSpaceId" });
            DropTable("dbo.NamespaceDatasets");
        }
    }
}
