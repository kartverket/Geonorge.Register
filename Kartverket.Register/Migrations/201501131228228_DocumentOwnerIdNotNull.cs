namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentOwnerIdNotNull : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE RegisterItems SET documentownerId = '10087020-f17c-45e1-8542-02acbcf3d8a3' WHERE  (documentownerId IS NULL) AND (Discriminator = 'Document')");
        }
        
        public override void Down()
        {
        }
    }
}
