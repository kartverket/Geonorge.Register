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
    
    public partial class UpdateSeoname : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegister = from r in db.Registers
                                       where r.seoname == null
                                       select r.systemId;

            List<Guid> systIdListe = queryResultsRegister.ToList();

            //Guid systemID = queryResultsRegister.First();

            foreach (Guid item in systIdListe)
            {
                Register register = db.Registers.Find(item);

                string name = register.name;
                string seoName = ToUrl(name);

                //register.seoname = seoName;

                //db.Entry(register).State = EntityState.Modified;
                //db.SaveChanges();

                Sql("UPDATE Registers SET seoname = '" + seoName + "' WHERE  (systemId = '" + register.systemId.ToString() + "')");

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
