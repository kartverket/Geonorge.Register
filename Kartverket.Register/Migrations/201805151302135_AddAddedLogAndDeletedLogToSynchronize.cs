namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAddedLogAndDeletedLogToSynchronize : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SyncLogEntries", "Synchronize_Id", "dbo.Synchronizes");
            AddColumn("dbo.SyncLogEntries", "Synchronize_Id1", c => c.Guid());
            AddColumn("dbo.SyncLogEntries", "Synchronize_Id2", c => c.Guid());
            CreateIndex("dbo.SyncLogEntries", "Synchronize_Id1");
            CreateIndex("dbo.SyncLogEntries", "Synchronize_Id2");
            AddForeignKey("dbo.SyncLogEntries", "Synchronize_Id", "dbo.Synchronizes", "Id");
            AddForeignKey("dbo.SyncLogEntries", "Synchronize_Id1", "dbo.Synchronizes", "Id");
            AddForeignKey("dbo.SyncLogEntries", "Synchronize_Id2", "dbo.Synchronizes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SyncLogEntries", "Synchronize_Id2", "dbo.Synchronizes");
            DropForeignKey("dbo.SyncLogEntries", "Synchronize_Id1", "dbo.Synchronizes");
            DropForeignKey("dbo.SyncLogEntries", "Synchronize_Id", "dbo.Synchronizes");
            DropIndex("dbo.SyncLogEntries", new[] { "Synchronize_Id2" });
            DropIndex("dbo.SyncLogEntries", new[] { "Synchronize_Id1" });
            DropColumn("dbo.SyncLogEntries", "Synchronize_Id2");
            DropColumn("dbo.SyncLogEntries", "Synchronize_Id1");
            AddForeignKey("dbo.SyncLogEntries", "Synchronize_Id", "dbo.Synchronizes", "Id");
        }
    }
}
