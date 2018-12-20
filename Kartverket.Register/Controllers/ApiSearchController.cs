﻿using System.Collections.Generic;
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
using System.Net.Http;
using Kartverket.Register.Models.Translations;
using System.Threading;
using System.Globalization;
using Kartverket.Register.Helpers;
using System.Net.Http.Headers;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApiSearchController : ApiController
    {

        private readonly ISearchIndexService _searchIndexService;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ApiSearchController(ISearchIndexService searchService)
        {
            _searchIndexService = searchService;
        }

        /// <summary>
        /// Register search
        /// </summary>
        public SearchResult Get([System.Web.Http.ModelBinding.ModelBinder(typeof(SM.General.Api.FieldValueModelBinder))] SearchParameters parameters)
        {
            try
            {
                SetLanguage(Request);
                if (parameters == null)
                    parameters = new SearchParameters();

                Models.SearchParameters searchParameters = CreateSearchParameters(parameters);
                searchParameters.AddDefaultFacetsIfMissing();
                Models.SearchResult searchResult = _searchIndexService.Search(searchParameters);


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
                Limit = parameters.limit,
                OrderBy = parameters.orderby,
                IncludeObjektkatalog = false
            };
        }

        private List<FacetParameter> CreateFacetParameters(IEnumerable<FacetInput> facets)
        {
            return facets
                .Select(item => new FacetParameter
                {
                    Name = item.name,
                    Value = item.value
                })
                .ToList();
        }

        private void SetLanguage(HttpRequestMessage request)
        {
            string language = Culture.NorwegianCode;

            var lang = Kartverket.Register.Helpers.HttpRequestMessageExtensions.GetQueryString(request, "lang");

            CookieHeaderValue cookie = request.Headers.GetCookies("_culture").FirstOrDefault();

            if (!string.IsNullOrEmpty(lang))
            {
                cookie = null;
                language = lang;
            }

            if (cookie != null && !string.IsNullOrEmpty(cookie["_culture"].Value))
            {
                language = cookie["_culture"].Value;
            }
            else
            {
                IEnumerable<string> headerValues;
                if (request.Headers.TryGetValues("Accept-Language", out headerValues))
                {
                    language = headerValues.FirstOrDefault();
                    if (CultureHelper.IsNorwegian(language))
                        language = Culture.NorwegianCode;
                    else
                        language = Culture.EnglishCode;
                }
            }

            var culture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

        }
    }
}
