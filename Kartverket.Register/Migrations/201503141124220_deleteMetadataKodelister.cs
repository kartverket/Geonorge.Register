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
    
    public partial class deleteMetadataKodelister : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegisterItem = from r in db.RegisterItems
                                           where r.register.parentRegisterId.ToString() == "9A46038D-16EE-4562-96D2-8F6304AAB689"
                                           select r.systemId;

            List<Guid> registeritems = queryResultsRegisterItem.ToList();

            foreach (Guid item in registeritems)
            {
                //RegisterItem registerItem = db.RegisterItems.Find(item);

                Sql("DELETE RegisterItems WHERE (systemId = '" + item + "')");

            }

            var queryResultsRegister = from r in db.Registers
                                       where r.parentRegisterId.ToString() == "9A46038D-16EE-4562-96D2-8F6304AAB689" || r.systemId.ToString() == "9A46038D-16EE-4562-96D2-8F6304AAB689"
                                       select r.systemId;

            List<Guid> register = queryResultsRegister.ToList();
            foreach (Guid item in register)
            {
                //RegisterItem registerItem = db.RegisterItems.Find(item);

                Sql("DELETE FROM Registers WHERE (systemId = '" + item + "')");

            }

                Sql("DELETE FROM Registers WHERE (systemId = '9A46038D-16EE-4562-96D2-8F6304AAB689')");
       }

       
        public override void Down()
        {
        }
    }
}
