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
    
    public partial class UpdateSeonameRegisterItems : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegisteritems = from r in db.RegisterItems
                                       where r.seoname == null
                                       select r.systemId;

            List<Guid> systIdListe = queryResultsRegisteritems.ToList();

            foreach (Guid item in systIdListe)
            {
                RegisterItem registeritem = db.RegisterItems.Find(item);

                string name = registeritem.name;
                string seoName = ToUrl(name);

                Sql("UPDATE RegisterItems SET seoname = '" + seoName + "' WHERE  (systemId = '" + registeritem.systemId.ToString() + "')");

            }


        }

        private string ToUrl(string name)
        {
            string encodedUrl = (name ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "a").Replace("æ", "ae").Replace("ø", "o");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }

        public override void Down()
        {
        }
    }
}
