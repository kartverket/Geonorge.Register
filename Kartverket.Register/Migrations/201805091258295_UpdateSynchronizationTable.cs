namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSynchronizationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Synchronizes", "NumberOfItems", c => c.Int(nullable: false));
            AddColumn("dbo.Synchronizes", "NumberOfDeletedItems", c => c.Int(nullable: false));
            AddColumn("dbo.Synchronizes", "NumberOfNewItems", c => c.Int(nullable: false));
            AlterColumn("dbo.Synchronizes", "Stop", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Synchronizes", "Stop", c => c.DateTime(nullable: false));
            DropColumn("dbo.Synchronizes", "NumberOfNewItems");
            DropColumn("dbo.Synchronizes", "NumberOfDeletedItems");
            DropColumn("dbo.Synchronizes", "NumberOfItems");
        }
    }
}
