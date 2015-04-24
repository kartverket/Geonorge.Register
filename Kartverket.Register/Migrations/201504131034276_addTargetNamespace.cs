namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addTargetNamespace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registers", "targetNamespace", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Registers", "targetNamespace");
        }
    }
}
