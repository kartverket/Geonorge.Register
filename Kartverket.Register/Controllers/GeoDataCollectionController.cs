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
        [Authorize]
        [HttpPost]
        public ActionResult Edit(string systemId, string ownerId, string responsibleId, GeoDataCollection collection, HttpPostedFileBase imagefile)
        {
            try
            {
                var geodataCollection = _dbContext.GeoDataCollections.Where(g => g.systemId.ToString() == systemId).FirstOrDefault();

                if (!(User.IsInRole(GeonorgeRoles.MetadataAdmin) || (User.IsInRole(GeonorgeRoles.MetadataEditor) && geodataCollection.Owner == ClaimsPrincipal.Current.GetOrganizationName()) ))
                    new HttpUnauthorizedResult();

                geodataCollection.Title = collection.Title;
                geodataCollection.SeoName = RegisterUrls.MakeSeoFriendlyString(collection.Title);
                geodataCollection.Link = collection.Link;
                geodataCollection.LinkLabel = collection.LinkLabel;
                geodataCollection.Purpose = collection.Purpose;

                geodataCollection.DatasetTitle = collection.DatasetTitle;
                geodataCollection.DatasetLink = collection.DatasetLink;
                geodataCollection.Mapper = collection.Mapper;
                geodataCollection.DataOwner = collection.DataOwner;
                geodataCollection.Distributor = collection.Distributor;
                geodataCollection.Methodology = collection.Methodology;
                geodataCollection.ProcessHistory = collection.ProcessHistory;
                geodataCollection.ProcessHistoryLabel = collection.ProcessHistoryLabel;
                geodataCollection.RegistrationRequirements = collection.RegistrationRequirements;
                geodataCollection.MappingRequirements = collection.MappingRequirements;
                geodataCollection.MappingRequirementsLink = collection.MappingRequirementsLink;
                geodataCollection.MappingRequirementsLinkLabel = collection.MappingRequirementsLinkLabel;
                geodataCollection.MethodologyDocumentLink = collection.MethodologyDocumentLink;
                geodataCollection.MethodologyLinkWebPage = collection.MethodologyLinkWebPage;
                geodataCollection.SupportSchemes = collection.SupportSchemes;

                geodataCollection.OtherOrganizationsInvolved = collection.OtherOrganizationsInvolved;
                geodataCollection.LinkToMapSolution = collection.LinkToMapSolution;
                geodataCollection.LinkToMapSolutionLabel = collection.LinkToMapSolutionLabel;
                geodataCollection.LinkInfoPage = collection.LinkInfoPage;
                geodataCollection.LinkInfoPageLabel = collection.LinkInfoPageLabel;
                geodataCollection.LinkOtherInfo = collection.LinkOtherInfo;
                geodataCollection.OtherInfo = collection.OtherInfo;
                geodataCollection.AidAndSubsidies = collection.AidAndSubsidies;
                geodataCollection.MethodForMappingShort = collection.MethodForMappingShort;
                geodataCollection.OtherWebInfoAboutMappingMethodology = collection.OtherWebInfoAboutMappingMethodology;
                geodataCollection.OtherWebInfoAboutMappingMethodologyLabel = collection.OtherWebInfoAboutMappingMethodologyLabel;
                geodataCollection.LinkToRequirementsForDelivery = collection.LinkToRequirementsForDelivery;
                geodataCollection.LinkToRequirementsForDeliveryLabel = collection.LinkToRequirementsForDeliveryLabel;
                geodataCollection.OrganizationInfo = collection.OrganizationInfo;
                geodataCollection.ContactEmail = collection.ContactEmail;

                var org = _dbContext.Organizations.Where(o => o.systemId.ToString() == ownerId).FirstOrDefault();
                geodataCollection.Organization = org;

                var responsible = _dbContext.Organizations.Where(o => o.systemId.ToString() == responsibleId).FirstOrDefault();
                if(responsible != null)
                    geodataCollection.Responsible = responsible;

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
                return View();
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
