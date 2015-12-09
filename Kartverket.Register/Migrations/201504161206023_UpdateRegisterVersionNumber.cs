namespace Kartverket.Register.Migrations
{
    using Models;
    using System;
    using System.Data.Entity.Migrations;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Text.RegularExpressions;

    public partial class UpdateRegisterVersionNumber : DbMigration
    {
        public override void Up()
        {
            //RegisterDbContext db = new RegisterDbContext();

            //var queryResultsRegister = from r in db.Registers
            //                           where r.versionNumber == 0
            //                           select r.systemId;

            //List<Guid> registers = queryResultsRegister.ToList();

            //foreach (Guid item in registers)
            //{
            //    Kartverket.Register.Models.Register register = db.Registers.Find(item);
            //    int versionNumber = 1;

            //    Sql("UPDATE Registers SET versionNumber = '" + versionNumber + "' WHERE  (systemId = '" + register.systemId.ToString() + "')");

            //}
        }

        public override void Down()
        {
        }
    }
}
