namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDokStatusDateAccepted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "dokStatusDateAccepted", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "dokStatusDateAccepted");
        }
    }
}
