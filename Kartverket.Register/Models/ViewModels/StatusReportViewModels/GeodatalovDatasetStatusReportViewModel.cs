using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models.ViewModels.StatusReportViewModels;
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

        public GeodatalovLineChart GeodatalovLineChart { get; set; }
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
        public int NumberOfItemsWithInspireTheme { get; set; }

        // Dok
        [Display(Name = "Dok", ResourceType = typeof(GeodatalovDataSet))]
        public int NumberOfItemsWithDok { get; set; }

        // National dataset
        [Display(Name = "NationalDataset", ResourceType = typeof(GeodatalovDataSet))]
        public int NumberOfItemsWithNationalDataset { get; set; }

        // Plan
        [Display(Name = "Plan", ResourceType = typeof(GeodatalovDataSet))]
        public int NumberOfItemsWithPlan { get; set; }

        // Geodatalov
        [Display(Name = "Geodatalov", ResourceType = typeof(GeodatalovDataSet))]
        public int NumberOfItemsWithGeodatalov { get; set; }


        public GeodatalovDatasetStatusReportViewModel(StatusReport statusReport, List<StatusReport> statusReports, string statusType)
        {
            ReportsSelectList = CreateSelectList(statusReports);
            StatusTypeSelectList = CreateStatusTypeSelectList();  // TODO
            GeodatalovLineChart = new GeodatalovLineChart(statusReports, statusReport, statusType);
            StatusChart = new StatusLineChart(statusReports, statusReport, statusType);

            if (statusReport != null)
            {
                ReportNotExists = false;
                Id = statusReport.Id;
                Date = statusReport.Date;
                NumberOfItems = statusReport.NumberOfIems();

                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithMetadata(Good), statusReport.NumberOfGeodatalovDatasetsWithMetadata(Useable), statusReport.NumberOfGeodatalovDatasetsWithMetadata(Deficient), statusReport.NumberOfGeodatalovDatasetsWithMetadata(Notset));
                NumberOfItemsWithProductSpecification = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithProductSpecification(Good), statusReport.NumberOfGeodatalovDatasetsWithProductSpecification(Useable), statusReport.NumberOfGeodatalovDatasetsWithProductSpecification(Deficient), statusReport.NumberOfGeodatalovDatasetsWithProductSpecification(Notset));
                NumberOfItemsWithSosiRequirements = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Good), statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Useable), statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Deficient), statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Notset));
                NumberOfItemsWithGmlRequirements = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Good), statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Useable), statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Deficient), statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Notset));
                NumberOfItemsWithWms = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithWms(Good), statusReport.NumberOfGeodatalovDatasetsWithWms(Useable), statusReport.NumberOfGeodatalovDatasetsWithWms(Deficient), statusReport.NumberOfGeodatalovDatasetsWithWms(Notset));
                NumberOfItemsWithWfs = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithWfs(Good), statusReport.NumberOfGeodatalovDatasetsWithWfs(Useable), statusReport.NumberOfGeodatalovDatasetsWithWfs(Deficient), statusReport.NumberOfGeodatalovDatasetsWithWfs(Notset));
                NumberOfItemsWithAtomFeed = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Good), statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Useable), statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Deficient), statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Notset));
                NumberOfItemsWithCommon = new NumberOfStatuses(statusReport.NumberOfItemsWithCommon(Good), statusReport.NumberOfItemsWithCommon(Useable), statusReport.NumberOfItemsWithCommon(Deficient), statusReport.NumberOfItemsWithCommon(Notset));

                NumberOfItemsWithInspireTheme = statusReport.NumberOfGeodatalovDatasetsWithInspireTheme();
                NumberOfItemsWithDok = statusReport.NumberOfGeodatalovDatasetsWithDok();
                NumberOfItemsWithNationalDataset = statusReport.NumberOfGeodatalovDatasetsWithNationalDataset();
                NumberOfItemsWithPlan = statusReport.NumberOfGeodatalovDatasetsWithPlan();
                NumberOfItemsWithGeodatalov = statusReport.NumberOfGeodatalovDatasetsWithGeodatalov();
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

        public int NumberOfItemsWithoutInspireTheme()
        {
            return NumberOfItems - NumberOfItemsWithInspireTheme;
        }

        public int NumberOfItemsWithoutDok()
        {
            return NumberOfItems - NumberOfItemsWithDok;
        }

        public int NumberOfItemsWithoutNationalDataset()
        {
            return NumberOfItems - NumberOfItemsWithNationalDataset;
        }

        public int NumberOfItemsWithoutPlan()
        {
            return NumberOfItems - NumberOfItemsWithPlan;
        }

        public int NumberOfItemsWithoutGeodatalov()
        {
            return NumberOfItems - NumberOfItemsWithGeodatalov;
        }
    }
}