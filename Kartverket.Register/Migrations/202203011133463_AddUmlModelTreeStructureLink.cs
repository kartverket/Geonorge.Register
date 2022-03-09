namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUmlModelTreeStructureLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "UmlModelTreeStructureLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "UmlModelTreeStructureLink");
        }
    }
}
