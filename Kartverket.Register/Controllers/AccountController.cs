using System.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Kartverket.DOK.Models;
using Kartverket.Register.Models;

namespace Kartverket.DOK.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vennligst fyll ut påkrevde felter.";
                return View("Login");
            }

            if (login.Username.Equals(ConfigurationManager.AppSettings["AdminUsername"])
                && login.Password.Equals(ConfigurationManager.AppSettings["AdminPassword"]))
            {
                FormsAuthentication.RedirectFromLoginPage(login.Username, true);
            }

            ViewBag.Error = "Feil brukernavn/passord.";
            return View("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}