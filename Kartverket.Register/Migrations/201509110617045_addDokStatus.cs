namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDokStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "dokStatusId", c => c.String(maxLength: 128));
            CreateIndex("dbo.RegisterItems", "dokStatusId");
            AddForeignKey("dbo.RegisterItems", "dokStatusId", "dbo.DokStatus", "value");

            Sql("UPDATE RegisterItems SET dokStatusId = 'Submitted' WHERE (dokStatusId is NULL AND Discriminator = 'Dataset')");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegisterItems", "dokStatusId", "dbo.DokStatus");
            DropIndex("dbo.RegisterItems", new[] { "dokStatusId" });
            DropColumn("dbo.RegisterItems", "dokStatusId");
        }
    }
}
