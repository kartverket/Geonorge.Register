using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Services.Search;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using System.Web.Routing;
using PagedList;


namespace Kartverket.Register.Controllers
{
    public class SearchController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public ActionResult Index(SearchParameters parameters, int? page)
        {
            ViewBag.searchRegister = parameters.Register;
            SearchResult searchResult = _searchService.Search(parameters);

            SearchViewModel model = new SearchViewModel(parameters, searchResult);

            //int pageSize = 50;
            //int pageNumber = (page ?? 1);

            return View(model); //.ToPagedList(pageNumber, pageSize)
        }

        // GET: Registers/Details/5
        [Route("register/{registername}/{itemOwner}/")]
        public ActionResult DetailsFilter(string registername, string itemOwner, string sorting, int? page)  //(string registername, string documentownername, string sorting, int? page)
        {
            var queryResults = from o in db.Registers
                               where o.name == registername || o.seoname == registername
                               select o.systemId;

            var queryResultsOrganization = from o in db.Organizations
                                           where o.seoname == itemOwner 
                                           select o.systemId;

            Guid orgId = queryResultsOrganization.First();
            Kartverket.Register.Models.Organization organisasjon = db.Organizations.Find(orgId);
            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);
            
            ViewBag.page = page;
            ViewBag.SortOrder = sorting;
            ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
            ViewBag.register = register.seoname;
            ViewBag.owner = organisasjon.name;
            ViewBag.ownerLogo = organisasjon.logoFilename;
            ViewBag.ownerSEO = organisasjon.seoname;

                

            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }
    }
}
