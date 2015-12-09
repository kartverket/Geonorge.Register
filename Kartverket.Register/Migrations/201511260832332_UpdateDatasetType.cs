namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatasetType : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE RegisterItems SET DatasetType = 'Nasjonalt' WHERE (DatasetType is NULL) or DatasetType = 'National' or DatasetType = 'Nasjonal'");
        }
        
        public override void Down()
        {
        }
    }
}
