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

        public ActionResult Index(SearchParameters parameters)
        {
            SearchResult searchResult = _searchService.Search(parameters);

            SearchViewModel model = new SearchViewModel(parameters, searchResult);

            return View(model);
        }


        [Route("register/{registername}/{documentownername}/")]
        public ActionResult DetailsFilterDocument(string registername, string documentownername, string sorting, int? page)
        {

            var queryResultsTest = from d in db.Documents
                                  where d.register.seoname == registername && d.documentowner.seoname == documentownername
                                  select d.systemId;

            List<Guid> documentsId = queryResultsTest.ToList();
            List<Document> documents = new List<Document>();

            ViewBag.page = page;
            ViewBag.sortOrder = sorting;
            ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
            
            foreach (Guid item in documentsId)
            {
                Kartverket.Register.Models.Document document = db.Documents.Find(item);
                if (document.documentowner.seoname == documentownername)
                {
                    documents.Add(document);
                }                
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            documents.OrderBy(d => d.name);
            if (sorting == "submitter")
            {
                return View(documents.OrderBy(o => o.submitter.name).ToList().ToPagedList(pageNumber, pageSize));
            }
            else if (sorting == "status")
            {
                return View(documents.OrderBy(o => o.description).ToList().ToPagedList(pageNumber, pageSize));
            }
            else if (sorting == "dateSubmitted")
            {
                return View(documents.OrderByDescending(o => o.dateSubmitted).ToList().ToPagedList(pageNumber, pageSize));
            }
            else if (sorting == "modified")
            {
                return View(documents.OrderByDescending(o => o.modified).ToList().ToPagedList(pageNumber, pageSize));
            }
            else if (sorting == "dateAccepted")
            {
                return View(documents.OrderByDescending(o => o.dateAccepted).ToList().ToPagedList(pageNumber, pageSize));
            }

            
            return View(documents.ToPagedList(pageNumber, pageSize));
        }       
    }
}
