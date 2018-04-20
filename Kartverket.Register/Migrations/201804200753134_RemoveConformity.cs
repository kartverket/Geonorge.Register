namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveConformity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InspireDataServices", "Conformity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InspireDataServices", "Conformity", c => c.Boolean(nullable: false));
        }
    }
}
