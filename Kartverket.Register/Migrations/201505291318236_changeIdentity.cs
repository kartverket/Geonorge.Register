namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeIdentity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.accessTypes", "accessLevel", c => c.Int(nullable: false, identity: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.accessTypes", "accessLevel", c => c.Int(nullable: false, identity: true));
        }
    }
}
