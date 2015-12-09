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
    
    public partial class UpdateSubmitter : DbMigration
    {
        public override void Up()
        {
            //RegisterDbContext db = new RegisterDbContext();

            //var queryResults = from r in db.Documents
            //                   select r.systemId;

            //List<Guid> systId = queryResults.ToList();

            //foreach (Guid item in systId)
            //{
            //    Document document = db.Documents.Find(item);
            //    Guid documentOwner = document.documentownerId;

            //    Sql("UPDATE RegisterItems SET submitterId = '" + documentOwner + "' WHERE  (systemId = '" + item.ToString() + "')");

            //}
        }
        
        public override void Down()
        {
        }
    }
}
