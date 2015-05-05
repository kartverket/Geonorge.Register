using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound(string url)
        {
            var originalUri = url ?? Request.QueryString["aspxerrorpath"] ?? Request.Url.OriginalString;

            var controllerName = (string)RouteData.Values["controller"];
            var actionName = (string)RouteData.Values["action"];
            var model = new NotFoundModel(new HttpException(404, "Failed to find page"), controllerName, actionName)
            {
                RequestedUrl = originalUri,
                ReferrerUrl = Request.UrlReferrer == null ? "" : Request.UrlReferrer.OriginalString
            };

            Response.StatusCode = 404;
            return View("NotFound", model);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            var name = GetViewName(ControllerContext,
                                                        "~/Views/Error/Error.cshtml",
                                                        "~/Views/Error/General.cshtml",
                                                        "~/Views/Shared/Error.cshtml");

            var controllerName = (string)RouteData.Values["controller"];
            var model = new HandleErrorInfo(Server.GetLastError(), controllerName, actionName);
            var result = new ViewResult
            {
                ViewName = name,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            };


            Response.StatusCode = 501;
            result.ExecuteResult(ControllerContext);
        }

        protected string GetViewName(ControllerContext context, params string[] names)
        {
            foreach (var name in names)
            {
                var result = ViewEngines.Engines.FindView(ControllerContext, name, null);
                if (result.View != null)
                    return name;
            }
            return null;
        }
    }
}