using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
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

        public ApiRootController(ISearchService searchService, IRegisterService registerService)
        {
            _searchService = searchService;
            _registerService = registerService;
        }

        /// <summary>
        /// List top level registers. Use id in response to navigate.
        /// </summary>
        [Route("api/register")]
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
        /// <param name="register">The search engine optimized name of the register</param>
        [Route("api/register/{register}.{ext}")]
        [Route("api/register/{register}")]
        [HttpGet]
        public IHttpActionResult GetRegisterByName(string register)
        {
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            var it = GetRegister(null, register);                                  
            return Ok(ConvertRegisterAndNextLevel(it, urlHelper));
        }

        /// <summary>
        /// Gets subregister by name
        /// </summary>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="parentregisterOwner">The search engine optimized name of the register owner</param>
        /// <param name="parentregister">The search engine optimized name of the parentregister</param>
        [Route("api/subregister/{parentregister}/{parentregisterOwner}/{register}.{ext}")]
        [Route("api/subregister/{parentregister}/{parentregisterOwner}/{register}")]
        [HttpGet]
        public IHttpActionResult GetSubregisterByName(string parentregister, string register)
        {
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            var it = GetRegister(parentregister, register);
            return Ok(ConvertRegisterAndNextLevel(it, urlHelper));
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
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            var it = db.Registers.Where(w => w.systemId == new Guid(systemid)).FirstOrDefault();
            return Ok(ConvertRegisterAndNextLevel(it, urlHelper));
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
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            var it = GetRegister(null, register); 
            RegisterItem rit = GetCurrentVersion(null, register, item);
            var versjoner = GetVersions(rit, urlHelper);
            
            return Ok(versjoner);            
        }


        /// <summary>
        /// Gets register item by register- organization- registeritem-name  and version-id
        /// </summary>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="itemowner">The search engine optimized name of the register item owner</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        /// <param name="version">The version id of the registeritem</param>
        [Route("api/register/versjoner/{register}/{itemowner}/{item}/{version}/no.{ext}")]
        [Route("api/register/versjoner/{register}/{itemowner}/{item}/{version}/no")]
        [HttpGet]
        public IHttpActionResult GetRegisterItemByNameAndVersion(string register, string itemowner, string item, int version)
        {
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            var it = GetRegister(null, register);
            var rit = GetVersion(null, register, item, version);
            return Ok(ConvertRegisterItemDetails(it, rit, urlHelper));
        }


        /// <summary>
        /// Gets register item by parent-register, register- organization- and registeritem-name 
        /// </summary>
        /// <param name="parentregister">The search engine optimized name of the parent register</param>
        /// <param name="registerowner">The search engine optimized name of the register owner</param>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="itemowner">The search engine optimized name of the register item owner</param>
        /// <param name="items">The search engine optimized name of the register item</param>
        [Route("api/subregister/{parentregister}/{registerowner}/{register}/{itemowner}/{item}")]
        [Route("api/subregister/{parentregister}/{registerowner}/{register}/{itemowner}/{item}.{ext}")]
        [HttpGet]
        public IHttpActionResult GetSubregisterItemByName(string parentregister, string register, string itemowner, string item)
        {
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            var it = GetRegister(parentregister, register);
            var rit = GetCurrentVersion(parentregister, register, item);
            return Ok(ConvertRegisterItemDetails(it, rit, urlHelper));
        }

        /// <summary>
        /// List items for specific organization 
        /// </summary>
        /// <param name="name">The name of the organization</param>
        [Route("api/register/organisasjon/{name}")]
        [HttpGet]
        public IHttpActionResult SearchByOrganizationName(string name)
        {
            List<Kartverket.Register.Models.Api.Item> resultat = new List<Models.Api.Item>();

            SearchParameters parameters = new SearchParameters();
            parameters.Text = name;
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            SearchResult searchResult = _searchService.Search(parameters);
            foreach (var it in searchResult.Items)
            {
                resultat.Add(Convert(it, urlHelper));
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
        public IHttpActionResult GetRegisterItemsFromOrganization(string register, string itemowner)
        {
            List<Models.Api.Registeritem> itemsFromOwner = new List<Models.Api.Registeritem>();
            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            Models.Register it = GetRegister(null, register);
            foreach (var item in it.items)
            {
                if (item.submitter.seoname == itemowner)
                {
                    itemsFromOwner.Add(ConvertRegisterItemDetails(it, item, urlHelper));
                }
            }
            return Ok(itemsFromOwner);
        }


        private Models.Api.Register ConvertRegister(Models.Register item, Uri uri)
        {
            string registerId = uri.Scheme + "://" + uri.Authority;
            if (item.parentRegisterId != null)
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
                targetNamespace = item.targetNamespace
            };
            if (item.owner != null) tmp.owner = item.owner.seoname;
            if (item.manager != null) tmp.manager = item.manager.seoname;

            return tmp;

        }

        private Models.Api.Register ConvertRegister(Models.Register item, System.Web.Mvc.UrlHelper urlHelper)
        {
            string registerId = "";
            if (item.parentRegisterId != null)
            {
                registerId = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/subregister/" + item.parentRegister.seoname + "/" + item.parentRegister.owner.seoname + "/" + item.seoname;
            }
            else
            {
                registerId = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/register/" + item.seoname;
            }
            var tmp = new Models.Api.Register
            {
                label = item.name,
                id = registerId,
                contentsummary = item.description,
                lastUpdated = item.modified,
                targetNamespace = item.targetNamespace
            };
            if (item.owner != null) tmp.owner = item.owner.seoname;
            if (item.manager != null) tmp.manager = item.manager.seoname;

            return tmp;
        }

        private Models.Api.Register ConvertRegisterAndNextLevel(Models.Register item, System.Web.Mvc.UrlHelper urlHelper)
        {
            var tmp = ConvertRegister(item, urlHelper);
            if (item.items != null) tmp.containeditems = new List<Models.Api.Registeritem>();
            tmp.containedSubRegisters = new List<Models.Api.Register>();
            var queryResultParentRegister = from r in db.Registers
                                            where r.parentRegisterId == item.systemId
                                            select r;

            foreach (var reg in queryResultParentRegister)
            {
                tmp.containedSubRegisters.Add(ConvertRegister(reg, urlHelper));
            }

            foreach (var d in item.items)
            {
                tmp.containeditems.Add(ConvertRegisterItem(item, d, urlHelper));
            }

            return tmp;
        }

        private Models.Api.Registeritem ConvertRegisterItem(Models.Register reg, Models.RegisterItem item, System.Web.Mvc.UrlHelper urlHelper)
        {
            var tmp = new Models.Api.Registeritem();
            tmp.label = item.name;
            
            tmp.lastUpdated = item.modified;
            if (item.submitter != null) tmp.owner = item.submitter.name;
            if (item.status != null) tmp.status = item.status.description;
            if (item.description != null) tmp.description = item.description;
            if (reg.parentRegisterId != null)
            {
                tmp.id = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/subregister/" + reg.parentRegister.seoname + "/" + reg.parentRegister.owner.seoname + "/" + reg.seoname + "/" + item.submitter.seoname + "/" + item.seoname;
            }
            else
            {
                tmp.id = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/register/" + reg.seoname + "/" + item.submitter.seoname + "/" + item.seoname;
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
                        tmp.broader = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/subregister/" + c.broaderItem.register.parentRegister.seoname + "/" + c.broaderItem.register.parentRegister.owner.seoname + "/" + c.broaderItem.register.seoname + "/" + c.broaderItem.submitter.seoname + "/" + c.broaderItem.seoname;
                    }
                    else
                    {
                        tmp.broader = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/register/" + c.broaderItem.register.seoname + "/" + c.broaderItem.submitter.seoname + "/" + c.broaderItem.seoname;
                    }
                }
            }

            if (item is Document)
            {
                var d = (Document)item;
                tmp.owner = d.documentowner.name;
            }

            if (item is Dataset)
            {
                var d = (Dataset)item;
                tmp.owner = d.datasetowner.name;
            }
            if (item is NameSpace)
            {
                var n = (NameSpace)item;
                tmp.serviceUrl = n.serviceUrl;
            }
            return tmp;
        }

        private Models.Api.Registeritem ConvertRegisterItemDetails(Models.Register reg, Models.RegisterItem item, System.Web.Mvc.UrlHelper urlHelper)
        {
            var tmp = new Models.Api.Registeritem();
            tmp.label = item.name;

            if (reg.parentRegisterId != null)
            {
                tmp.id = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/subregister/" + reg.parentRegister.seoname + "/" + reg.parentRegister.owner.seoname + "/" + reg.seoname + "/" + item.submitter.seoname + "/" + item.seoname;
            }
            else
            {
                tmp.id = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/register/" + reg.seoname + "/" + item.submitter.seoname + "/" + item.seoname;
            }


            if (item.status != null) tmp.status = item.status.description;
            if (item.description != null) tmp.description = item.description;
            if (item.submitter != null) tmp.owner = item.submitter.name;
            tmp.lastUpdated = item.modified;

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
                var c = (CodelistValue)item;
                tmp.codevalue = c.value;
                if (c.broaderItemId != null)
                {
                    if (c.register.parentRegisterId != null)
                    {
                        tmp.broader = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/subregister/" + c.broaderItem.register.parentRegister.seoname + "/" + c.broaderItem.register.parentRegister.owner.seoname + "/" + c.broaderItem.register.seoname + "/" + c.broaderItem.submitter.seoname + "/" + c.broaderItem.seoname;
                    }
                    else
                    {
                        tmp.broader = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/register/" + c.broaderItem.register.seoname + "/" + c.broaderItem.submitter.seoname + "/" + c.broaderItem.seoname;
                    }
                }
            }
            else tmp.itemclass = "RegisterItem";


            return tmp;
        }

        private Models.Api.Item Convert(SearchResultItem searchitem, System.Web.Mvc.UrlHelper urlHelper)
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
                if (ri.statusId != "Submitted" && ri.statusId != "NotAccepted") 
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

        private List<Models.Api.Registeritem> GetVersions(RegisterItem rit, System.Web.Mvc.UrlHelper urlHelper)
        {
            List<Models.Api.Registeritem> versions = new List<Models.Api.Registeritem>();
            var queryResult = from ri in db.RegisterItems
                              where ri.versioningId == rit.versioningId
                              select ri;

            foreach (var item in queryResult.ToList())
            {
                versions.Add(ConvertRegisterItem(item.register, item, urlHelper));
            }
            versions.OrderBy(o => o.status);
            return versions;
        }
    }
}
