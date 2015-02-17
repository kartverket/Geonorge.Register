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
    
    public partial class UpdateStatusId : DbMigration
    {
        public override void Up()
        {
            //RegisterDbContext db = new RegisterDbContext();

            //var queryResultsRegister = from r in db.RegisterItems
            //                           where r.statusId == null
            //                           select r.systemId;

            //List<Guid> systIdListe = queryResultsRegister.ToList();

            //foreach (Guid item in systIdListe)
            //{
            //    RegisterItem registerItem = db.RegisterItems.Find(item);

            //    string statusId = "Submitted";


            //    Sql("UPDATE RegisterItems SET statusId = '" + statusId + "' WHERE  (systemId = '" + registerItem.systemId.ToString() + "')");

            //}
        }
        
        public override void Down()
        {
        }
    }
}
