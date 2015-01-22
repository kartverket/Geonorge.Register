using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace Kartverket.Register.Controllers
{
    public class SearchApiController : ApiController
    {
        private RegisterDbContext db = new RegisterDbContext();

        private readonly ISearchService _searchService;

        public SearchApiController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        //Søket skal gi "Mine registerdata"
        [Route("api/register/search/organisasjon/{name}")]
        [HttpGet]
        public IHttpActionResult SearchByOrganizationName(string name)
        {
            List<Kartverket.Register.Models.Api.Item> resultat = new List<Models.Api.Item>();

            //Hvilke orgnavn er aktuelle?
            SearchParameters parameters = new SearchParameters();
            parameters.Text = name;

            var urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext); 

            SearchResult searchResult = _searchService.Search(parameters);
            //registry -> register -> containeditems
            foreach (var it in searchResult.Items)
            {
                resultat.Add(Convert(it, urlHelper));
            }

            return Ok(resultat);
            
        }

        private Models.Api.Item Convert(SearchResultItem searchitem, System.Web.Mvc.UrlHelper urlHelper)
        {
            var tmp= new Models.Api.Item
            {
                name = searchitem.RegisterItemName,
                description = searchitem.RegisterItemDescription,
                status = searchitem.RegisterItemStatus,
                updated = searchitem.RegisterItemUpdated,
                author = searchitem.DocumentOwner,
                showUrl =  searchitem.RegisteItemUrl,
                editUrl = null
                
            };
            
            return tmp;
        }
    }
}
