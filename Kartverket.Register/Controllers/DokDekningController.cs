using Kartverket.Register.Services.Register;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kartverket.Register.Controllers
{
    public class DokDekningController : Controller
    {
        private readonly IRegisterService _registerService;

        public DokDekningController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        public ActionResult Index()
        {
            Models.Register register = _registerService.GetRegisterByName("Fylkesnummer");
            IEnumerable<Models.RegisterItem> states = register.items.OrderBy(i => i.name);

            ViewBag.States = new SelectList(states, "value", "name");

            return View();
        }
    }
}