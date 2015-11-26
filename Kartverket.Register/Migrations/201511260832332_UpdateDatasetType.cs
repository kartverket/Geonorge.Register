namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatasetType : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE RegisterItems SET DatasetType = 'National' WHERE (DatasetType is NULL)");
        }
        
        public override void Down()
        {
        }
    }
}
