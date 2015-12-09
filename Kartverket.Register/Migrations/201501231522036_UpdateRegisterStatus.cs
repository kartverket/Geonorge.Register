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
    
    public partial class UpdateRegisterStatus : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegister = from r in db.Registers
                                            where r.statusId == null
                                            select r.systemId;

            List<Guid> systIdListe = queryResultsRegister.ToList();

            foreach (Guid item in systIdListe)
            {
                Register register = db.Registers.Find(item);

                string statusId = "Submitted";


                Sql("UPDATE Registers SET statusId = '" + statusId + "' WHERE  (systemId = '" + register.systemId.ToString() + "')");

            }
        }
        
        public override void Down()
        {
        }
    }
}
