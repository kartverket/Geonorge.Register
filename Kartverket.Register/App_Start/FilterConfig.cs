using Kartverket.Register.Helpers;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new Log4netExceptionFilter()); //must be before HandleErrorAttribute
            filters.Add(new HandleErrorAttribute());
        }
    }
}
