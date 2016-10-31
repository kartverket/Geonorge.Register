using System;
using Kartverket.Register.Services.Register;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services;

namespace Kartverket.Register.Controllers
{
    public class DokCoverageController : Controller
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Guid SystemIdDokRegister = Guid.Parse("CD429E8B-2533-45D8-BCAA-86BC2CBDD0DD");

        private static readonly Dictionary<string, BoundingBoxViewModel> StateBoundingBoxes = new Dictionary<string, BoundingBoxViewModel>
        {
            {"02", new BoundingBoxViewModel("59.471882","10.328467","60.605148","11.926978")},
            {"09", new BoundingBoxViewModel("57.917234","6.824533","59.672687","9.668877" )},
            {"06", new BoundingBoxViewModel("59.407871","7.438842","61.09172" ,"10.634994")},
            {"20", new BoundingBoxViewModel("68.554592","20.479733","71.384879","31.761485")},
            {"04", new BoundingBoxViewModel("59.840785","9.583827","62.696928","12.870849")},
            {"12", new BoundingBoxViewModel("59.475421","4.184771","61.035238","7.732115" )},
            {"15", new BoundingBoxViewModel("61.956584","4.816602","63.768169","9.584212")},
            {"17", new BoundingBoxViewModel("63.180687","9.699817","65.470175","14.325986")},
            {"18", new BoundingBoxViewModel("64.939497","10.57806","69.596701","18.151355")},
            {"05", new BoundingBoxViewModel("60.13161","7.34253","62.378402","11.153986")},
            {"03", new BoundingBoxViewModel("59.809311","10.489165","60.135106","10.951389")},
            {"11", new BoundingBoxViewModel("58.027854","4.454274","59.844574","7.214667" )},
            {"14", new BoundingBoxViewModel("60.675548","4.087526","62.382396","8.322053")},
            {"16", new BoundingBoxViewModel("63.15","7.62","64.25","12.23")},
            {"08", new BoundingBoxViewModel("58.603311","7.096288","60.188272","9.969765" )},
            {"19", new BoundingBoxViewModel("68.356014","15.592542","70.703616","22.894466")},
            {"10", new BoundingBoxViewModel("57.759005","6.149699","59.189687","8.37168" )},
            {"07", new BoundingBoxViewModel("58.720455","9.755336","59.701938","10.67502" )},
            {"01", new BoundingBoxViewModel("58.76096","10.536679","59.790586","11.946004")}
        };

        private readonly IRegisterService _registerService;
        private readonly IDatasetService _datasetService;
        private readonly IOrganizationService _organizationService;

        public DokCoverageController(IRegisterService registerService, IDatasetService datasetService, IOrganizationService organizationService)
        {
            _registerService = registerService;
            _datasetService = datasetService;
            _organizationService = organizationService;
        }

        public ActionResult Index(string fylke, string dataset)
        {
            ViewBag.States = new SelectList(GetListOfStates(), "value", "name", fylke);

            Models.Register dokDatasets = _registerService.GetRegisterBySystemId(SystemIdDokRegister);

            List<DatasetCoverageViewModel> datasetCoverages = CreateViewModels(dokDatasets.items);

            ViewBag.Datasets = new SelectList(datasetCoverages.OrderBy(d => d.FullName), "DatasetUuid", "FullName", dataset);


            ShowDatasetCoverageViewModel model = new ShowDatasetCoverageViewModel();
            
            if (!string.IsNullOrWhiteSpace(dataset))
            {
                Dataset datasetItem = GetDatasetByUuid(dataset);

                model.DatasetUuid = dataset;
                model.DatasetCoverageConfirmedCounties = GetListOfConfirmedMunicipalitiesForDataset(datasetItem);
                if (DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping.ContainsKey(dataset))
                {
                    model.DatasetWmsLayerName = DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping[dataset];
                }
                else
                {
                    ViewBag.Warning = "Dekningskart mangler for valgte datasett.";
                }
            }

            if (!string.IsNullOrWhiteSpace(fylke))
            {
                model.StateName = fylke;
                model.StateBoundingBox = StateBoundingBoxes[fylke];
            }

            return View(model);
        }

        private IEnumerable<CodelistValue> GetListOfStates()
        {
            Models.Register register = _registerService.GetRegisterByName("Fylkesnummer");

            IEnumerable<CodelistValue> states = register.items.Cast<CodelistValue>();
            states = states.Where(i => i.value != "23"); // remove "Kontinentalsokkelen" not relevant in this context
            states = states.OrderBy(i => i.name);
            return states;
        }

        private List<CoverageConfirmedMunicipalityViewModel> GetListOfConfirmedMunicipalitiesForDataset(Dataset dataset)
        {
            var confirmed = new List<CoverageConfirmedMunicipalityViewModel>();

            foreach (CoverageDataset coverage in dataset.Coverage)
            {
                if (coverage.ConfirmedDok)
                { 
                    var confirmedMunicipality = new CoverageConfirmedMunicipalityViewModel();
                    confirmedMunicipality.Name = coverage.Municipality.name;
                    Organization org = _organizationService.GetOrganizationByNumber(coverage.Municipality.number);
                    confirmedMunicipality.Number = org.MunicipalityCode;
                    confirmedMunicipality.CenterCoordinateX = org.GeographicCenterX;
                    confirmedMunicipality.CenterCoordinateY = org.GeographicCenterY;

                    confirmed.Add(confirmedMunicipality);
                }
            }
            return confirmed;
        }

        private Dataset GetDatasetByUuid(string uuid)
        {
            return _datasetService.GetDatasetByUuid(uuid);
        }

        private List<DatasetCoverageViewModel> CreateViewModels(ICollection<RegisterItem> items)
        {
            var viewModels = new List<DatasetCoverageViewModel>();

            foreach (RegisterItem item in items)
            {
                Dataset dataset = (Dataset) item;

                if (string.IsNullOrWhiteSpace(dataset.Uuid))
                {
                    Log.Warn("Create list of DOK-coverage datasets, skipping dataset without uuid: " + dataset.name);
                }
                else
                {
                    var viewModel = new DatasetCoverageViewModel();
                    viewModel.ThemeGroupName = dataset.theme.description;
                    viewModel.DatasetName = dataset.name;
                    viewModel.DatasetUrl = dataset.GetObjectUrl();
                    viewModel.DatasetUuid = dataset.Uuid;
                    viewModels.Add(viewModel);
                }
            }
            return viewModels;
        }
    }
}