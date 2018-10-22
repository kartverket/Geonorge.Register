using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Services;
using Resources;
using SolrNet.Mapping.Validation.Rules;

namespace Kartverket.Register.Models.ViewModels
{
    public class GeodatalovDatasetStatusReportViewModel : StatusReportViewModel
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public DokLineChart DokHistoricalChart { get; set; }
        public StatusLineChart StatusChart { get; set; }

        // Metadata
        [Display(Name = "Metadata", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }


        // Productspecification
        [Display(Name = "DOK_ProductSpecificationStatus", ResourceType = typeof(DataSet))]
        public NumberOfStatuses NumberOfItemsWithProductSpecification { get; set; }
        

        // SosiRequirements
        [Display(Name = "DOK_Delivery_SosiRequirements", ResourceType = typeof(DataSet))]
        public NumberOfStatuses NumberOfItemsWithSosiRequirements { get; set; }


        // GmlRequirements
        [Display(Name = "DOK_Delivery_GmlRequirements", ResourceType = typeof(DataSet))]
        public NumberOfStatuses NumberOfItemsWithGmlRequirements { get; set; }
        

        // Wms
        [Display(Name = "DOK_Delivery_Wms", ResourceType = typeof(DataSet))]
        public NumberOfStatuses NumberOfItemsWithWms { get; set; }
        

        // Wfs
        [Display(Name = "DOK_Delivery_Wfs", ResourceType = typeof(DataSet))]
        public NumberOfStatuses NumberOfItemsWithWfs { get; set; }
        

        // AtomFeed
        [Display(Name = "DOK_Delivery_AtomFeed", ResourceType = typeof(DataSet))]
        public NumberOfStatuses NumberOfItemsWithAtomFeed { get; set; }


        // Common 
        [Display(Name = "Common", ResourceType = typeof(GeodatalovDataSet))]
        public NumberOfStatuses NumberOfItemsWithCommon { get; set; }


        //Inspire theme
        [Display(Name = "InspireTheme", ResourceType = typeof(GeodatalovDataSet))]
        public int InspireTheme { get; set; }

        // Dok
        [Display(Name = "Dok", ResourceType = typeof(GeodatalovDataSet))]
        public int Dok { get; set; }

        // National dataset
        [Display(Name = "NationalDataset", ResourceType = typeof(GeodatalovDataSet))]
        public int NationalDataset { get; set; }

        // Plan
        [Display(Name = "Plan", ResourceType = typeof(GeodatalovDataSet))]
        public int Plan { get; set; }

        // Geodatalov
        [Display(Name = "Geodatalov", ResourceType = typeof(GeodatalovDataSet))]
        public int Geodatalov { get; set; }


        public GeodatalovDatasetStatusReportViewModel(StatusReport statusReport, List<StatusReport> statusReports, string statusType)
        {
            ReportsSelectList = CreateSelectList(statusReports);
            StatusTypeSelectList = CreateStatusTypeSelectList();  // TODO
            DokHistoricalChart = new DokLineChart(statusReports, statusReport, statusType);
            StatusChart = new StatusLineChart(statusReports, statusReport, statusType);

            if (statusReport != null)
            {
                ReportNotExists = false;
                Id = statusReport.Id;
                Date = statusReport.Date;
                NumberOfItems = statusReport.NumberOfIems();

                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithMetadata(Good), statusReport.NumberOfGeodatalovDatasetsWithMetadata(Deficient), statusReport.NumberOfGeodatalovDatasetsWithMetadata(Notset), statusReport.NumberOfGeodatalovDatasetsWithMetadata(Useable));
                NumberOfItemsWithProductSpecification = new NumberOfStatuses(statusReport.NumberOfItemsWithProductSpecification(Good), statusReport.NumberOfItemsWithProductSpecification(Deficient), statusReport.NumberOfItemsWithProductSpecification(Notset), statusReport.NumberOfItemsWithProductSpecification(Useable));
                NumberOfItemsWithSosiRequirements = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Good), statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Deficient), statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Notset), statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Useable));
                NumberOfItemsWithGmlRequirements = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Good), statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Deficient), statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Notset), statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Useable));
                NumberOfItemsWithWms = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithWms(Good), statusReport.NumberOfGeodatalovDatasetsWithWms(Deficient), statusReport.NumberOfGeodatalovDatasetsWithWms(Notset), statusReport.NumberOfGeodatalovDatasetsWithWms(Useable));
                NumberOfItemsWithWfs = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithWfs(Good), statusReport.NumberOfGeodatalovDatasetsWithWfs(Deficient), statusReport.NumberOfGeodatalovDatasetsWithWfs(Notset), statusReport.NumberOfGeodatalovDatasetsWithWfs(Useable));
                NumberOfItemsWithAtomFeed = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Good), statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Deficient), statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Notset), statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Useable));
                NumberOfItemsWithCommon = new NumberOfStatuses(statusReport.NumberOfItemsWithCommon(Good), statusReport.NumberOfItemsWithCommon(Deficient), statusReport.NumberOfItemsWithCommon(Notset), statusReport.NumberOfItemsWithCommon(Useable));

                InspireTheme = statusReport.NumberOfGeodatalovDatasetsWithInspireTheme();
                Dok = statusReport.NumberOfGeodatalovDatasetsWithDok();
                NationalDataset = statusReport.NumberOfGeodatalovDatasetsWithNationalDataset();
                Plan = statusReport.NumberOfGeodatalovDatasetsWithPlan();
                Geodatalov = statusReport.NumberOfGeodatalovDatasetsWithGeodatalov();
            }
            else
            {
                ReportNotExists = true;
            }
        }


        private SelectList CreateStatusTypeSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Text = Shared.ShowAll, Value = "all" });
            items.Add(new SelectListItem() { Text = InspireDataSet.Metadata, Value = "Metadata" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_ProductSpecificationStatus, Value = "ProductSpecification" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_SosiRequirements, Value = "SosiRequirements" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_GmlRequirements, Value = "GmlRequirements" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Wms, Value = "Wms" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Wfs, Value = "Wfs" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_AtomFeed, Value = "AtomFeed" });
            items.Add(new SelectListItem() { Text = GeodatalovDataSet.Common, Value = "Common" });
            items.Add(new SelectListItem() { Text = GeodatalovDataSet.InspireTheme, Value = "InspireTheme" });
            items.Add(new SelectListItem() { Text = GeodatalovDataSet.Dok, Value = "Dok" });
            items.Add(new SelectListItem() { Text = GeodatalovDataSet.NationalDataset, Value = "NationalDataset" });
            items.Add(new SelectListItem() { Text = GeodatalovDataSet.Plan, Value = "Plan" });
            items.Add(new SelectListItem() { Text = GeodatalovDataSet.Geodatalov, Value = "Geodatalov" });

            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }


    }
}