using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class SearchResultItemViewModel
    {

        public string Name { get; set; }
        public string Description { get; set; }

        private SearchResultItemViewModel(SearchResultItem item)
        {
            Name = item.Name;
            Description = item.Description;
        }

        public static List<SearchResultItemViewModel> CreateFromList(IEnumerable<SearchResultItem> items)
        {
            return items.Select(item => new SearchResultItemViewModel(item)).ToList();
        }

    }
}