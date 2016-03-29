namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoServiceAlertNoteRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RegisterItems", "Note", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RegisterItems", "Note", c => c.String());
        }
    }
}
