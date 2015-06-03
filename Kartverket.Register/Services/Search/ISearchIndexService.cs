using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services.Search
{
    public interface ISearchIndexService
    {
        SearchResult Search(SearchParameters parameters);
    }
}