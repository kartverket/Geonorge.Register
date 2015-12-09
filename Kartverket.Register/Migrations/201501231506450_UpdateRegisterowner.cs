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
    
    public partial class UpdateRegisterowner : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegisteritems = from r in db.Registers
                                            where r.ownerId == null
                                            select r.systemId;

            List<Guid> systIdListe = queryResultsRegisteritems.ToList();

            foreach (Guid item in systIdListe)
            {
                Register register = db.Registers.Find(item);

                string ownerId = "10087020-f17c-45e1-8542-02acbcf3d8a3";


                Sql("UPDATE Registers SET ownerId = '" + ownerId + "' WHERE  (systemId = '" + register.systemId.ToString() + "')");

            }

        }
        
        public override void Down()
        {
        }
    }
}
