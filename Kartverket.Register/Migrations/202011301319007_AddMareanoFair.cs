namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMareanoFair : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItemStatusReports", "FindableMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "AccesibleMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "InteroperableMareanoDataset", c => c.String());
            AddColumn("dbo.RegisterItemStatusReports", "ReUsableMareanoDataset", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "ReUsableMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "InteroperableMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "AccesibleMareanoDataset");
            DropColumn("dbo.RegisterItemStatusReports", "FindableMareanoDataset");
        }
    }
}
