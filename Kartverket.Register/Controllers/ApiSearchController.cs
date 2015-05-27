using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Search;
using Kartverket.Register.Models.Api;
using SearchParameters = Kartverket.Register.Models.Api.SearchParameters;
using SearchResult = Kartverket.Register.Models.Api.SearchResult;
using System;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApiSearchController : ApiController
    {
        private readonly SearchIndexService _searchService;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ApiSearchController()
        {
            _searchService = new SearchIndexService();
        }

        public SearchResult Get([System.Web.Http.ModelBinding.ModelBinder(typeof(SM.General.Api.FieldValueModelBinder))] SearchParameters parameters)
        {
            try
            {

                if (parameters == null)
                    parameters = new SearchParameters();

                Models.SearchParameters searchParameters = CreateSearchParameters(parameters);
                searchParameters.AddDefaultFacetsIfMissing();
                Models.SearchResult searchResult = _searchService.Search(searchParameters);


                return new SearchResult(searchResult);
            }
            catch (Exception ex)
            {
                Log.Error("Error API", ex);
                return null;
            }

        }

        private Models.SearchParameters CreateSearchParameters(SearchParameters parameters)
        {
            return new Models.SearchParameters
            {
                Text = parameters.text,
                Facets = CreateFacetParameters(parameters.facets),
                Offset = parameters.offset,
                Limit = parameters.limit
            };
        }

        private List<FacetParameter> CreateFacetParameters(IEnumerable<FacetParameter> facets)
        {
            return facets
                .Select(item => new FacetParameter
                {
                    Name = item.Name,
                    Value = item.Value
                })
                .ToList();
        }


    }
}
