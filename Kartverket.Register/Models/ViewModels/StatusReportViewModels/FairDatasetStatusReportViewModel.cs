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
    public class FairDatasetStatusReportViewModel : StatusReportViewModel
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";
        private const string Satisfactory = "satisfactory";

        public FairLineChart FairLineChart { get; set; }
        public StatusLineChart StatusChart { get; set; }

        // Findable
        [Display(Name = "Findable_Label", ResourceType = typeof(FairDataSet))]
        public NumberOfStatuses NumberOfItemsWithFindable { get; set; }

        // Accesible
        [Display(Name = "Accesible_Label", ResourceType = typeof(FairDataSet))]
        public NumberOfStatuses NumberOfItemsWithAccesible { get; set; }

        // Interoperable
        [Display(Name = "Interoperable_Label", ResourceType = typeof(FairDataSet))]
        public NumberOfStatuses NumberOfItemsWithInteroperable { get; set; }

        // ReUsable
        [Display(Name = "ReUseable_Label", ResourceType = typeof(FairDataSet))]
        public NumberOfStatuses NumberOfItemsWithReUseable { get; set; }

        // Metadata
        [Display(Name = "Metadata", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }

        // Productspecification
        [Display(Name = "DOK_ProductSheetStatus", ResourceType = typeof(DataSet))]
        public NumberOfStatuses NumberOfItemsWithProductSheet { get; set; }

        // Productspecification
        [Display(Name = "DOK_PresentationRulesStatus", ResourceType = typeof(DataSet))]
        public NumberOfStatuses NumberOfItemsWithPresentationRules { get; set; }


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
        [Display(Name = "Common", ResourceType = typeof(FairDataSet))]
        public NumberOfStatuses NumberOfItemsWithCommon { get; set; }

        public double FindableStatusPerCent { get; set; }
        public double AccessibleStatusPerCent { get; set; }
        public double InteroperableStatusPerCent { get; set; }
        public double ReUseableStatusPerCent { get; set; }

        public FairDatasetStatusReportViewModel(StatusReport statusReport, List<StatusReport> statusReports, string statusType)
        {
            ReportsSelectList = CreateSelectList(statusReports);
            StatusTypeSelectList = CreateStatusTypeSelectList();
            FairLineChart = new FairLineChart(statusReports, statusReport, statusType);
            StatusChart = new StatusLineChart(statusReports, statusReport, statusType);

            if (statusReport != null)
            {
                ReportNotExists = false;
                Id = statusReport.Id;
                Date = statusReport.Date;
                NumberOfItems = statusReport.NumberOfIems();

                NumberOfItemsWithFindable = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithFindable(Good), statusReport.NumberOfFairDatasetsWithFindable(Useable), statusReport.NumberOfFairDatasetsWithFindable(Deficient), statusReport.NumberOfFairDatasetsWithFindable(Notset), statusReport.NumberOfFairDatasetsWithFindable(Satisfactory));
                NumberOfItemsWithAccesible = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithAccesible(Good), statusReport.NumberOfFairDatasetsWithAccesible(Useable), statusReport.NumberOfFairDatasetsWithAccesible(Deficient), statusReport.NumberOfFairDatasetsWithAccesible(Notset), statusReport.NumberOfFairDatasetsWithAccesible(Satisfactory));
                NumberOfItemsWithInteroperable = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithInteroperable(Good), statusReport.NumberOfFairDatasetsWithInteroperable(Useable), statusReport.NumberOfFairDatasetsWithInteroperable(Deficient), statusReport.NumberOfFairDatasetsWithInteroperable(Notset), statusReport.NumberOfFairDatasetsWithInteroperable(Satisfactory));
                NumberOfItemsWithReUseable = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithReUsable(Good), statusReport.NumberOfFairDatasetsWithReUsable(Useable), statusReport.NumberOfFairDatasetsWithReUsable(Deficient), statusReport.NumberOfFairDatasetsWithReUsable(Notset), statusReport.NumberOfFairDatasetsWithReUsable(Satisfactory));

                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithMetadata(Good), statusReport.NumberOfFairDatasetsWithMetadata(Useable), statusReport.NumberOfFairDatasetsWithMetadata(Deficient), statusReport.NumberOfFairDatasetsWithMetadata(Notset));
                NumberOfItemsWithProductSheet = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithProductSheet(Good), statusReport.NumberOfFairDatasetsWithProductSheet(Useable), statusReport.NumberOfFairDatasetsWithProductSheet(Deficient), statusReport.NumberOfFairDatasetsWithProductSheet(Notset));
                NumberOfItemsWithPresentationRules = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithPresentationRules(Good), statusReport.NumberOfFairDatasetsWithPresentationRules(Useable), statusReport.NumberOfFairDatasetsWithPresentationRules(Deficient), statusReport.NumberOfFairDatasetsWithPresentationRules(Notset));
                NumberOfItemsWithProductSpecification = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithProductSpecification(Good), statusReport.NumberOfFairDatasetsWithProductSpecification(Useable), statusReport.NumberOfFairDatasetsWithProductSpecification(Deficient), statusReport.NumberOfFairDatasetsWithProductSpecification(Notset));
                NumberOfItemsWithSosiRequirements = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithSosiRequirements(Good), statusReport.NumberOfFairDatasetsWithSosiRequirements(Useable), statusReport.NumberOfFairDatasetsWithSosiRequirements(Deficient), statusReport.NumberOfFairDatasetsWithSosiRequirements(Notset));
                NumberOfItemsWithGmlRequirements = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithGmlRequirements(Good), statusReport.NumberOfFairDatasetsWithGmlRequirements(Useable), statusReport.NumberOfFairDatasetsWithGmlRequirements(Deficient), statusReport.NumberOfFairDatasetsWithGmlRequirements(Notset));
                NumberOfItemsWithWms = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithWms(Good), statusReport.NumberOfFairDatasetsWithWms(Useable), statusReport.NumberOfFairDatasetsWithWms(Deficient), statusReport.NumberOfFairDatasetsWithWms(Notset));
                NumberOfItemsWithWfs = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithWfs(Good), statusReport.NumberOfFairDatasetsWithWfs(Useable), statusReport.NumberOfFairDatasetsWithWfs(Deficient), statusReport.NumberOfFairDatasetsWithWfs(Notset));
                NumberOfItemsWithAtomFeed = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithAtomFeed(Good), statusReport.NumberOfFairDatasetsWithAtomFeed(Useable), statusReport.NumberOfFairDatasetsWithAtomFeed(Deficient), statusReport.NumberOfFairDatasetsWithAtomFeed(Notset));
                NumberOfItemsWithCommon = new NumberOfStatuses(statusReport.NumberOfItemsWithCommon(Good), statusReport.NumberOfItemsWithCommon(Useable), statusReport.NumberOfItemsWithCommon(Deficient), statusReport.NumberOfItemsWithCommon(Notset));

                FindableStatusPerCent = Math.Round(statusReport.FairDatasetsFindablePercent(),2);
                AccessibleStatusPerCent = Math.Round(statusReport.FairDatasetsAccessiblePercent(),2);
                InteroperableStatusPerCent = Math.Round(statusReport.FairDatasetsInteroperablePercent(),2);
                ReUseableStatusPerCent = Math.Round(statusReport.FairDatasetsReuseablePercent(), 2);


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
            items.Add(new SelectListItem() { Text = FairDataSet.Findable_Label, Value = "Findable" });
            items.Add(new SelectListItem() { Text = FairDataSet.Accesible_Label, Value = "Accesible" });
            items.Add(new SelectListItem() { Text = FairDataSet.Interoperable_Label, Value = "Interoperable" });
            items.Add(new SelectListItem() { Text = FairDataSet.ReUseable_Label, Value = "ReUseable" });

            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }
    }
}