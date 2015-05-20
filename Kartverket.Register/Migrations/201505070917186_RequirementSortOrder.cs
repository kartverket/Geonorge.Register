namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequirementSortOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requirements", "sortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requirements", "sortOrder");
        }
    }
}
