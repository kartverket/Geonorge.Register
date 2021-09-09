﻿namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodelistValueEnglish : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterItems", "valueEnglish", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisterItems", "valueEnglish");
        }
    }
}
