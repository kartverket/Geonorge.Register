using System;
using Kartverket.Register.Services.Register;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Controllers
{
    public class DokCoverageController : Controller
    {
        private static readonly Guid SystemIdDokRegister = Guid.Parse("CD429E8B-2533-45D8-BCAA-86BC2CBDD0DD");

        private readonly IRegisterService _registerService;

        public DokCoverageController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        public ActionResult Index(string fylke)
        {
            Models.Register register = _registerService.GetRegisterByName("Fylkesnummer");
            IEnumerable<RegisterItem> states = register.items.OrderBy(i => i.name);

            ViewBag.States = new SelectList(states, "value", "name", fylke);

            Models.Register dokDatasets = _registerService.GetRegisterBySystemId(SystemIdDokRegister);

            List<DatasetCoverageViewModel> datasetCoverages = CreateViewModels(dokDatasets.items);

            IOrderedEnumerable<DatasetCoverageViewModel> sortedDatasetCoverages = datasetCoverages.OrderBy(d => d.FullName);

            ViewBag.Datasets = new SelectList(sortedDatasetCoverages, "DatasetUrl", "FullName");

            return View();
        }

        private List<DatasetCoverageViewModel> CreateViewModels(ICollection<RegisterItem> items)
        {
            var viewModels = new List<DatasetCoverageViewModel>();

            foreach (RegisterItem item in items)
            {
                var viewModel = new DatasetCoverageViewModel();

                Dataset dataset = (Dataset) item;
                viewModel.ThemeGroupName = dataset.theme.description;
                viewModel.DatasetName = dataset.name;
                viewModel.DatasetUrl = dataset.GetObjectUrl();

                viewModels.Add(viewModel);
            }
            return viewModels;
        }
    }
}