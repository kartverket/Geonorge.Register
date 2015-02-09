using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.IO;
using Kartverket.Register.Helpers;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Kartverket.Register.Controllers
{
    public class DatasetsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: Datasets
        public ActionResult Index()
        {
            var datasets = db.Datasets.Include(d => d.register).Include(d => d.status).Include(d => d.submitter);
            return View(datasets.ToList());
        }

        // GET: Datasets/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dataset dataset = db.Datasets.Find(id);
            if (dataset == null)
            {
                return HttpNotFound();
            }
            return View(dataset);
        }

        // GET: Datasets/Create
        [Authorize]
        [Route("dataset/{registername}/ny")]
        public ActionResult Create(string registername)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("dataset");

            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);
            string registerStatus = register.statusId;

            if (role == "nd.metadata_admin" || role == "nd.metadata" && register.statusId == "Submitted")
            {
                return View();
            }
            return HttpNotFound();
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name");
            //ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
            //ViewBag.submitterId = new SelectList(db.RegisterItems, "systemId", "name");
            //ViewBag.datasetownerId = new SelectList(db.RegisterItems, "systemId", "name");
            //return View();
        }

        // POST: Datasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("dataset/{registername}/ny")]
        public ActionResult Create(Dataset dataset, string registername)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername
                                       select o.systemId;

            Guid regId = queryResultsRegister.First();

            if (ModelState.IsValid)
            {
                dataset.systemId = Guid.NewGuid();
                dataset.modified = DateTime.Now;
                dataset.dateSubmitted = DateTime.Now;
                dataset.registerId = regId;
                dataset.statusId = "Submitted";

                if (dataset.name == null || dataset.name.Length == 0)
                {
                    dataset.name = "ikke angitt";
                    dataset.seoname = dataset.systemId.ToString();
                }
                if (dataset.description == null || dataset.description.Length == 0)
                {
                    dataset.description = "ikke angitt";
                }
                dataset.seoname = MakeSeoFriendlyString(dataset.name);

                string organizationLogin = GetSecurityClaim("organization");

                var queryResults = from o in db.Organizations
                                   where o.name == organizationLogin
                                   select o.systemId;

                Guid orgId = queryResults.First();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                dataset.submitterId = orgId;
                dataset.submitter = submitterOrganisasjon;
                dataset.datasetowner = submitterOrganisasjon;
                dataset.datasetownerId = orgId;

                db.Entry(dataset).State = EntityState.Modified;
                //db.SaveChanges();

                db.RegisterItems.Add(dataset);
                db.SaveChanges();

                return Redirect("/register/" + registername);
            }

            return View(dataset);
        }

        // GET: Datasets/Edit/5
        [Authorize]
        [Route("dataset/{registername}/{organization}/{datasetname}/rediger")]
        public ActionResult Edit(string registername, string datasetname)
        {
            string registerOwner = FindRegisterOwner(registername);
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin" || user == registerOwner)
            {

                var queryResults = from o in db.Datasets
                                   where o.seoname == datasetname && o.register.seoname == registername
                                   select o.systemId;

                Guid systId = queryResults.First();

                if (systId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Dataset dataset = db.Datasets.Find(systId);
                if (dataset == null)
                {
                    return HttpNotFound();
                }
                ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", dataset.registerId);
                ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", dataset.statusId);
                ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", dataset.submitterId);
                ViewBag.datasetownerId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", dataset.datasetownerId);
                return View(dataset);
            }
            return HttpNotFound();
        }


        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("dataset/{registername}/{organization}/{datasetname}/rediger")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Dataset dataset, string registername, string datasetname)
        {
            var queryResults = from o in db.Datasets
                               where o.seoname == datasetname && o.register.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();

            if (ModelState.IsValid)
            {
                Dataset originalDataset = db.Datasets.Find(systId);
                if (dataset.name != null) originalDataset.name = dataset.name; originalDataset.seoname = MakeSeoFriendlyString(originalDataset.name);
                if (dataset.description != null) originalDataset.description = dataset.description;
                if (dataset.datasetownerId != null) originalDataset.datasetownerId = dataset.datasetownerId;
                if (dataset.submitterId != null) originalDataset.submitterId = dataset.submitterId;
                if (dataset.statusId != null)
                {
                    if (dataset.statusId == "Accepted" && originalDataset.statusId != "Accepted")
                    {
                        originalDataset.dateAccepted = DateTime.Now;
                    }
                    if (originalDataset.statusId == "Accepted" && dataset.statusId != "Accepted")
                    {
                        originalDataset.dateAccepted = null;
                    }
                    originalDataset.statusId = dataset.statusId;
                }

                if (dataset.DistributionUrl != null) originalDataset.DistributionUrl = dataset.DistributionUrl;
                if (dataset.MetadataUrl != null) originalDataset.MetadataUrl = dataset.MetadataUrl;
                if (dataset.PresentationRulesUrl != null) originalDataset.PresentationRulesUrl = dataset.PresentationRulesUrl;
                if (dataset.ProductSheetUrl != null) originalDataset.ProductSheetUrl = dataset.ProductSheetUrl;
                if (dataset.ProductSpecificationUrl != null) originalDataset.ProductSpecificationUrl = dataset.ProductSpecificationUrl;
                if (dataset.WmsUrl != null) originalDataset.WmsUrl = dataset.WmsUrl;
                if (dataset.DistributionFormat != null) originalDataset.DistributionFormat = dataset.DistributionFormat;
                if (dataset.DistributionArea != null) originalDataset.DistributionArea = dataset.DistributionArea;
                if (dataset.Notes != null) originalDataset.Notes = dataset.Notes;
                
               
                originalDataset.modified = DateTime.Now;
                db.Entry(originalDataset).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", originalDataset.statusId);
                ViewBag.submitterId = new SelectList(db.Organizations, "systemId", "name", originalDataset.submitterId);
                ViewBag.datasetownerId = new SelectList(db.Organizations, "systemId", "name", originalDataset.datasetownerId);

                return Redirect("/register/" + registername + "/" + originalDataset.datasetowner.seoname + "/" + originalDataset.seoname);
            }
            return View(dataset);
        }

        // GET: Documents/Delete/5
        [Authorize]
        [Route("dataset/{registername}/{organization}/{datasetname}/slett")]
        public ActionResult Delete(string registername, string datasetname)
        {
            string registerOwner = FindRegisterOwner(registername);
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin" || user == registerOwner)
            {
                var queryResults = from o in db.Datasets
                                   where o.seoname == datasetname && o.register.seoname == registername
                                   select o.systemId;

                Guid systId = queryResults.First();

                if (systId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Dataset dataset = db.Datasets.Find(systId);
                if (dataset == null)
                {
                    return HttpNotFound();
                }
                return View(dataset);
            }
            return HttpNotFound();
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("dataset/{registername}/{organization}/{datasetname}/slett")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string registername, string datasetname)
        {
            var queryResults = from o in db.Datasets
                               where o.seoname == datasetname && o.register.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();

            Dataset dataset = db.Datasets.Find(systId);
            db.RegisterItems.Remove(dataset);
            db.SaveChanges();
            return Redirect("/register/" + registername);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        private string GetSecurityClaim(string type)
        {
            string result = null;
            foreach (var claim in System.Security.Claims.ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result = claim.Value;
                    break;
                }
            }

            // bad hack, must fix BAAT
            if (!string.IsNullOrWhiteSpace(result) && type.Equals("organization") && result.Equals("Statens kartverk"))
            {
                result = "Kartverket";
            }

            return result;
        }

        private static string MakeSeoFriendlyString(string input)
        {
            string encodedUrl = (input ?? "").ToLower();

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

        private string FindRegisterOwner(string registername)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid regId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
            string registerOwner = register.owner.name;
            return registerOwner;
        }
    }
}
