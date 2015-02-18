namespace Kartverket.Register.Migrations
{
    using Kartverket.Register.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Text.RegularExpressions;
    
    public partial class fixSubmitter : DbMigration
    {
        public override void Up()
        {

            RegisterDbContext db = new RegisterDbContext();

            var queryResults = from r in db.Documents
                               select r.systemId;

            List<Guid> systId = queryResults.ToList();

            foreach (Guid item in systId)
            {
                Document document = db.Documents.Find(item);
                Guid documentOwner = document.documentownerId;

                Sql("UPDATE RegisterItems SET submitterId = '" + documentOwner + "' WHERE  (systemId = '" + item.ToString() + "')");

            }

            Register dokregister = new Register
            {
                systemId = Guid.Parse("CD429E8B-2533-45D8-BCAA-86BC2CBDD0DD"),
                dateSubmitted = DateTime.Now,
                modified = DateTime.Now,
                dateAccepted = DateTime.Now,
                name = "Det offentlige kartgrunnlaget",
                description = "Det offentlige kartgrunnlaget beskrives i plan- og bygningslovens paragraf 2-1 og kart- og planforskriften og skal være er en samling geografiske kvalitetsdata, såkalt offentlige autoritative data. Disse skal være valgt ut og tilrettelagt for å være et egnet kunnskapsgrunnlag for de mest vesentlige behovene som følger av plan- og bygningsloven.",
                containedItemClass = "Dataset",
                statusId = "Valid",
                seoname = "det-offentlige-kartgrunnlaget",
                managerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3"),
                ownerId = Guid.Parse("10087020-F17C-45E1-8542-02ACBCF3D8A3")

            };

            db.Registers.AddOrUpdate(
                dokregister
           );
            db.SaveChanges();

            var queryResultsRegister = from r in db.RegisterItems
                                       where r.statusId == null
                                       select r.systemId;

            List<Guid> systIdListe = queryResultsRegister.ToList();

            foreach (Guid item in systIdListe)
            {
                RegisterItem registerItem = db.RegisterItems.Find(item);

                string statusId = "Submitted";


                Sql("UPDATE RegisterItems SET statusId = '" + statusId + "' WHERE  (systemId = '" + registerItem.systemId.ToString() + "')");

            }

        }
        
        public override void Down()
        {
        }
    }
}
