﻿namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentUrl2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "documentUrl2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "documentUrl2");
        }
    }
}
