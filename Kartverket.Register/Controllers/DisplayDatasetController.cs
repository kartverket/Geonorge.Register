using System.Linq;
using System.Web.Mvc;
using Kartverket.DOK.Models;

namespace Kartverket.DOK.Controllers
{
    [HandleError]
    public class DisplayDatasetController : Controller
    {
        private Kartverket.Register.Models.RegisterDbContext db = new Kartverket.Register.Models.RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            return View(db.DokDatasets.ToList());
        }

        public ActionResult Display(int id, string name)
        {
            DokDataset dataset = db.DokDatasets.FirstOrDefault(d => d.Id == id);
            if (dataset == null)
            {
                return RedirectToAction("Index");
            }

            string seoUrl = dataset.ToSeoUrl();
            if (name != seoUrl)
                return RedirectToActionPermanent("Index", new { id, name = seoUrl });

            return View(dataset);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}