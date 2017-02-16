namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDateConfirmedMunicipalDOK : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "DateConfirmedMunicipalDOK", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "DateConfirmedMunicipalDOK");
        }
    }
}
