using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Geonorge.AuthLib.Common;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace Kartverket.Register.Controllers
{
    public class HomeController : Controller
    {
        public void SignIn()
        {
            var redirectUri = Url.Action(nameof(RegistersController.Index), "Registers");

            if (Request.QueryString["ReturnUrl"] != null)
            {
                redirectUri = Request.QueryString["ReturnUrl"];
            }

            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUri },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public void SignOut()
        {
            var redirectUri = WebConfigurationManager.AppSettings["GeoID:PostLogoutRedirectUri"];

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties {RedirectUri = redirectUri},
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }

        /// <summary>
        /// This is the action responding to /signout-callback-oidc route after logout at the identity provider
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOutCallback()
        {
            return RedirectToAction(nameof(RegistersController.Index), "Registers");
        }
    }
}