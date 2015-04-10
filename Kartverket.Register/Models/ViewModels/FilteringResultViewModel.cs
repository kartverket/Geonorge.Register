using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class FilteringResultViewModel
    {
        public List<FilteringResultItemViewModel> Items { get; set; }

        public FilteringResultViewModel(FilterResult filterResult)
        {
            Items = FilteringResultItemViewModel.CreateFromList(filterResult.ItemsFilter);
        }
    }
}