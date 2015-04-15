using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class FilterResult
    {
        public List<Filter> ItemsFilter { get; set; }
        public int NumFound { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }

        public FilterResult()
        {

        }

        public FilterResult(FilterResult otherResult)
        {
            ItemsFilter = otherResult.ItemsFilter;
            NumFound = otherResult.NumFound;
            Limit = otherResult.Limit;
            Offset = otherResult.Offset;
        }
    }
}