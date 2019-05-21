using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
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
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}