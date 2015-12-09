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

    public partial class UpdateRegisterItemsVersioningId : DbMigration
    {
        public override void Up()
        {

            //RegisterDbContext db = new RegisterDbContext();

            //var queryResultsVersions = from r in db.Versions
            //                           select r;

            //List<Kartverket.Register.Models.Version> versions = queryResultsVersions.ToList();

            //var queryResultsRegisterItems = from r in db.RegisterItems
            //                                select r;
            //List<RegisterItem> registerItems = queryResultsRegisterItems.ToList();


            //foreach (Kartverket.Register.Models.Version v in versions)
            //{
            //    foreach (RegisterItem ri in registerItems)
            //    {
            //        if (v.currentVersion == ri.systemId)
            //        {
            //            ri.versioningId = v.systemId;
            //            string versjonsID = ri.versioningId.ToString();
            //            string systemID = ri.systemId.ToString();

            //            Sql("UPDATE RegisterItems SET versioningId = '" + versjonsID + "' WHERE (systemId = '" + systemID + "')");
            //        }
            //    }
            //}
        }

        public override void Down()
        {
        }
    }
}
