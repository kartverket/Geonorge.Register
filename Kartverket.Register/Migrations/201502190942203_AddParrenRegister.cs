namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddParrenRegister : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Registers", name: "Register_systemId", newName: "parentRegisterId");
            RenameIndex(table: "dbo.Registers", name: "IX_Register_systemId", newName: "IX_parentRegisterId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Registers", name: "IX_parentRegisterId", newName: "IX_Register_systemId");
            RenameColumn(table: "dbo.Registers", name: "parentRegisterId", newName: "Register_systemId");
        }
    }
}
