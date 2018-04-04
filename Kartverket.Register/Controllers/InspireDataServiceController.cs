using Kartverket.Register.Models;
using Kartverket.Register.Services;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Controllers
{
    public class InspireDataServiceController : Controller
    {
        private readonly IInspireDatasetService _inspireDatasetService;
        private readonly IAccessControlService _accessControlService;

        public InspireDataServiceController(IInspireDatasetService inspireDatasetService, IAccessControlService accessControllService)
        {
            _inspireDatasetService = inspireDatasetService;
            _accessControlService = accessControllService;
        }


        // GET: InspireDataService/Edit/5
        [Authorize]
        [Route("inspire-data-service/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemname)
        {
            var inspireDataService = _inspireDatasetService.GetInspireDataServiceByName(registername, itemname);
            if (inspireDataService == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (_accessControlService.Access(inspireDataService))
            {
                return View(inspireDataService);
            }
            throw new HttpException(401, "Access Denied");
        }

        // POST: InspireDataService/Edit/5
        [HttpPost]
        [Authorize]
        [Route("inspire-data-service/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(InspireDataService inspireDataService)
        {
            try
            {
                var updatedInspireDataset = _inspireDatasetService.UpdateInspireDataService(inspireDataService);
                return Redirect(updatedInspireDataset.DetailPageUrl());
            }
            catch
            {
                return View();
            }
        }

    }
}
