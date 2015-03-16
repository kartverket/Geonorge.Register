namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteMetadataKodeliste : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM Registers WHERE (systemId = '9A46038D-16EE-4562-96D2-8F6304AAB689')");
        }
        
        public override void Down()
        {
        }
    }
}
