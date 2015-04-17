namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemsToVersionTable : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Versions (systemId, currentVersion, lastVersionNumber, containedItemClass) SELECT NEWID() as systemId, systemId as currentVersion, versionNumber as lastVersionNumber, containedItemClass as containedItemClass FROM Registers");
            Sql("INSERT INTO Versions (systemId, currentVersion, lastVersionNumber, containedItemClass) SELECT NEWID() as systemId, systemId as currentVersion, versionNumber as lastVersionNumber, Discriminator as containedItemClass FROM RegisterItems");
        }
        
        public override void Down()
        {
        }
    }
}
