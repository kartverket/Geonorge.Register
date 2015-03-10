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
    
    public partial class UpdateMetadataUrl : DbMigration
    {
        public override void Up()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegister = from r in db.Datasets
                                       where r.MetadataUrl.Contains("www.geonorge.no/geonetwork/")
                                       select r.systemId;

            List<Guid> systIdListe = queryResultsRegister.ToList();

            foreach (Guid item in systIdListe)
            {
                Dataset dataset = db.Datasets.Find(item);

                string metadataUrl = dataset.MetadataUrl.Replace("www.geonorge.no/geonetwork/?uuid=", "kartkatalogen.dev.geonorge.no/metadata/uuid/");


                Sql("UPDATE Registeritems SET MetadataUrl = '" + metadataUrl + "' WHERE  (systemId = '" + dataset.systemId.ToString() + "')");

            }
        }
        
        public override void Down()
        {
        }
    }
}
