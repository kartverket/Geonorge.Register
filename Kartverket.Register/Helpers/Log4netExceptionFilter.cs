using System;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Helpers
{
    public class Log4netExceptionFilter : IExceptionFilter
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void OnException(ExceptionContext context)
        {
            Exception ex = context.Exception;
            if (!(ex is HttpException)) //ignore "file not found"
            {
                Log.Error("App_Error", ex);
            }
        }
    }
}