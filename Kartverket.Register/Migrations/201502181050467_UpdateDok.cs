namespace Kartverket.Register.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Text.RegularExpressions;
    
    public partial class UpdateDok : DbMigration
    {
        public override void Up()
        {



            RenameColumn(table: "dbo.RegisterItems", name: "theme_value", newName: "ThemeGroupId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_theme_value", newName: "IX_ThemeGroupId");
            AddColumn("dbo.RegisterItems", "Uuid", c => c.String());
            AddColumn("dbo.RegisterItems", "Notes", c => c.String());
            AddColumn("dbo.RegisterItems", "ProductSheetUrl", c => c.String());
            AddColumn("dbo.RegisterItems", "PresentationRulesUrl", c => c.String());
            AddColumn("dbo.RegisterItems", "ProductSpecificationUrl", c => c.String());
            AddColumn("dbo.RegisterItems", "MetadataUrl", c => c.String());
            AddColumn("dbo.RegisterItems", "DistributionUrl", c => c.String());
            DropColumn("dbo.RegisterItems", "productsheet");
            DropColumn("dbo.RegisterItems", "presentationRules");
            DropColumn("dbo.RegisterItems", "productspesification");
            DropColumn("dbo.RegisterItems", "metadata");
            DropColumn("dbo.RegisterItems", "distributionUri");
            DropColumn("dbo.RegisterItems", "metadataUuid");


           

        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItems", "metadataUuid", c => c.String());
            AddColumn("dbo.RegisterItems", "distributionUri", c => c.String());
            AddColumn("dbo.RegisterItems", "metadata", c => c.String());
            AddColumn("dbo.RegisterItems", "productspesification", c => c.String());
            AddColumn("dbo.RegisterItems", "presentationRules", c => c.String());
            AddColumn("dbo.RegisterItems", "productsheet", c => c.String());
            DropColumn("dbo.RegisterItems", "DistributionUrl");
            DropColumn("dbo.RegisterItems", "MetadataUrl");
            DropColumn("dbo.RegisterItems", "ProductSpecificationUrl");
            DropColumn("dbo.RegisterItems", "PresentationRulesUrl");
            DropColumn("dbo.RegisterItems", "ProductSheetUrl");
            DropColumn("dbo.RegisterItems", "Notes");
            DropColumn("dbo.RegisterItems", "Uuid");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_ThemeGroupId", newName: "IX_theme_value");
            RenameColumn(table: "dbo.RegisterItems", name: "ThemeGroupId", newName: "theme_value");
        }
    }
}
