using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Controllers
{
    public class InspireDataServiceController : Controller
    {
        private readonly IInspireDatasetService _inspireDatasetService;
        private readonly IAccessControlService _accessControlService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;

        public InspireDataServiceController(IInspireDatasetService inspireDatasetService, IAccessControlService accessControllService, IDatasetDeliveryService datasetDeliveryService)
        {
            _inspireDatasetService = inspireDatasetService;
            _accessControlService = accessControllService;
            _datasetDeliveryService = datasetDeliveryService;
        }


        // GET: InspireDataService/Edit/5
        [Authorize]
        //[Route("inspire-data-service/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemname)
        {
            var inspireDataService = _inspireDatasetService.GetInspireDataServiceByName(registername, itemname);
            if (inspireDataService == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (_accessControlService.HasAccessTo(inspireDataService))
            {
                inspireDataService = _inspireDatasetService.UpdateInspireDataServiceFromKartkatalogen(inspireDataService);
                var viewModel = new InspireDataServiceViewModel(inspireDataService);
                ViewBags(viewModel);
                return View(viewModel);
            }
            throw new HttpException(401, "Access Denied");
        }

        private void ViewBags(InspireDataServiceViewModel viewModel)
        {
            ViewBag.MetadataStatusId = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatusId);
            ViewBag.MetadataInSearchServiceStatusId = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataInSearchServiceStatusId);
            ViewBag.ServiceStatusId = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.ServiceStatusId);
        }

        // POST: InspireDataService/Edit/5
        [HttpPost]
        [Authorize]
        //[Route("inspire-data-service/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(InspireDataServiceViewModel viewModel)
        {
            try
            {
                var updatedInspireDataService = _inspireDatasetService.UpdateInspireDataService(viewModel);
                return Redirect(updatedInspireDataService.DetailPageUrl());
            }
            catch
            {
                return View();
            }
        }

        // GET: InspireDataService/Delete/5
        [Authorize]
        //[Route("inspire-data-service/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult Delete(string parentregister, string registername, string itemname, string itemowner)
        {
            var inspireDataService = _inspireDatasetService.GetInspireDataServiceByName(registername, itemname);
            if (inspireDataService != null)
            {
                if (_accessControlService.HasAccessTo(inspireDataService))
                {
                    return View(inspireDataService);
                }
                throw new HttpException(401, "Access Denied");
            }
            return HttpNotFound("Finner ikke datasettet");
        }

        // POST: InspireDataService/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        //[Route("inspire-data-service/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult DeleteConfirmed(string registername, string itemname)
        {
            var inspireDataset = _inspireDatasetService.GetInspireDataServiceByName(registername, itemname);
            if (inspireDataset != null)
            {
                var registerUrl = inspireDataset.Register.GetObjectUrl();
                if (_accessControlService.HasAccessTo(inspireDataset))
                {
                    _inspireDatasetService.DeleteInspireDataService(inspireDataset);
                    return Redirect(registerUrl);
                }
                throw new HttpException(401, "Access Denied");
            }
            return HttpNotFound("Finner ikke datasettet");
        }

    }
}
