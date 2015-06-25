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
    [HandleError]
    public class SearchController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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



    //    // GET: Registers/Details/5
    //    [Route("register/{registername}/{itemOwner}/")]
    //    public ActionResult DetailsFilter(SearchParameters parameters, string registername, string itemOwner, string sorting, int? page)
    //    {
    //        var queryresults = from r in db.Organizations
    //                           where r.seoname == itemOwner
    //                           select r.name;

    //        ViewBag.ownerName = queryresults.FirstOrDefault();

    //        var queryresultsRegister = from r in db.Registers
    //                           where r.seoname == registername
    //                           select r.name;

    //        ViewBag.RegisterName = queryresultsRegister.FirstOrDefault();
            
    //        if (sorting != null)
    //        {
    //            parameters.OrderBy = sorting;
    //        }
    //        else {
    //            sorting = parameters.OrderBy;
    //        }
    //        parameters.Owner = itemOwner;
    //        parameters.Register = registername;

    //        ViewBag.searchRegister = parameters.Register;
    //        ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description", parameters.OrderBy);
    //        ViewBag.registerSeo = registername;
    //        FilterResult filterResult = _searchService.Filter(parameters);
    //        FilteringViewModel model = new FilteringViewModel(parameters, filterResult);

    //        return View(model);
    //    }

    //    protected override void OnException(ExceptionContext filterContext)
    //    {
    //        Log.Error("Error", filterContext.Exception);
    //    }
    }
}
