using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kartverket.Register.Models;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using SearchParameters = Kartverket.Register.Models.SearchParameters;
using SearchResult = Kartverket.Register.Models.SearchResult;
using System;

namespace Kartverket.Register.Services.Search
{
    public class SearchIndexService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private readonly ISolrOperations<RegisterIndexDoc> _solrInstance;

        public SearchIndexService()
        {
            _solrInstance = ServiceLocator.Current.GetInstance<ISolrOperations<RegisterIndexDoc>>();
        }

        public SearchResult Search(SearchParameters parameters)
        {

            ISolrQuery query = BuildQuery(parameters);
            var order =new[] {new SortOrder("score", Order.DESC)};
            if (parameters.OrderBy == "title")
            {
                order = new[] { new SortOrder("RegisterItemName", Order.ASC) };
            }
            else if (parameters.OrderBy == "date_updated")
            {
                order = new[] { new SortOrder("RegisterItemUpdated", Order.DESC) };
            }
            else if (string.IsNullOrWhiteSpace(parameters.Text) && HasNoFacetvalue(parameters.Facets))
            {
                order = new[] { new SortOrder("RegisterItemName", Order.ASC) };
            }
            else if (parameters.OrderBy == "score")
            {
                order = new[] { new SortOrder("score", Order.DESC) };
            }
            try
            {

                SolrQueryResults<RegisterIndexDoc> queryResults = _solrInstance.Query(query, new QueryOptions
                {

                    FilterQueries = BuildFilterQueries(parameters),
                    OrderBy = order,
                    Rows = parameters.Limit,
                    Start = parameters.Offset -1,
                    Facet = BuildFacetParameters(parameters),

                    Fields = new[] { "SystemID", "RegisterName", "RegisterDescription", "RegisterItemName", "RegisterItemDescription", "RegisterID", "Discriminator", "RegisterItemUpdated", "Type",
                    "ParentRegisterUrl", "RegisteItemUrl",  "SubregisterUrl","subregisterItemUrl", "theme" , "organization" }
                    

                });
                return CreateSearchResults(queryResults, parameters);
            }
            catch (Exception ex)
            {
                Log.Error("Error in search", ex);

                return CreateSearchResults(null, parameters);
            }
            
        }

        private bool HasNoFacetvalue(List<FacetParameter> list)
        {

            bool hasnovalue = true;
            foreach (FacetParameter f in list)
            {
                if (!string.IsNullOrEmpty(f.Value))
                {
                    hasnovalue = false;
                    break;
                }
            }
            return hasnovalue;
        }


        private SearchResult CreateSearchResults(SolrQueryResults<RegisterIndexDoc> queryResults, SearchParameters parameters)
        {
            List<SearchResultItem> items = ParseResultDocuments(queryResults);

            List<Facet> facets = ParseFacetResults(queryResults);

            return new SearchResult
            {
                Items = items,
                Facets = facets,
                Limit = parameters.Limit,
                Offset = parameters.Offset,
                NumFound = queryResults.NumFound
            };
        }

        private List<Facet> ParseFacetResults(SolrQueryResults<RegisterIndexDoc> queryResults)
        {
            List<Facet> facets = new List<Facet>();
            if (queryResults != null)
            {
                foreach (var key in queryResults.FacetFields.Keys)
                {
                    var facet = new Facet
                    {
                        FacetField = key,
                        FacetResults = new List<Facet.FacetValue>()
                    };
                    foreach (var facetValueResult in queryResults.FacetFields[key])
                    {
                        facet.FacetResults.Add(new Facet.FacetValue
                        {
                            Name = facetValueResult.Key,
                            Count = facetValueResult.Value
                        });
                    }
                    facets.Add(facet);
                }
            }
            return facets;
        }

