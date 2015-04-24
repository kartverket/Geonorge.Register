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

    public partial class UpdateRegistersVersioningId : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsVersions = from r in db.Versions
                                       select r;

            List<Kartverket.Register.Models.Version> versions = queryResultsVersions.ToList();

            var queryResultsRegisterItems = from r in db.Registers
                                            select r;
            List<Kartverket.Register.Models.Register> registers = queryResultsRegisterItems.ToList();


            foreach (Kartverket.Register.Models.Version v in versions)
            {
                foreach (Kartverket.Register.Models.Register r in registers)
                {
                    if (v.currentVersion == r.systemId)
                    {
                        r.versioningId = v.systemId;
                        string versjonsID = r.versioningId.ToString();
                        string systemID = r.systemId.ToString();

                        Sql("UPDATE Registers SET versioningId = '" + versjonsID + "' WHERE (systemId = '" + systemID + "')");
                    }
                }
            }
        }

        public override void Down()
        {
        }
    }
}
