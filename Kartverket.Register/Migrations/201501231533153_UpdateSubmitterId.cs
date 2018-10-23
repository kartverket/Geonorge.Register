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
    
    public partial class UpdateSubmitterId : DbMigration
    {
        public override void Up()
        {
            //RegisterDbContext db = new RegisterDbContext();

            //var queryResultsRegister = from r in db.RegisterItems
            //                           where r.submitterId == null
            //                           select r.systemId;

            //List<Guid> systIdListe = queryResultsRegister.ToList();

            //foreach (Guid item in systIdListe)
            //{
            //    RegisterItem registerItem = db.RegisterItems.Find(item);

            //    string submitterId = "10087020-f17c-45e1-8542-02acbcf3d8a3";


            //    Sql("UPDATE RegisterItems SET submitterId = '" + submitterId + "' WHERE  (systemId = '" + registerItem.systemId.ToString() + "')");

            //}
        }
        
        public override void Down()
        {
        }
    }
}
