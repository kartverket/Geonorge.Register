using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Kartverket.Register.Models.ViewModels
{
    public class FilteringViewModel
    {
        public string Text { get; set; }
        public string Owner { get; set; }
        public string Register { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int NumFound { get; set; }
        public int pages { get; set; }
        public int page { get; set; }
        public int startPage { get; set; }
        public int endPage { get; set; }
        public string Orderby { get; set; }

        public FilteringResultViewModel FilterResult { get; set; }


        public FilteringViewModel(SearchParameters parameters, FilterResult filterResult)
        {

            Text = parameters.Text;
            Register = parameters.Register;
            Owner = parameters.Owner;
            FilterResult = new FilteringResultViewModel(filterResult);
            Limit = filterResult.Limit;
            Offset = filterResult.Offset;
            NumFound = filterResult.NumFound;
            Orderby = parameters.OrderBy;

            page = 1;
            if (Offset != 1)
            {
                page = (Offset / Limit) + 1;
            }

            //Finne totalt antall sider
            pages = NumFound / Limit;

            //Test om det er noe bak komma.... 
            if ((Limit * pages) != NumFound)
            {
                pages = pages + 1;
            }

            //Hvilke sider som skal være synlige
            if (pages > 5)
            {
                startPage = 1;
                endPage = 5;

                if (page > 3 && page <= (pages - 2))
                {
                    startPage = page - 2;
                    endPage = page + 2;
                }
                if (page > (pages - 2) && page > 3)
                {
                    startPage = pages - 4;
                    endPage = pages;
                }
            }
            else
            {
                startPage = 1;
                endPage = pages;
            }
        }

        public bool IsActivePage(int i)
        {

            if (i == page)
            {
                return true;
            }
            else return false;
        }

        public bool IsPreviousButtonActive()
        {
            return Offset > 1 && (Offset - Limit) >= 1;
        }

        public RouteValueDictionary ParamsForPageNumber(int page)
        {
            page = page - 1;

            var routeValues = new RouteValueDictionary();
            routeValues = CreateLinkWithParameters(routeValues);
            routeValues["Offset"] = page * Limit + 1;
            return routeValues;
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
            routeValues["OrderBy"] = Orderby;
            routeValues["Owner"] = Owner;

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
            return NumFound >= (Offset + Limit);
        }

    }
}