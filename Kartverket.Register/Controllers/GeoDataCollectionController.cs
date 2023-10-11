using Geonorge.AuthLib.Common;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Resources;
using System.IO;
using System.Security.Claims;
using Ganss.Xss;

namespace Kartverket.Register.Controllers
{
    public class GeoDataCollectionController : Controller
    {
        private readonly RegisterDbContext _dbContext;

        public GeoDataCollectionController(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: GeoDataCollection
        public ActionResult Index(string text = null, string sorting = null)
        {
            ViewBag.register = GeodataCollection.RegisterName;
            ViewBag.registerSEO = GeodataCollection.RegisterSeoName;

            IEnumerable<GeoDataCollection> query;

            if (!string.IsNullOrEmpty(text))
                query = _dbContext.GeoDataCollections.Where(g => g.Title.Contains(text) || g.Purpose.Contains(text)).Include("Organization");
            else
                query = _dbContext.GeoDataCollections.Include("Organization");

            if (!string.IsNullOrEmpty(sorting))
            {
                if (sorting == "title")
                    query = query.ToList().OrderBy(o => o.Title);
                else if (sorting == "title_desc")
                    query = query.ToList().OrderByDescending(o => o.Title);
                else if (sorting == "owner")
                    query = query.ToList().OrderBy(o => o.Organization.NameTranslated());
                else if (sorting == "owner_desc")
                    query = query.ToList().OrderByDescending(o => o.Organization.NameTranslated());

            }
            else
                query = query.ToList().OrderBy(o => o.Title).ToList();

            return View(query);

        }

        // GET: GeoDataCollection/Details/5
        public ActionResult Details(string itemname)
        {
            var model = _dbContext.GeoDataCollections.Include("Organization").Include("Responsible").Where(o => o.SeoName == itemname).FirstOrDefault();
            FixEmptyLabels(ref model);
            return View(model);
        }

        private void FixEmptyLabels(ref GeoDataCollection model)
        {
            if (string.IsNullOrEmpty(model.LinkInfoPageLabel))
                model.LinkInfoPageLabel = GeodataCollection.LinkInfoPage;

            if (string.IsNullOrEmpty(model.ProcessHistoryLabel))
                model.ProcessHistoryLabel = GeodataCollection.ProcessHistory;

            if (string.IsNullOrEmpty(model.LinkToRequirementsForDeliveryLabel))
                model.LinkToRequirementsForDeliveryLabel = GeodataCollection.LinkToRequirementsForDelivery;

            if (string.IsNullOrEmpty(model.LinkToMapSolutionLabel))
                model.LinkToMapSolutionLabel = GeodataCollection.LinkToMapSolution;

            if (string.IsNullOrEmpty(model.LinkLabel))
                model.LinkLabel = GeodataCollection.Link;

            if (string.IsNullOrEmpty(model.OtherWebInfoAboutMappingMethodologyLabel))
                model.OtherWebInfoAboutMappingMethodologyLabel = GeodataCollection.OtherWebInfoAboutMappingMethodology;

            if (string.IsNullOrEmpty(model.MappingRequirementsLinkLabel))
                model.MappingRequirementsLinkLabel = GeodataCollection.MappingRequirementsLink;
        }

        // GET: GeoDataCollection/Create
        public ActionResult Create()
        {
            ViewBag.ownerId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name");
            return View();
        }

        // POST: GeoDataCollection/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(GeoDataCollection collection, string ownerId)
        {
            try
            {
                if (!(User.IsInRole(GeonorgeRoles.MetadataAdmin) || User.IsInRole(GeonorgeRoles.MetadataEditor)))
                    new HttpUnauthorizedResult();

                var org = _dbContext.Organizations.Where(o => o.systemId.ToString() == ownerId).FirstOrDefault();
                collection.systemId = Guid.NewGuid();
                collection.Organization = org;
                collection.Owner = ClaimsPrincipal.Current.GetOrganizationName();
                collection.SeoName = RegisterUrls.MakeSeoFriendlyString(collection.Title);

                _dbContext.GeoDataCollections.Add(collection);

                _dbContext.SaveChanges();

                return Redirect("Edit?id=" + collection.systemId);
            }
            catch
            {
                return View();
            }
        }

        // GET: GeoDataCollection/Edit/5
        [Authorize]
        public ActionResult Edit(string id)
        {
            var collection = _dbContext.GeoDataCollections.Where(o => o.systemId.ToString() == id).FirstOrDefault();
            ViewBag.ownerId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", collection.Organization.systemId);
            ViewBag.responsibleId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", collection.Responsible != null ? collection.Responsible.systemId : collection.Organization.systemId);
            return View(collection);
        }

        // POST: GeoDataCollection/Edit/5
        [ValidateInput(false)]
        [Authorize]
        [HttpPost]
        public ActionResult Edit(string systemId, string ownerId, string responsibleId, GeoDataCollection collection, HttpPostedFileBase imagefile)
        {
            try
            {
                var sanitizer = new HtmlSanitizer();

                var geodataCollection = _dbContext.GeoDataCollections.Where(g => g.systemId.ToString() == systemId).FirstOrDefault();

                if (!(User.IsInRole(GeonorgeRoles.MetadataAdmin) || (User.IsInRole(GeonorgeRoles.MetadataEditor) && geodataCollection.Owner == ClaimsPrincipal.Current.GetOrganizationName()) ))
                    new HttpUnauthorizedResult();

                geodataCollection.Title = sanitizer.Sanitize(collection.Title);
                geodataCollection.SeoName = RegisterUrls.MakeSeoFriendlyString(sanitizer.Sanitize(collection.Title));
                geodataCollection.Link = sanitizer.Sanitize(collection.Link);
                geodataCollection.LinkLabel = sanitizer.Sanitize(collection.LinkLabel);
                geodataCollection.Purpose = sanitizer.Sanitize(collection.Purpose);

                geodataCollection.DatasetTitle = sanitizer.Sanitize(collection.DatasetTitle);
                geodataCollection.DatasetLink = sanitizer.Sanitize(collection.DatasetLink);
                geodataCollection.Mapper = sanitizer.Sanitize(collection.Mapper);
                geodataCollection.DataOwner = sanitizer.Sanitize(collection.DataOwner);
                geodataCollection.Distributor = sanitizer.Sanitize(collection.Distributor);
                geodataCollection.Methodology = sanitizer.Sanitize(collection.Methodology);
                geodataCollection.ProcessHistory = sanitizer.Sanitize(collection.ProcessHistory);
                geodataCollection.ProcessHistoryLabel = sanitizer.Sanitize(collection.ProcessHistoryLabel);
                geodataCollection.RegistrationRequirements = sanitizer.Sanitize(collection.RegistrationRequirements);
                geodataCollection.MappingRequirements = sanitizer.Sanitize(collection.MappingRequirements);
                geodataCollection.MappingRequirementsLink = sanitizer.Sanitize(collection.MappingRequirementsLink);
                geodataCollection.MappingRequirementsLinkLabel = sanitizer.Sanitize(collection.MappingRequirementsLinkLabel);
                geodataCollection.MethodologyDocumentLink = sanitizer.Sanitize(collection.MethodologyDocumentLink);
                geodataCollection.MethodologyLinkWebPage = sanitizer.Sanitize(collection.MethodologyLinkWebPage);
                geodataCollection.SupportSchemes = sanitizer.Sanitize(collection.SupportSchemes);

                geodataCollection.OtherOrganizationsInvolved = sanitizer.Sanitize(collection.OtherOrganizationsInvolved);
                geodataCollection.LinkToMapSolution = sanitizer.Sanitize(collection.LinkToMapSolution);
                geodataCollection.LinkToMapSolutionLabel = sanitizer.Sanitize(collection.LinkToMapSolutionLabel);
                geodataCollection.LinkInfoPage = sanitizer.Sanitize(collection.LinkInfoPage);
                geodataCollection.LinkInfoPageLabel = sanitizer.Sanitize(collection.LinkInfoPageLabel);
                geodataCollection.LinkOtherInfo = sanitizer.Sanitize(collection.LinkOtherInfo);
                geodataCollection.OtherInfo = sanitizer.Sanitize(collection.OtherInfo);
                geodataCollection.AidAndSubsidies = sanitizer.Sanitize(collection.AidAndSubsidies);
                geodataCollection.MethodForMappingShort = sanitizer.Sanitize(collection.MethodForMappingShort);
                geodataCollection.OtherWebInfoAboutMappingMethodology = sanitizer.Sanitize(collection.OtherWebInfoAboutMappingMethodology);
                geodataCollection.OtherWebInfoAboutMappingMethodologyLabel = sanitizer.Sanitize(collection.OtherWebInfoAboutMappingMethodologyLabel);
                geodataCollection.LinkToRequirementsForDelivery = sanitizer.Sanitize(collection.LinkToRequirementsForDelivery);
                geodataCollection.LinkToRequirementsForDeliveryLabel = sanitizer.Sanitize(collection.LinkToRequirementsForDeliveryLabel);
                geodataCollection.OrganizationInfo = sanitizer.Sanitize(collection.OrganizationInfo);
                geodataCollection.ContactEmail = sanitizer.Sanitize(collection.ContactEmail);

                var org = _dbContext.Organizations.Where(o => o.systemId.ToString() == ownerId).FirstOrDefault();
                geodataCollection.Organization = org;

                var responsible = _dbContext.Organizations.Where(o => o.systemId.ToString() == responsibleId).FirstOrDefault();
                if(responsible != null)
                    geodataCollection.Responsible = responsible;

                ViewBag.ownerId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", geodataCollection.Organization.systemId);
                ViewBag.responsibleId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", geodataCollection.Responsible != null ? geodataCollection.Responsible.systemId : geodataCollection.Organization.systemId);

                if (imagefile != null && !(imagefile.ContentType == "image/jpeg" || imagefile.ContentType == "image/gif" || imagefile.ContentType == "image/png"))
                {
                    ModelState.AddModelError("ImageFileName", "Bilde må være jpeg, gif eller png");
                    return View(collection);
                }

                if (imagefile != null && imagefile.ContentLength > 0)
                {
                    geodataCollection.ThumbnailFileName = SaveImageOptimizedToDisk(imagefile, geodataCollection.SeoName);

                    geodataCollection.ImageFileName = SaveImageToDisk(imagefile, geodataCollection.SeoName);
                }


                _dbContext.Entry(geodataCollection).State = EntityState.Modified;
                _dbContext.SaveChanges();

                ViewBag.ownerId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", geodataCollection.Organization.systemId);

                return RedirectToAction("Index");
            }
            catch
            {
                return View(collection);
            }
        }

        // GET: GeoDataCollection/Delete/5
        [Authorize]
        public ActionResult Delete(string id)
        {
            return View(_dbContext.GeoDataCollections.Where(o => o.systemId.ToString() == id).FirstOrDefault());
        }

        // POST: GeoDataCollection/Delete/5
        [Authorize]
        [HttpPost]
        public ActionResult Delete(string id, GeoDataCollection collection)
        {
            try
            {
                var geocollection = _dbContext.GeoDataCollections.Where(g => g.systemId.ToString() == id).FirstOrDefault();

                if (!(User.IsInRole(GeonorgeRoles.MetadataAdmin) || (User.IsInRole(GeonorgeRoles.MetadataEditor) && geocollection.Owner == ClaimsPrincipal.Current.GetOrganizationName())))
                    new HttpUnauthorizedResult();

                _dbContext.GeoDataCollections.Remove(geocollection);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private string SaveImageToDisk(HttpPostedFileBase file, string seo)
        {
            string filename = seo + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + "img/"), filename);
            file.SaveAs(path);
            return filename;
        }

        private string SaveImageOptimizedToDisk(HttpPostedFileBase file, string seo)
        {
            string filename = seo + "_thumb_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + "img/"), filename);

            ImageResizer.ImageJob newImage =
               new ImageResizer.ImageJob(file, path,
               new ImageResizer.Instructions("maxwidth=300;maxheight=1000;quality=75"));

            newImage.Build();

            return filename;
        }

    }
}