        private static List<SearchResultItem> ParseResultDocuments(SolrQueryResults<RegisterIndexDoc> queryResults)
        {
            var items = new List<SearchResultItem>();
            if (queryResults != null)
            {
                foreach (var doc in queryResults)
                {
                    Log.Debug(doc.Score + " " + doc.SystemID);

                    var item = new SearchResultItem
                    {
                        CodelistValue = doc.CodelistValue,
                        currentVersion = doc.currentVersion,
                        DatasetOwner = doc.DatasetOwner,
                        Discriminator = doc.Discriminator,
                        DocumentOwner = doc.DocumentOwner,
                        ObjektkatalogUrl = doc.ObjektkatalogUrl,
                        ParentRegisterDescription = doc.ParentRegisterDescription,
                        ParentRegisterId = doc.ParentRegisterId,
                        ParentRegisterName = doc.ParentRegisterName,
                        ParentregisterOwner = doc.ParentregisterOwner,
                        ParentRegisterSeoname = doc.ParentRegisterSeoname,
                        ParentRegisterUrl = doc.ParentRegisterUrl,
                        RegisteItemUrl = doc.RegisteItemUrl,
                        RegisteItemUrlDataset = doc.RegisteItemUrlDataset,
                        RegisteItemUrlDocument = doc.RegisteItemUrlDocument,
                        RegisterDescription = doc.RegisterDescription,
                        RegisterID = doc.RegisterID,
                        RegisterItemDescription = doc.RegisterItemDescription,
                        RegisterItemName = doc.RegisterItemName,
                        RegisterItemSeoname = doc.RegisterItemSeoname,
                        RegisterItemStatus = doc.RegisterItemStatus,
                        RegisterItemUpdated = doc.RegisterItemUpdated,
                        RegisterName = doc.RegisterName,
                        RegisterSeoname = doc.RegisterSeoname,
                        Shortname = doc.Shortname,
                        Submitter = doc.Submitter,
                        subregisterItemUrl = doc.subregisterItemUrl,
                        SubregisterUrl = doc.SubregisterUrl,
                        SystemID = doc.SystemID,
                        Type = doc.Type,
                        theme = doc.theme,
                        organization = doc.Organization
                       
                    };
                    items.Add(item);
                }
            }
            return items;
        }

        private static FacetParameters BuildFacetParameters(SearchParameters parameters)
        {
            return new FacetParameters
            {
                Queries = parameters.Facets.Select(item => 
                    new SolrFacetFieldQuery(item.Name) { MinCount = 1, Limit=150,  Sort=false }
                    ).ToList<ISolrFacetQuery>()
            };
        }

        private ICollection<ISolrQuery> BuildFilterQueries(SearchParameters parameters)
        {
            return parameters.Facets
                .Where(f => !string.IsNullOrWhiteSpace(f.Value))
                .Select(f => new SolrQueryByField(f.Name, f.Value))
                .ToList<ISolrQuery>();
        }

        private ISolrQuery BuildQuery(SearchParameters parameters)
        {
            var text = parameters.Text;
            ISolrQuery query;
            
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace(":", " ");
                text = text.Replace("!", " ");
                text = text.Replace("{", " ");
                text = text.Replace("}", " ");
                text = text.Replace("[", " ");
                text = text.Replace("]", " ");
                text = text.Replace("(", " ");
                text = text.Replace(")", " ");
                text = text.Replace("^", " ");
                
                if (text.Trim().Length == 0) query = SolrQuery.All;
                else if (text.Trim().Length < 5)
                {
                    query = new SolrMultipleCriteriaQuery(new[]
                    {
                        new SolrQuery("registerText:"+ text + "^50"),
                        new SolrQuery("registerItemText:"+ text + "^50"),
                        new SolrQuery("registerText:"+ text + "*^40"),
                        new SolrQuery("registerItemText:"+ text + "*^40"),
                        new SolrQuery("allText:" + text + "^1.2"),
                        new SolrQuery("allText:" + text + "*^1.1")
                    });
                }
                else
                {
                    query = new SolrMultipleCriteriaQuery(new[]
                    {
                        new SolrQuery("registerText:"+ text + "^50"),
                        new SolrQuery("registerItemText:"+ text + "^50"),
                        new SolrQuery("registerText:"+ text + "*^40"),
                        new SolrQuery("registerItemText:"+ text + "*^40"),
                        new SolrQuery("registerText:"+ text + "~2^1.1"),
                        new SolrQuery("registerItemText:"+ text + "~2^1.1"),
                        new SolrQuery("allText:" + text + "^1.2"),
                        new SolrQuery("allText:" + text + "*^1.1"),
                        new SolrQuery("allText:" + text + "~1"),   //Fuzzy
                        new SolrQuery("allText2:" + text + "") //Stemmer
                        //new SolrQuery("allText3:" + text)        //Fonetisk
                    });
                }
            }
            else query = SolrQuery.All;
            
            Log.Debug("Query: " + query.ToString());
            return query; 
        }


    }
}