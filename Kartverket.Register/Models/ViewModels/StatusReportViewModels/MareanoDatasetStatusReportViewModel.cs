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
    public class MareanoDatasetStatusReportViewModel : StatusReportViewModel
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";
        private const string Satisfactory = "satisfactory";

        public MareanoLineChart MareanoLineChart { get; set; }
        public StatusLineChart StatusChart { get; set; }

        // Findable
        [Display(Name = "Findable_Label", ResourceType = typeof(MareanoDataSet))]
        public NumberOfStatuses NumberOfItemsWithFindable { get; set; }

        // Accesible
        [Display(Name = "Accesible_Label", ResourceType = typeof(MareanoDataSet))]
        public NumberOfStatuses NumberOfItemsWithAccesible { get; set; }

        // Interoperable
        [Display(Name = "Interoperable_Label", ResourceType = typeof(MareanoDataSet))]
        public NumberOfStatuses NumberOfItemsWithInteroperable { get; set; }

        // ReUsable
        [Display(Name = "ReUseable_Label", ResourceType = typeof(MareanoDataSet))]
        public NumberOfStatuses NumberOfItemsWithReUseable { get; set; }

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
        [Display(Name = "Common", ResourceType = typeof(MareanoDataSet))]
        public NumberOfStatuses NumberOfItemsWithCommon { get; set; }


        public MareanoDatasetStatusReportViewModel(StatusReport statusReport, List<StatusReport> statusReports, string statusType)
        {
            ReportsSelectList = CreateSelectList(statusReports);
            StatusTypeSelectList = CreateStatusTypeSelectList();  // TODO
            MareanoLineChart = new MareanoLineChart(statusReports, statusReport, statusType);
            StatusChart = new StatusLineChart(statusReports, statusReport, statusType);

            if (statusReport != null)
            {
                ReportNotExists = false;
                Id = statusReport.Id;
                Date = statusReport.Date;
                NumberOfItems = statusReport.NumberOfIems();

                NumberOfItemsWithFindable = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithFindable(Good), statusReport.NumberOfMareanoDatasetsWithFindable(Useable), statusReport.NumberOfMareanoDatasetsWithFindable(Deficient), statusReport.NumberOfMareanoDatasetsWithFindable(Notset), statusReport.NumberOfMareanoDatasetsWithFindable(Satisfactory));
                NumberOfItemsWithAccesible = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithAccesible(Good), statusReport.NumberOfMareanoDatasetsWithAccesible(Useable), statusReport.NumberOfMareanoDatasetsWithAccesible(Deficient), statusReport.NumberOfMareanoDatasetsWithAccesible(Notset), statusReport.NumberOfMareanoDatasetsWithAccesible(Satisfactory));
                NumberOfItemsWithInteroperable = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithInteroperable(Good), statusReport.NumberOfMareanoDatasetsWithInteroperable(Useable), statusReport.NumberOfMareanoDatasetsWithInteroperable(Deficient), statusReport.NumberOfMareanoDatasetsWithInteroperable(Notset), statusReport.NumberOfMareanoDatasetsWithInteroperable(Satisfactory));
                NumberOfItemsWithReUseable = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithReUsable(Good), statusReport.NumberOfMareanoDatasetsWithReUsable(Useable), statusReport.NumberOfMareanoDatasetsWithReUsable(Deficient), statusReport.NumberOfMareanoDatasetsWithReUsable(Notset), statusReport.NumberOfMareanoDatasetsWithReUsable(Satisfactory));

                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithMetadata(Good), statusReport.NumberOfMareanoDatasetsWithMetadata(Useable), statusReport.NumberOfMareanoDatasetsWithMetadata(Deficient), statusReport.NumberOfMareanoDatasetsWithMetadata(Notset));
                NumberOfItemsWithProductSpecification = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithProductSpecification(Good), statusReport.NumberOfMareanoDatasetsWithProductSpecification(Useable), statusReport.NumberOfMareanoDatasetsWithProductSpecification(Deficient), statusReport.NumberOfMareanoDatasetsWithProductSpecification(Notset));
                NumberOfItemsWithSosiRequirements = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Good), statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Useable), statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Deficient), statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Notset));
                NumberOfItemsWithGmlRequirements = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Good), statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Useable), statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Deficient), statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Notset));
                NumberOfItemsWithWms = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithWms(Good), statusReport.NumberOfMareanoDatasetsWithWms(Useable), statusReport.NumberOfMareanoDatasetsWithWms(Deficient), statusReport.NumberOfMareanoDatasetsWithWms(Notset));
                NumberOfItemsWithWfs = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithWfs(Good), statusReport.NumberOfMareanoDatasetsWithWfs(Useable), statusReport.NumberOfMareanoDatasetsWithWfs(Deficient), statusReport.NumberOfMareanoDatasetsWithWfs(Notset));
                NumberOfItemsWithAtomFeed = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithAtomFeed(Good), statusReport.NumberOfMareanoDatasetsWithAtomFeed(Useable), statusReport.NumberOfMareanoDatasetsWithAtomFeed(Deficient), statusReport.NumberOfMareanoDatasetsWithAtomFeed(Notset));
                NumberOfItemsWithCommon = new NumberOfStatuses(statusReport.NumberOfItemsWithCommon(Good), statusReport.NumberOfItemsWithCommon(Useable), statusReport.NumberOfItemsWithCommon(Deficient), statusReport.NumberOfItemsWithCommon(Notset));

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
            items.Add(new SelectListItem() { Text = MareanoDataSet.Findable_Label, Value = "Findable" });
            items.Add(new SelectListItem() { Text = MareanoDataSet.Accesible_Label, Value = "Accesible" });
            items.Add(new SelectListItem() { Text = MareanoDataSet.Interoperable_Label, Value = "Interoperable" });
            items.Add(new SelectListItem() { Text = MareanoDataSet.ReUseable_Label, Value = "ReUseable" });
            items.Add(new SelectListItem() { Text = InspireDataSet.Metadata, Value = "Metadata" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_ProductSpecificationStatus, Value = "ProductSpecification" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_SosiRequirements, Value = "SosiRequirements" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_GmlRequirements, Value = "GmlRequirements" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Wms, Value = "Wms" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Wfs, Value = "Wfs" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_AtomFeed, Value = "AtomFeed" });
            items.Add(new SelectListItem() { Text = MareanoDataSet.Common, Value = "Common" });

            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }
    }
}