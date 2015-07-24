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
    
    public partial class UpdateLastVersionNumber : DbMigration
    {
        public override void Up()
        {
            //RegisterDbContext db = new RegisterDbContext();

            //var queryResultsVersions = from r in db.Versions
            //                           select r;

            //List<Kartverket.Register.Models.Version> versions = queryResultsVersions.ToList();

            //foreach (Kartverket.Register.Models.Version v in versions)
            //{
            //    string systemID = v.systemId.ToString();
            //    int startVersjonsNummer = 1;

            //    Sql("UPDATE Versions SET lastVersionNumber = '" + startVersjonsNummer + "' WHERE (systemId = '" + systemID + "')");


            //}
        }
        
        public override void Down()
        {
        }
    }
}
