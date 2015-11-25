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

            return View(model); //.ToPagedList(pageNumber, pageSize)
        }  
    }
}
