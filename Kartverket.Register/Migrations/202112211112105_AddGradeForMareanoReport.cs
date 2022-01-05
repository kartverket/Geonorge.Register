namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGradeForMareanoReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MareanoDatasets", "Grade", c => c.Single());
            AddColumn("dbo.RegisterItemStatusReports", "Grade", c => c.Single());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItemStatusReports", "Grade");
            DropColumn("dbo.MareanoDatasets", "Grade");
        }
    }
}
