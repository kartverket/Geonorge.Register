using System.Web.Http;
using Kartverket.ReportApi;

namespace Kartverket.Register.Controllers
{
    public class ReportController : ApiController
    {

        public void Get()
        {
            var query = new ReportQuery();
        }


    }
}