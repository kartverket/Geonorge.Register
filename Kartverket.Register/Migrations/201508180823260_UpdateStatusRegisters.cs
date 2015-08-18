namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStatusRegisters : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE Registers SET statusId = 'Valid'  WHERE (statusId = 'Accepted')");
            Sql("UPDATE Registers SET statusId = 'Submitted'  WHERE (statusId = 'Experimental')");
            Sql("UPDATE Registers SET statusId = 'Submitted'  WHERE (statusId = 'Candidate')");
            Sql("UPDATE Registers SET statusId = 'Submitted'  WHERE (statusId = 'InProgress')");
            Sql("UPDATE Registers SET statusId = 'Submitted'  WHERE (statusId = 'Deprecated')");
            Sql("UPDATE Registers SET statusId = 'Retired'  WHERE (statusId = 'Proposal')");
        }
        
        public override void Down()
        {
        }
    }
}
