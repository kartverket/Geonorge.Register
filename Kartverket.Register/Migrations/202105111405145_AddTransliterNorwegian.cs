namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransliterNorwegian : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registers", "TransliterNorwegian", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Registers", "TransliterNorwegian");
        }
    }
}
