namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Status", "group", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Status", "group");
        }
    }
}
