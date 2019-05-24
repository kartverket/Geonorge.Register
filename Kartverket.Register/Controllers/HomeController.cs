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
            var redirectUrl = Url.Action(nameof(RegistersController.Index), "Registers");
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        public void SignOut()
        {
            var authenticationProperties = new AuthenticationProperties {RedirectUri = WebConfigurationManager.AppSettings["GeoID:PostLogoutRedirectUri"]};

            HttpContext.GetOwinContext().Authentication.SignOut(
                authenticationProperties,
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