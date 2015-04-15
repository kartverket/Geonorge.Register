using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kartverket.Register.Services.Search
{
    public interface ISearchService
    {
        SearchResult Search(SearchParameters parameters);
        FilterResult Filter(SearchParameters parameters);
    }
}
