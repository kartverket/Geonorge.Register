namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConformity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspireDataServices", "Conformity", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspireDataServices", "Conformity");
        }
    }
}
