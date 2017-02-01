namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatasetKandidatdato : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "Kandidatdato", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "Kandidatdato");
        }
    }
}
