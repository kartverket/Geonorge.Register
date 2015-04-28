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
    
    public partial class UpdateStatusIdVersioning : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegister = from r in db.RegisterItems
                                       where r.statusId == "NotAccepted" || 
                                            r.statusId == "Accepted" || 
                                            r.statusId == "Experimental" || 
                                            r.statusId == "Deprecated" || 
                                            r.statusId == "Candidate" || 
                                            r.statusId == "InProgress" ||
                                            r.statusId == "Proposal"
                                       select r;

            List<RegisterItem> registers = queryResultsRegister.ToList();
            foreach (RegisterItem item in registers)
            {
                if (item.statusId == "NotAccepted" || item.statusId == "Proposal" || item.statusId == "InProgress" || item.statusId == "Candidate" || item.statusId == "Experimental" || item.statusId == "Deprecated")
                {
                    item.statusId = "Submitted";
                }
                if (item.statusId == "Accepted")
                {
                    item.statusId = "Valid";
                }

                Sql("UPDATE RegisterItems SET statusId = '" + item.statusId + "' WHERE  (systemId = '" + item.statusId.ToString() + "')");

            }
        }
        
        public override void Down()
        {
        }
    }
}
