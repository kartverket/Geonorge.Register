namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedNameEpsgAttDocumentAtt : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.RegisterItems","document","documentUrl");
            RenameColumn("dbo.RegisterItems","epsg","epsgcode");
            
        }
        
        public override void Down()
        {
            RenameColumn("dbo.RegisterItems", "documentUrl", "document");
            RenameColumn("dbo.RegisterItems", "epsgcode", "epsg");
        }
    }
}
