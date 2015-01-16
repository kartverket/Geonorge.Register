using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Services.Search;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

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

        

        [Route("dokument/{registername}/{documentownername}/")]
        public ActionResult DetailsFilterDocument(string registername, string documentownername)
        {

            var queryResultsTest = from d in db.Documents
                                  where d.register.seoname == registername && d.documentowner.seoname == documentownername
                                  select d.systemId;

            List<Guid> documentsId = queryResultsTest.ToList();
            List<Document> documents = new List<Document>(); 
            
            foreach (Guid item in documentsId)
            {
                Kartverket.Register.Models.Document document = db.Documents.Find(item);
                if (document.documentowner.seoname == documentownername)
                {
                    documents.Add(document);
                }                
            }            
           return View(documents);
        }
    }
}
