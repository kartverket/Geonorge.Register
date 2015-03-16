namespace Kartverket.Register.Migrations
{
    using Kartverket.Register.Models;
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
    
    public partial class DeleteMetadataKodelisterSubregisters : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegister = from r in db.Registers
                                       where r.parentRegisterId.ToString() == "9A46038D-16EE-4562-96D2-8F6304AAB689"
                                       select r.systemId;

            List<Guid> register = queryResultsRegister.ToList();
            foreach (Guid item in register)
            {


                Sql("DELETE FROM Registers WHERE (systemId = '" + item.ToString() + "')");

            }
        }
        
        public override void Down()
        {
        }
    }
}
