namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlertImageThumbnail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "Image1Thumbnail", c => c.String());
            AddColumn("dbo.RegisterItems", "Image2Thumbnail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "Image2Thumbnail");
            DropColumn("dbo.RegisterItems", "Image1Thumbnail");
        }
    }
}
