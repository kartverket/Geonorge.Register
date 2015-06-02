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
using System.ComponentModel.DataAnnotations;
using Kartverket.DOK.Service;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class DatasetsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dataset/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            Dataset dataset = new Dataset();
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("dataset");

            var queryResults = from o in db.Registers
                               where o.seoname == registername && o.parentRegister.seoname == parentRegister
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);
            dataset.register = register;

            if (register.parentRegisterId != null)
            {
                dataset.register.parentRegister = register.parentRegister;
            }

            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description");

            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && register.accessId == 2))
            {
                return View(dataset);
            }
            return HttpNotFound("Ingen tilgang");
        }


        // POST: Datasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dataset/{registername}/ny")]
        public ActionResult Create(Dataset dataset, string registername, string uuid, string parentRegister, string registerowner)
        {
            if (uuid != null)
            {
                var model = new Dataset();
                try
                {
                    new MetadataService().UpdateDatasetWithMetadata(model, uuid);
                }
                catch (Exception e)
                {
                    TempData["error"] = "Det oppstod en feil ved henting av metadata: " + e.Message;
                }

                ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", dataset.ThemeGroupId);
                ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
                return View(model);
            }

            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername && o.parentRegister.seoname == parentRegister
                                       select o.systemId;

            Guid regId = queryResultsRegister.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);

            ValidationName(dataset, register);

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

                Guid orgId = queryResults.FirstOrDefault();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                dataset.submitterId = orgId;
                dataset.submitter = submitterOrganisasjon;
                dataset.datasetowner = submitterOrganisasjon;
                dataset.datasetownerId = orgId;

                db.Entry(dataset).State = EntityState.Modified;
                //db.SaveChanges();

                db.RegisterItems.Add(dataset);
                db.SaveChanges();
                ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", dataset.ThemeGroupId);
                if (!String.IsNullOrWhiteSpace(parentRegister))
                {
                    return Redirect("/subregister/" + dataset.register.parentRegister.seoname + "/" + dataset.register.parentRegister.owner.seoname + "/" + registername + "/" + "/" + dataset.datasetowner.seoname + "/" + dataset.seoname);
                }
                else
                {
                    return Redirect("/register/" + registername + "/" + dataset.datasetowner.seoname + "/" + dataset.seoname);
                }

            }
            dataset.register = register;
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", dataset.ThemeGroupId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
        
            return View(dataset);
        }

        private void ValidationName(Dataset dataset, Kartverket.Register.Models.Register register)
        {
            var queryResultsDataset = from o in db.Datasets
                                      where o.name == dataset.name &&
                                      o.systemId != dataset.systemId &&
                                      o.register.name == register.name &&
                                      o.register.parentRegisterId == register.parentRegisterId
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
            }
        }

        // GET: Datasets/Edit/5
        [Authorize]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{datasetname}/rediger")]
        [Route("dataset/{registername}/{organization}/{datasetname}/rediger")]
        public ActionResult Edit(string registername, string datasetname, string parentRegister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");


            var queryResults = from o in db.Datasets
                               where o.seoname == datasetname &&
                               o.register.seoname == registername &&
                               o.register.parentRegister.seoname == parentRegister
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dataset dataset = db.Datasets.Find(systId);
            if (dataset == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && dataset.register.accessId == 2 && dataset.datasetowner.name == user))
            {
                Viewbags(dataset);
                return View(dataset);
            }
            return HttpNotFound("Ingen tilgang");
        }

        private void Viewbags(Dataset dataset)
        {
            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", dataset.registerId);
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", dataset.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", dataset.submitterId);
            ViewBag.datasetownerId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", dataset.datasetownerId);
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", dataset.ThemeGroupId);
        }


        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{datasetname}/rediger")]
        [Route("dataset/{registername}/{organization}/{datasetname}/rediger")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Dataset dataset, string registername, string datasetname, string uuid, bool dontUpdateDescription, string parentRegister)
        {
            var queryResults = from o in db.Datasets
                               where o.seoname == datasetname && o.register.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
            Dataset originalDataset = db.Datasets.Find(systId);

            //Finn registerobjektet
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername && o.parentRegister.seoname == parentRegister
                                       select o.systemId;

            Guid regId = queryResultsRegister.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);

            ValidationName(dataset, register);

            if (dataset.name == null)
            {
                var model = db.Datasets.Find(originalDataset.systemId);

                string originalDescription = model.description;

                try
                {
                    new MetadataService().UpdateDatasetWithMetadata(model, uuid);
                }
                catch (Exception e)
                {
                    TempData["error"] = "Det oppstod en feil ved henting av metadata: " + e.Message;
                }

                if (dontUpdateDescription) model.description = originalDescription;

                Viewbags(model);
                return View(model);
            }

            if (ModelState.IsValid)
            {

                if (dataset.name != null) originalDataset.name = dataset.name;
                originalDataset.seoname = MakeSeoFriendlyString(originalDataset.name);
                originalDataset.description = dataset.description;
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

                originalDataset.DistributionUrl = dataset.DistributionUrl;
                originalDataset.MetadataUrl = dataset.MetadataUrl;
                originalDataset.PresentationRulesUrl = dataset.PresentationRulesUrl;
                originalDataset.ProductSheetUrl = dataset.ProductSheetUrl;
                originalDataset.ProductSpecificationUrl = dataset.ProductSpecificationUrl;
                originalDataset.WmsUrl = dataset.WmsUrl;
                originalDataset.DistributionFormat = dataset.DistributionFormat;
                originalDataset.DistributionArea = dataset.DistributionArea;
                originalDataset.Notes = dataset.Notes;
                originalDataset.ThemeGroupId = dataset.ThemeGroupId;
                originalDataset.datasetthumbnail = dataset.datasetthumbnail;


                originalDataset.modified = DateTime.Now;
                db.Entry(originalDataset).State = EntityState.Modified;
                db.SaveChanges();
                Viewbags(originalDataset);

                if (!String.IsNullOrWhiteSpace(parentRegister))
                {
                    return Redirect("/subregister/" + parentRegister + "/" + register.parentRegister.owner.seoname + "/" + registername + "/" + "/" + originalDataset.datasetowner.seoname + "/" + originalDataset.seoname);
                }
                else
                {
                    return Redirect("/register/" + registername + "/" + originalDataset.datasetowner.seoname + "/" + originalDataset.seoname);
                }
            }
            Viewbags(originalDataset);
            return View(originalDataset);
        }


        // GET: Documents/Delete/5
        [Authorize]
        [Route("dataset/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{datasetname}/slett")]
        [Route("dataset/{registername}/{organization}/{datasetname}/slett")]
        public ActionResult Delete(string registername, string datasetname, string parentregister, string parentregisterowner)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");


            var queryResults = from o in db.Datasets
                               where o.seoname == datasetname && o.register.seoname == registername
                               && o.register.parentRegister.seoname == parentregister
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dataset dataset = db.Datasets.Find(systId);
            if (dataset == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && dataset.register.accessId == 2 && dataset.datasetowner.name == user))
            {
                Viewbags(dataset);
                return View(dataset);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("dataset/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{datasetname}/slett")]
        [Route("dataset/{registername}/{organization}/{datasetname}/slett")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string registername, string datasetname, string parentregister, string parentregisterowner)
        {
            var queryResults = from o in db.Datasets
                               where o.seoname == datasetname && o.register.seoname == registername
                               && o.register.parentRegister.seoname == parentregister
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
            Dataset dataset = db.Datasets.Find(systId);

            string parent = null;
            if (dataset.register.parentRegisterId != null)
            {
                parent = dataset.register.parentRegister.seoname;
            }

            db.RegisterItems.Remove(dataset);
            db.SaveChanges();

            if (parent != null)
            {
                return Redirect("/subregister/" + parentregister + "/" + parentregisterowner + "/" + registername);
            }

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

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}
