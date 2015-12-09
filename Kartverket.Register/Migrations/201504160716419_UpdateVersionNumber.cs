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

    public partial class UpdateVersionNumber : DbMigration
    {
        public override void Up()
        {
            //RegisterDbContext db = new RegisterDbContext();

            //var queryResultsRegister = from r in db.RegisterItems
            //                           where r.versionNumber == 0
            //                           select r.systemId;

            //List<Guid> registeritems = queryResultsRegister.ToList();

            //foreach (Guid item in registeritems)
            //{
            //    RegisterItem registerItem = db.RegisterItems.Find(item);
            //    int versionNumber = 1;

            //    Sql("UPDATE RegisterItems SET versionNumber = '" + versionNumber + "' WHERE  (systemId = '" + registerItem.systemId.ToString() + "')");

            //}
        }

        public override void Down()
        {
        }
    }
}
