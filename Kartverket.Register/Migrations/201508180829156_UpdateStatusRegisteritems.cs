namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStatusRegisteritems : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE RegisterItems SET statusId = 'Valid'  WHERE (statusId = 'Accepted')");
            Sql("UPDATE RegisterItems SET statusId = 'Submitted'  WHERE (statusId = 'Experimental')");
            Sql("UPDATE RegisterItems SET statusId = 'Submitted'  WHERE (statusId = 'Candidate')");
            Sql("UPDATE RegisterItems SET statusId = 'Submitted'  WHERE (statusId = 'InProgress')");
            Sql("UPDATE RegisterItems SET statusId = 'Submitted'  WHERE (statusId = 'Deprecated')");
            Sql("UPDATE RegisterItems SET statusId = 'Retired'  WHERE (statusId = 'Proposal')");
        }
        
        public override void Down()
        {
        }
    }
}
