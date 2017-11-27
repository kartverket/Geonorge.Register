﻿
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Kartverket.DOK.Service;
using Kartverket.Register.Models.Api;
using SearchParameters = Kartverket.Register.Models.SearchParameters;
using SearchResult = Kartverket.Register.Models.SearchResult;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.Translations;
using System.Globalization;
using System.Threading;
using System.Net.Http.Headers;

namespace Kartverket.Register.Controllers
{
    public class ApiRootController : ApiController
    {
        private RegisterDbContext db;

        private readonly ISearchService _searchService;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        
        public ApiRootController(RegisterDbContext dbContext, ISearchService searchService, IRegisterService registerService, IRegisterItemService registerItemService) 
        {
            _registerItemService = registerItemService;
            _searchService = searchService;
            _registerService = registerService;
            db = dbContext;
        }

        /// <summary>
        /// List top level registers. Use id in response to navigate.
        /// </summary>
        [Route("api/register")]
        [Route("api/register.{ext}")]
        [HttpGet]
        public IHttpActionResult GetRegisters()
        {
            SetLanguage(Request);
            var list = new List<Models.Api.Register>();
            List<Models.Register> registers = _registerService.GetRegisters();
            foreach (Models.Register register in registers)
            {
                list.Add(ConvertRegister(register));
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
        public IHttpActionResult GetRegisterByName(string registerName, [FromUri] FilterParameters filter = null)
        {
            SetLanguage(Request);
            var register = _registerService.GetRegisterByName(registerName);
            if(filter != null || registerName == "det-offentlige-kartgrunnlaget-kommunalt")
            {
                register = RegisterItems(register, filter);
            }

            return Ok(ConvertRegisterAndNextLevel(register, filter));
        }

        private Models.Register RegisterItems(Models.Register register, FilterParameters filter)
        {
            if (Search(filter)) register = _searchService.Search(register, filter.text);
            register = _registerService.FilterRegisterItems(register, filter);
            return register;
        }

        private static bool Search(FilterParameters filter)
        {
            return !string.IsNullOrWhiteSpace(filter.text);
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
            SetLanguage(Request);
            var it = _registerService.GetSubregisterByName(parentregister, register);
            return Ok(ConvertRegisterAndNextLevel(it));
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
            SetLanguage(Request);
            var it = _registerService.GetRegisterBySystemId(Guid.Parse(systemid));
            return Ok(ConvertRegisterAndNextLevel(it));
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
            SetLanguage(Request);
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
            SetLanguage(Request);
            var registerItem = _registerItemService.GetRegisterItem(null, register, item, version);
            return Ok(ConvertRegisterItem(registerItem));
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
            SetLanguage(Request);
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
                ConverteditemsByOwner.Add(ConvertRegisterItem(item));
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
            SetLanguage(Request);
            List<Models.RegisterItem> itemsByOwner = _registerItemService.GetRegisterItemsFromOrganization(parent, register, itemowner);
            List<Models.Api.Registeritem> ConverteditemsByOwner = new List<Models.Api.Registeritem>();

            foreach (Models.RegisterItem item in itemsByOwner)
            {
                ConverteditemsByOwner.Add(ConvertRegisterItem(item));
            }

            return Ok(ConverteditemsByOwner);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/metadata/synchronize")]
        [HttpGet]
        public IHttpActionResult SynchronizeDokMetadata()
        {
            new CoverageService(db).UpdateDatasetsWithCoverage();
            new DOK.Service.MetadataService().UpdateDatasetsWithMetadata();
            _registerService.UpdateDOKStatus();
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/metadata/synchronize/inspire-statusregister")]
        [HttpGet]
        public IHttpActionResult SynchronizeInspireStatusregister()
        {
            new InspireDatasetService(db).SynchronizeInspireDatasets();
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/metadata/synchronize/geodatalov-statusregister")]
        [HttpGet]
        public IHttpActionResult SynchronizeGeodatalovStatusregister()
        {
            new GeodatalovDatasetService(db).SynchronizeGeodatalovDatasets();
            return Ok();
        }

        /// <summary>
        /// DokCoverageMapping
        /// </summary>
        /// <param name="name">DokCoverageMapping</param>
        [Route("api/metadata/DokCoverageMapping")]
        [HttpGet]
        public IHttpActionResult DokCoverageMapping()
        {
            return Ok(DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping);
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
                    currentVersion = ConvertRegisterItem(v);

                    foreach (var ve in versjoner)
                    {
                        if (v.versionNumber != ve.versionNumber)
                        {
                            currentVersion.versions.Add(ConvertRegisterItem(ve));
                        }
                    }
                }
            }
            return currentVersion;
        }

        private Models.Api.Register ConvertRegister(Models.Register item, FilterParameters filter = null)
        {
            string registerId = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"]; //uri.Scheme + "://" + uri.Authority;
            if (registerId.Substring(registerId.Length - 1, 1) == "/") registerId = registerId.Remove(registerId.Length - 1);
            string selectedDOKMunicipality = "";
            if (filter != null && !string.IsNullOrEmpty(filter.municipality))
            {
                Services.RegisterItem.RegisterItemService regItemService = new Services.RegisterItem.RegisterItemService(new RegisterDbContext());
                Models.Organization org = regItemService.GetMunicipalityOrganizationByNr(filter.municipality);
                if (org != null)
                {
                    selectedDOKMunicipality = org.name;
                }
            }
            var tmp = new Models.Api.Register(item, registerId, selectedDOKMunicipality);
            return tmp;
        }



        private Models.Api.Register ConvertRegisterAndNextLevel(Models.Register item, FilterParameters filter = null)
        {
            var tmp = ConvertRegister(item, filter);
            tmp.containeditems = new List<Models.Api.Registeritem>();
            tmp.lang = CultureHelper.GetCurrentCulture();
            if (!item.items.Any())
            {
                foreach (var registerItem in item.RegisterItems)
                {
                    tmp.containeditems.Add(ConvertRegisterItem(registerItem, filter));
                }
            }

            if (item.items.Any())
            {
                foreach (var d in item.items)
                {
                    if (d.register.containedItemClass == "Document")
                    {
                        if (d.statusId != "Submitted" && d.versioning.currentVersion == d.systemId)
                        {
                            tmp.containeditems.Add(ConvertRegisterItem(d, filter));
                        }
                    }
                    else
                    {
                        tmp.containeditems.Add(ConvertRegisterItem(d, filter));
                    }
                }
            }
            else
            {
                tmp.containedSubRegisters = new List<Models.Api.Register>();
                var subregisters = _registerService.GetSubregistersOfRegister(item);
                if (subregisters != null)
                {
                    foreach (var reg in subregisters)
                    {
                        tmp.containedSubRegisters.Add(ConvertRegister(reg));
                    }
                }
            }

            return tmp;
        }

        private Registeritem ConvertRegisterItem(RegisterItem item, FilterParameters filter = null)
        {
            string registerId = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"];  //uri.Scheme + "://" + uri.Authority;
            if (registerId.Substring(registerId.Length - 1, 1) == "/") registerId = registerId.Remove(registerId.Length - 1);
            var tmp = new Registeritem(item,registerId, filter);
            return tmp;
        }
        private Registeritem ConvertRegisterItem(RegisterItemV2 item, FilterParameters filter = null)
        {
            string registerId = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"];  //uri.Scheme + "://" + uri.Authority;
            if (registerId.Substring(registerId.Length - 1, 1) == "/") registerId = registerId.Remove(registerId.Length - 1);
            var tmp = new Registeritem(item, registerId, filter);
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

        private List<Models.Api.Registeritem> GetVersions(RegisterItem rit)
        {
            List<Models.Api.Registeritem> versions = new List<Models.Api.Registeritem>();
            var queryResult = from ri in db.RegisterItems
                              where ri.versioningId == rit.versioningId
                              select ri;

            foreach (var item in queryResult.ToList())
            {
                versions.Add(ConvertRegisterItem(item));
            }
            versions.OrderBy(o => o.status);
            return versions;
        }

        private void SetLanguage(HttpRequestMessage request)
        {
            string language = Culture.NorwegianCode;

            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues("Accept-Language", out headerValues))
            {
                language = headerValues.FirstOrDefault();
                if (CultureHelper.IsNorwegian(language))
                    language = Culture.NorwegianCode;
                else
                    language = Culture.EnglishCode;
            }
            else
            {
                CookieHeaderValue cookie = request.Headers.GetCookies("_culture").FirstOrDefault();
                if (cookie != null && !string.IsNullOrEmpty(cookie["_culture"].Value))
                {
                    language = cookie["_culture"].Value;
                }
            }

            var culture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

        }
    }
}
