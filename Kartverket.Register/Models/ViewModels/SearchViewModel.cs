using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Kartverket.Register.Models.ViewModels
{
    public class SearchViewModel
    {

        public string Text { get; set; }
        public string Register { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int NumFound { get; set; }
      
        public SearchResultViewModel Result { get; set; }

        public SearchViewModel(SearchParameters parameters, SearchResult searchResult)
        {
            Text = parameters.Text;
            Register = parameters.Register;
            Result = new SearchResultViewModel(searchResult);
            Limit = searchResult.Limit;
            Offset = searchResult.Offset;
            NumFound = searchResult.NumFound;
        }

        public bool IsPreviousButtonActive()
        {
            return Offset > 1 && (Offset - Limit) >= 1;
        }

        public RouteValueDictionary ParamsForPreviousLink()
        {
            var routeValues = new RouteValueDictionary();
            routeValues = CreateLinkWithParameters(routeValues);
            routeValues["Offset"] = (Offset - Limit);
            return routeValues;
        }

        public RouteValueDictionary ParamsForNextLink()
        {
            var routeValues = new RouteValueDictionary();
            routeValues = CreateLinkWithParameters(routeValues);
            routeValues["Offset"] = (Offset + Limit);
            return routeValues;
        }

        private RouteValueDictionary CreateLinkWithParameters(RouteValueDictionary routeValues, int index = 0)
        {
            routeValues["text"] = Text;
            routeValues["register"] = Register;

            return routeValues;
        }
        public string ShowingFromAndTo()
        {
            int from = Offset;
            int to = (Offset + Limit - 1);

            if (to > NumFound)
            {
                to = NumFound;
            }

            return string.Format("{0} - {1}", from, to);
        }

        public bool IsNextButtonActive()
        {
            return NumFound > (Offset + Limit);
        }

    }
}