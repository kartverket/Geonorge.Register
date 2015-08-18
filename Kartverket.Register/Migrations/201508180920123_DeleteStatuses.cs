namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteStatuses : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM Status WHERE (value = 'Accepted')");
            Sql("DELETE FROM Status WHERE (value = 'Experimental')");
            Sql("DELETE FROM Status WHERE (value = 'Deprecated')");
            Sql("DELETE FROM Status WHERE (value = 'Candidate')");
            Sql("DELETE FROM Status WHERE (value = 'InProgress')");
            Sql("DELETE FROM Status WHERE (value = 'Proposal')");
        }
        
        public override void Down()
        {
        }
    }
}
