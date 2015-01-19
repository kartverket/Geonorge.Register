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
                status = "Unknown",
                showUrl =  "",
                editUrl = ""
                
            };
            if (searchitem.Discriminator == "Document")
            {
                tmp.editUrl = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/dokument/" + searchitem.RegisterSeoname + "/" + searchitem.DocumentOwner + "/" + searchitem.RegisterItemSeoname + "/rediger/";
                tmp.showUrl = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/" + searchitem.RegisterSeoname + "/" + searchitem.DocumentOwner + "/" + searchitem.RegisterItemSeoname;
            }
            else if (searchitem.Discriminator == "EPSG")
            {
                tmp.editUrl = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/epsg-koder/" + searchitem.RegisterSeoname + "/" + searchitem.DocumentOwner + "/" + searchitem.RegisterItemSeoname + "/rediger/";
                tmp.showUrl = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/" + searchitem.RegisterSeoname + "/" + searchitem.DocumentOwner + "/" + searchitem.RegisterItemSeoname;
            }
            else 
            {
                tmp.editUrl = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/" + searchitem.RegisterSeoname + "/" + searchitem.RegisterItemSeoname + "/rediger/";
                tmp.showUrl = urlHelper.RequestContext.HttpContext.Request.Url.Scheme + "://" + urlHelper.RequestContext.HttpContext.Request.Url.Authority + "/" + searchitem.RegisterSeoname + "/" + searchitem.RegisterItemSeoname;
            }
            return tmp;
        }
    }
}
