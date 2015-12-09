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
    
    public partial class DeleteMetadataKoderItems : DbMigration
    {
        public override void Up()
        {
            //RegisterDbContext db = new RegisterDbContext();

            //var queryResults = from r in db.RegisterItems
            //                   where r.register.parentRegisterId.ToString() == "9A46038D-16EE-4562-96D2-8F6304AAB689"
            //                   select r.systemId;

            //List<Guid> registeritems = queryResults.ToList();

            //foreach (Guid item in registeritems)
            //{
            //    //RegisterItem registerItem = db.RegisterItems.Find(item);

            //    Sql("DELETE FROM RegisterItems WHERE (systemId = '" + item.ToString() + "')");

            //}
        }
        
        public override void Down()
        {
        }
    }
}
