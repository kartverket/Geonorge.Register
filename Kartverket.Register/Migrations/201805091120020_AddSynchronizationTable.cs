namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSynchronizationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Synchronizes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Start = c.DateTime(nullable: false),
                        Stop = c.DateTime(nullable: true),
                        Active = c.Boolean(nullable: false),
                        SuccessCount = c.Int(nullable: false),
                        FailCount = c.Int(nullable: false),
                        Register_systemId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Registers", t => t.Register_systemId)
                .Index(t => t.Register_systemId);
            
            CreateTable(
                "dbo.SyncLogEntries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Uuid = c.String(),
                        Name = c.String(),
                        Message = c.String(),
                        Synchronize_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Synchronizes", t => t.Synchronize_Id)
                .Index(t => t.Synchronize_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Synchronizes", "Register_systemId", "dbo.Registers");
            DropForeignKey("dbo.SyncLogEntries", "Synchronize_Id", "dbo.Synchronizes");
            DropIndex("dbo.SyncLogEntries", new[] { "Synchronize_Id" });
            DropIndex("dbo.Synchronizes", new[] { "Register_systemId" });
            DropTable("dbo.SyncLogEntries");
            DropTable("dbo.Synchronizes");
        }
    }
}
