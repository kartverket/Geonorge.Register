namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HoizontalAndVertRefSys : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "verticalReferenceSystem", c => c.String());
            AddColumn("dbo.RegisterItems", "horizontalReferenceSystem", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "horizontalReferenceSystem");
            DropColumn("dbo.RegisterItems", "verticalReferenceSystem");
        }
    }
}
