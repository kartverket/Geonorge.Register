using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kartverket.Register.Controllers
{
    public class ApiRootController : ApiController
    {
        private RegisterDbContext db = new RegisterDbContext();

        private readonly ISearchService _searchService;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;

        public ApiRootController(ISearchService searchService, IRegisterService registerService, IRegisterItemService registerItemService)
        {
            _registerItemService = registerItemService;
            _searchService = searchService;
            _registerService = registerService;
        }

        /// <summary>
        /// List top level registers. Use id in response to navigate.
        /// </summary>
        [Route("api/register")]
        [Route("api/register.{ext}")]
        [HttpGet]
        public IHttpActionResult GetRegisters()
        {
            var list = new List<Models.Api.Register>();
            List<Models.Register> registers = _registerService.GetRegisters();
            foreach (Models.Register register in registers)
            {
                list.Add(ConvertRegister(register, Request.RequestUri));
            }
            return Ok(list);
        }


        /// <summary>
        /// Gets register by name
        /// </summary>
        /// <param name="registerName">The search engine optimized name of the register</param>
        [Route("api/register/{registerName}.{ext}")]
        [Route("api/register/{registerName}")]
        [HttpGet]
        public IHttpActionResult GetRegisterByName(string registerName)
        {
            var register = _registerService.GetRegisterByName(registerName);
            return Ok(ConvertRegisterAndNextLevel(register, Request.RequestUri));
        }


        /// <summary>
        /// Gets subregister by name
        /// </summary>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="parentregister">The search engine optimized name of the parentregister</param>
        [Route("api/subregister/{parentregister}/{parentregisterOwner}/{register}.{ext}")]
        [Route("api/subregister/{parentregister}/{parentregisterOwner}/{register}")]
        [HttpGet]
        public IHttpActionResult GetSubregisterByName(string parentregister, string register)
        {
            var it = _registerService.GetSubregisterByName(parentregister, register);
            return Ok(ConvertRegisterAndNextLevel(it, Request.RequestUri));
        }


        /// <summary>
        /// Gets codelist by systemid
        /// </summary>
        /// <param name="systemid">The uniqueidentifier for the register</param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/kodelister/{systemid}")]
        [HttpGet]
        public IHttpActionResult GetRegisterBySystemId(string systemid)
        {
            var it = _registerService.GetRegisterBySystemId(Guid.Parse(systemid));
            return Ok(ConvertRegisterAndNextLevel(it, Request.RequestUri));
        }


        /// <summary>
        /// Gets current and historical versions of register item by register- organization- and registeritem-name 
        /// </summary>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="itemowner">The search engine optimized name of the register item owner</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        [Route("api/register/{register}/{itemowner}/{item}.{ext}")]
        [Route("api/register/{register}/{itemowner}/{item}")]
        [Route("api/register/versjoner/{register}/{itemowner}/{item}.{ext}")]
        [Route("api/register/versjoner/{register}/{itemowner}/{item}")]
        [HttpGet]
        public IHttpActionResult GetRegisterItemByName(string register, string itemowner, string item)
        {
            Models.Api.Registeritem currentVersion = ConvertCurrentAndVersions(null, register, item);
            return Ok(currentVersion);
        }


        /// <summary>
        /// Gets register item by register- organization- registeritem-name  and version-id
        /// </summary>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        /// <param name="version">The version id of the registeritem</param>
        [Route("api/register/versjoner/{register}/{itemowner}/{item}/{version}/no.{ext}")]
        [Route("api/register/versjoner/{register}/{itemowner}/{item}/{version}/no")]
        [HttpGet]
        public IHttpActionResult GetRegisterItemByVersionNr(string register, string item, int version)
        {
            var registerItem = _registerItemService.GetRegisterItemByVersionNr(register, item, version);
            return Ok(ConvertRegisterItemDetails(registerItem.register, registerItem, Request.RequestUri));
        }


        /// <summary>
        /// Gets register item by parent-register, register- organization- and registeritem-name 
        /// </summary>
        /// <param name="parentregister">The search engine optimized name of the parent register</param>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        [Route("api/subregister/{parentregister}/{registerowner}/{register}/{itemowner}/{item}")]
        [Route("api/subregister/{parentregister}/{registerowner}/{register}/{itemowner}/{item}.{ext}")]
        [HttpGet]
        public IHttpActionResult GetSubregisterItemByName(string parentregister, string register, string item)
        {
            Models.Api.Registeritem currentVersion = ConvertCurrentAndVersions(parentregister, register, item);
            return Ok(currentVersion);
        }

        /// <summary>
        /// List items for specific organization 
        /// </summary>
        /// <param name="name">The name of the organization</param>
        [Route("api/register/organisasjon/{name}")]
        [HttpGet]
        public IHttpActionResult SearchByOrganizationName(string name)
        {
            List<Models.Api.Item> resultat = new List<Models.Api.Item>();
            SearchParameters parameters = new SearchParameters();
            parameters.Text = name;
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            SearchResult searchResult = _searchService.Search(parameters);
            foreach (var it in searchResult.Items)
            {
                resultat.Add(Convert(it));
            }
            return Ok(resultat);
        }

        /// <summary>
        /// List items for specific organization 
        /// </summary>
        /// <param name="register">The name of the register</param>
        /// <param name="itemowner">The name of the organization</param>
        [Route("api/register/{register}/{itemowner}.{ext}")]
        [Route("api/register/{register}/{itemowner}")]
        [HttpGet]
        public IHttpActionResult GetRegisterItemsByOrganization(string register, string itemowner)
        {
            List<Models.RegisterItem> itemsByOwner = _registerItemService.GetRegisterItemsFromOrganization(null, register, itemowner);
            List<Models.Api.Registeritem> ConverteditemsByOwner = new List<Models.Api.Registeritem>();

            foreach (Models.RegisterItem item in itemsByOwner)
            {
                ConverteditemsByOwner.Add(ConvertRegisterItemDetails(item.register, item, Request.RequestUri));
            }

            return Ok(ConverteditemsByOwner);
        }

        /// <summary>
        /// List items for specific organization in subregister
        /// </summary>
        /// <param name="parent">The name of the parentregister</param>
        /// <param name="register">The name of the register</param>
        /// <param name="itemowner">The name of the organization</param>
        [Route("api/subregister/{parent}/{registerOwner}/{register}/{itemowner}.{ext}")]
        [Route("api/subregister/{parent}/{registerOwner}/{register}/{itemowner}")]
        [HttpGet]
        public IHttpActionResult GetRegisterItemsByOrganization(string parent, string register, string itemowner)
        {
            List<Models.RegisterItem> itemsByOwner = _registerItemService.GetRegisterItemsFromOrganization(parent, register, itemowner);
            List<Models.Api.Registeritem> ConverteditemsByOwner = new List<Models.Api.Registeritem>();

            foreach (Models.RegisterItem item in itemsByOwner)
            {
                ConverteditemsByOwner.Add(ConvertRegisterItemDetails(item.register, item, Request.RequestUri));
            }

            return Ok(ConverteditemsByOwner);
        }





        // **** HJELPEMETODER ****

        private Models.Api.Registeritem ConvertCurrentAndVersions(string parent, string register, string item)
        {
            Models.Api.Registeritem currentVersion = null;
            var versjoner = _registerItemService.GetAllVersionsOfItem(parent, register, item);
            foreach (var v in versjoner)
            {
                if (v.versioning.currentVersion == v.systemId)
                {
                    currentVersion = ConvertRegisterItemDetails(v.register, v, Request.RequestUri);

                    foreach (var ve in versjoner)
                    {
                        if (v.versionNumber != ve.versionNumber)
                        {
                            currentVersion.versions.Add(ConvertRegisterItemDetails(ve.register, ve, Request.RequestUri));
                        }
                    }
                }
            }
            return currentVersion;
        }

        private Models.Api.Register ConvertRegister(Models.Register item, Uri uri)
        {
            string registerId = uri.Scheme + "://" + uri.Authority;
            if (item.parentRegister != null)
            {
                registerId = registerId + "/subregister/" + item.parentRegister.seoname + "/" + item.parentRegister.owner.seoname + "/" + item.seoname;
            }
            else
            {
                registerId = registerId + "/register/" + item.seoname;
            }
            var tmp = new Models.Api.Register
            {
                label = item.name,
                id = registerId,
                contentsummary = item.description,
                lastUpdated = item.modified,
                targetNamespace = item.targetNamespace,
                containedItemClass = item.containedItemClass
            };
            if (item.owner != null) tmp.owner = item.owner.seoname;
            if (item.manager != null) tmp.manager = item.manager.seoname;

            return tmp;
        }


        private Models.Api.Register ConvertRegisterAndNextLevel(Models.Register item, Uri uri)
        {
            var tmp = ConvertRegister(item, uri);
            if (item.items.Count() > 0 && item.items != null)
            {
                tmp.containeditems = new List<Models.Api.Registeritem>();

                foreach (var d in item.items)
                {
                    if (d.register.containedItemClass == "Document")
                    {
                        if (d.statusId != "Submitted" && d.versioning.currentVersion == d.systemId)
                        {
                            tmp.containeditems.Add(ConvertRegisterItem(item, d, uri));
                        }
                    }
                    else
                    {
                        tmp.containeditems.Add(ConvertRegisterItem(item, d, uri));
                    }
                }
            }
            else
            {
                tmp.containedSubRegisters = new List<Models.Api.Register>();
                List<Models.Register> subregisters = _registerService.GetSubregistersOfRegister(item);
                if (subregisters != null)
                {
                    foreach (var reg in subregisters)
                    {
                        tmp.containedSubRegisters.Add(ConvertRegister(reg, uri));
                    }
                }
            }

            return tmp;
        }

        private Models.Api.Registeritem ConvertRegisterItem(Models.Register reg, Models.RegisterItem item, Uri uri)
        {
            string registerId = uri.Scheme + "://" + uri.Authority;
            var tmp = new Models.Api.Registeritem();
            tmp.label = item.name;

            tmp.lastUpdated = item.modified;
            tmp.itemclass = item.register.containedItemClass;
            if (item.submitter != null) tmp.owner = item.submitter.name;
            if (item.status != null) tmp.status = item.status.description;
            if (item.description != null) tmp.description = item.description;
            if (item.versionName != null) tmp.versionName = item.description;
            if (item.versionNumber != null) tmp.versionNumber = item.versionNumber;
            if (reg.parentRegisterId != null)
            {
                tmp.id = registerId + "/subregister/" + reg.parentRegister.seoname + "/" + reg.parentRegister.owner.seoname + "/" + reg.seoname + "/" + item.submitter.seoname + "/" + item.seoname;
            }
            else
            {
                tmp.id = registerId + "/register/" + reg.seoname + "/" + item.submitter.seoname + "/" + item.seoname;
            }
            tmp.versionName = item.versionName;

            if (item is EPSG)
            {
                var d = (EPSG)item;
                tmp.documentreference = "http://www.opengis.net/def/crs/EPSG/0/" + d.epsgcode;
                tmp.inspireRequirement = d.inspireRequirement.description;
                tmp.nationalRequirement = d.nationalRequirement.description;
                tmp.nationalSeasRequirement = d.nationalSeasRequirement != null ? d.nationalSeasRequirement.description : "";
                tmp.horizontalReferenceSystem = d.horizontalReferenceSystem;
                tmp.verticalReferenceSystem = d.verticalReferenceSystem;
                tmp.dimension = d.dimension != null ? d.dimension.description : "";
            }

            if (item is CodelistValue)
            {
                var c = (CodelistValue)item;
                tmp.codevalue = c.value;

                if (c.broaderItemId != null)
                {
                    if (c.register.parentRegisterId != null)
                    {
                        tmp.broader = registerId + "/subregister/" + c.broaderItem.register.parentRegister.seoname + "/" + c.broaderItem.register.parentRegister.owner.seoname + "/" + c.broaderItem.register.seoname + "/" + c.broaderItem.submitter.seoname + "/" + c.broaderItem.seoname;
                    }
                    else
                    {
                        tmp.broader = registerId + "/register/" + c.broaderItem.register.seoname + "/" + c.broaderItem.submitter.seoname + "/" + c.broaderItem.seoname;
                    }
                }
            }

            if (item is Document)
            {
                var d = (Document)item;
                tmp.owner = d.documentowner.name;
                tmp.documentreference = d.documentUrl;
            }

            if (item is Dataset)
            {
                var d = (Dataset)item;
                tmp.owner = d.datasetowner.name;
                tmp.theme = d.theme.description;
                tmp.dokStatus = d.dokStatus.description;
            }
            if (item is NameSpace)
            {
                var n = (NameSpace)item;
                tmp.serviceUrl = n.serviceUrl;
            }
            return tmp;
        }

        private Models.Api.Registeritem ConvertRegisterItemDetails(Models.Register reg, Models.RegisterItem item, Uri uri)
        {
            string registerId = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"]; // uri.Scheme + "://" + uri.Authority;
            var tmp = new Models.Api.Registeritem();
            tmp.label = item.name;

            if (reg.parentRegisterId != null)
            {
                tmp.id = registerId + "subregister/" + reg.parentRegister.seoname + "/" + reg.parentRegister.owner.seoname + "/" + reg.seoname + "/" + item.submitter.seoname + "/" + item.seoname;
            }
            else
            {
                tmp.id = registerId + "register/" + reg.seoname + "/" + item.submitter.seoname + "/" + item.seoname;
            }

            if (item.status != null) tmp.status = item.status.description;
            if (item.description != null) tmp.description = item.description;
            if (item.submitter != null) tmp.owner = item.submitter.name;
            if (item.versionName != null) tmp.versionName = item.description;
            if (item.versionNumber != null) tmp.versionNumber = item.versionNumber;
            tmp.lastUpdated = item.modified;
            tmp.dateSubmitted = item.dateSubmitted;
            tmp.dateAccepted = item.dateAccepted.GetValueOrDefault();

            if (item is Document)
            {
                tmp.itemclass = "Document";
                var d = (Document)item;
                tmp.documentreference = d.documentUrl;
            }
            else if (item is Dataset)
            {
                tmp.itemclass = "Dataset";
            }
            else if (item is Organization)
            {
                tmp.itemclass = "Organization";
                var d = (Organization)item;
                tmp.logo = d.logoFilename;
            }
            else if (item is NameSpace)
            {
                tmp.itemclass = "NameSpace";
                var d = (NameSpace)item;
                tmp.serviceUrl = d.serviceUrl;
            }
            else if (item is EPSG)
            {
                tmp.itemclass = "EPSG";
                var d = (EPSG)item;
                tmp.documentreference = "http://www.opengis.net/def/crs/EPSG/0/" + d.epsgcode;
                tmp.inspireRequirement = d.inspireRequirement.description;
                tmp.nationalRequirement = d.nationalRequirement.description;
                tmp.nationalSeasRequirement = d.nationalSeasRequirement != null ? d.nationalSeasRequirement.description : "";
                tmp.horizontalReferenceSystem = d.horizontalReferenceSystem;
                tmp.verticalReferenceSystem = d.verticalReferenceSystem;
                tmp.dimension = d.dimension != null ? d.dimension.description : "";
            }
            else if (item is CodelistValue)
            {
                tmp.itemclass = "CodelistValue";
                var c = (CodelistValue)item;
                tmp.codevalue = c.value;
                if (c.broaderItemId != null)
                {
                    if (c.register.parentRegisterId != null)
                    {
                        tmp.broader = registerId + "/subregister/" + c.broaderItem.register.parentRegister.seoname + "/" + c.broaderItem.register.parentRegister.owner.seoname + "/" + c.broaderItem.register.seoname + "/" + c.broaderItem.submitter.seoname + "/" + c.broaderItem.seoname;
                    }
                    else
                    {
                        tmp.broader = registerId + "/register/" + c.broaderItem.register.seoname + "/" + c.broaderItem.submitter.seoname + "/" + c.broaderItem.seoname;
                    }
                }
            }
            else tmp.itemclass = "RegisterItem";


            return tmp;
        }

        private Models.Api.Item Convert(SearchResultItem searchitem)
        {
            var tmp = new Models.Api.Item
            {
                name = searchitem.RegisterItemName,
                description = searchitem.RegisterItemDescription,
                status = searchitem.RegisterItemStatus,
                updated = searchitem.RegisterItemUpdated,
                author = searchitem.DocumentOwner,
                showUrl = searchitem.RegisteItemUrl,
                editUrl = null

            };

            return tmp;
        }

        private RegisterItem GetCurrentVersion(string parent, string register, string item)
        {
            List<RegisterItem> itemList = new List<RegisterItem>();
            var queryresult = from d in db.RegisterItems
                              where d.register.seoname == register
                              && (d.register.parentRegister == null || d.register.parentRegister.seoname == parent)
                              && d.seoname == item
                              select d;


            foreach (var ri in queryresult.ToList())
            {
                if (ri.statusId != "Submitted")
                {
                    itemList.Add(ri);
                }
            }
            if (itemList.Count() > 1)
            {
                foreach (RegisterItem version in itemList)
                {
                    if (version.systemId == version.versioning.currentVersion)
                    {
                        return version;
                    }
                }
            }
            return queryresult.FirstOrDefault();
        }

        private Models.Register GetRegister(string parent, string register)
        {
            var queryresult = from d in db.Registers
                              where d.seoname == register
                              && (d.parentRegister == null || d.parentRegister.seoname == parent)
                              select d;

            return queryresult.FirstOrDefault();
        }

        private Models.RegisterItem GetVersion(string parent, string register, string item, int version)
        {
            var queryresult = from d in db.RegisterItems
                              where d.seoname == item
                              && d.register.seoname == register
                              && (d.register.parentRegister == null || d.register.parentRegister.seoname == parent)
                              && d.versionNumber == version
                              select d;

            return queryresult.FirstOrDefault();
        }

        private List<Models.Api.Registeritem> GetVersions(RegisterItem rit, Uri uri)
        {
            List<Models.Api.Registeritem> versions = new List<Models.Api.Registeritem>();
            var queryResult = from ri in db.RegisterItems
                              where ri.versioningId == rit.versioningId
                              select ri;

            foreach (var item in queryResult.ToList())
            {
                versions.Add(ConvertRegisterItem(item.register, item, uri));
            }
            versions.OrderBy(o => o.status);
            return versions;
        }
    }
}
