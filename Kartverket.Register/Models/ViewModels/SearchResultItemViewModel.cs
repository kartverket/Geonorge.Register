using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class SearchResultItemViewModel
    {

        public string RegisterName { get; set; }
        public string RegisterDescription { get; set; }
        public string RegisterItemName { get; set; }
        public string RegisterItemDescription { get; set; }
        public Guid RegisterID { get; set; }
        public Guid SystemID { get; set; }
        public string Discriminator { get; set; }


        private SearchResultItemViewModel(SearchResultItem item)
        {
            RegisterName = item.RegisterName;
            RegisterDescription = item.RegisterDescription;
            RegisterItemName = item.RegisterItemName;
            RegisterItemDescription = item.RegisterItemDescription;
            RegisterID = item.RegisterID;
            SystemID = item.SystemID;
            Discriminator = item.Discriminator;
        }

        public static List<SearchResultItemViewModel> CreateFromList(IEnumerable<SearchResultItem> items)
        {
            return items.Select(item => new SearchResultItemViewModel(item)).ToList();
        }

    }
}