using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public List<SearchResultItemViewModel> Items { get; set; }

        public SearchResultViewModel(SearchResult searchResult)
        {
            Items = SearchResultItemViewModel.CreateFromList(searchResult.Items);
        }


    }
}