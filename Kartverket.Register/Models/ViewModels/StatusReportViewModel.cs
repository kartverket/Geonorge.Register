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
    public class StatusReportViewModel
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public DateTime Date { get; set; }
        public int NumberOfItems { get; set; }
        public SelectList DokReportsSelectList { get; set; }
        public SelectList StatusTypeSelectList { get; set; }
        public bool ReportNotExists { get; set; }
        public DokHistoricalChart DokHistoricalChart { get; set; }
        public StatusChart StatusChart { get; set; }

        // Metadata
        [Display(Name = "DOK_Delivery_Metadata", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithMetadataGood { get; set; }
        public int NumberOfItemsWithMetadataNotSet { get; set; }
        public int NumberOfItemsWithMetadataDeficient { get; set; }
        public int NumberOfItemsWithMetadataUseable { get; set; }

        public DokPieChart DokMetadataPieChart { get; set; }

        // ProductSheet
        [Display(Name = "DOK_ProductSheetStatus", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithProductsheetGood { get; set; }
        public int NumberOfItemsWithProductsheetNotSet { get; set; }
        public int NumberOfItemsWithProductsheetDeficient { get; set; }
        public int NumberOfItemsWithProductsheetUseable { get; set; }

        public DokPieChart DokProductSheetPieChart { get; set; }


        // PresentationRules
        [Display(Name = "DOK_PresentationRulesStatus", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithPresentationRulesGood { get; set; }
        public int NumberOfItemsWithPresentationRulesNotSet { get; set; }
        public int NumberOfItemsWithPresentationRulesDeficient { get; set; }
        public int NumberOfItemsWithPresentationRulesUseable { get; set; }

        public DokPieChart DokPresentationRulesPieChart { get; set; }

        // ProductSpecification
        [Display(Name = "DOK_ProductSpecificationStatus", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithProductSpecificationGood { get; set; }
        public int NumberOfItemsWithProductSpecificationNotSet { get; set; }
        public int NumberOfItemsWithProductSpecificationDeficient { get; set; }
        public int NumberOfItemsWithProductSpecificationUseable { get; set; }

        public DokPieChart DokProductSpecificationPieChart { get; set; }

        // Wms
        [Display(Name = "DOK_Delivery_Wms", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithWmsGood { get; set; }
        public int NumberOfItemsWithWmsNotSet { get; set; }
        public int NumberOfItemsWithWmsDeficient { get; set; }
        public int NumberOfItemsWithWmsUseable { get; set; }

        public DokPieChart DokWmsPieChart { get; set; }

        // Wfs
        [Display(Name = "DOK_Delivery_Wfs", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithWfsGood { get; set; }
        public int NumberOfItemsWithWfsNotSet { get; set; }
        public int NumberOfItemsWithWfsDeficient { get; set; }
        public int NumberOfItemsWithWfsUseable { get; set; }

        public DokPieChart DokWfsPieChart { get; set; }

        // SosiRequirements
        [Display(Name = "DOK_Delivery_SosiRequirements", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithSosiRequirementsGood { get; set; }
        public int NumberOfItemsWithSosiRequirementsNotSet { get; set; }
        public int NumberOfItemsWithSosiRequirementsDeficient { get; set; }
        public int NumberOfItemsWithSosiRequirementsUseable { get; set; }

        public DokPieChart DokSosiRequirementsPieChart { get; set; }

        // GmlRequirements
        [Display(Name = "DOK_Delivery_GmlRequirements", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithGmlRequirementsGood { get; set; }
        public int NumberOfItemsWithGmlRequirementsNotSet { get; set; }
        public int NumberOfItemsWithGmlRequirementsDeficient { get; set; }
        public int NumberOfItemsWithGmlRequirementsUseable { get; set; }

        public DokPieChart DokGmlRequirementsPieChart { get; set; }

        // AtomFeed
        [Display(Name = "DOK_Delivery_AtomFeed", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithAtomFeedGood { get; set; }
        public int NumberOfItemsWithAtomFeedNotSet { get; set; }
        public int NumberOfItemsWithAtomFeedDeficient { get; set; }
        public int NumberOfItemsWithAtomFeedUseable { get; set; }

        public DokPieChart DokAtomFeedPieChart { get; set; }

        // Distribution
        [Display(Name = "DOK_Delivery_Distribution", ResourceType = typeof(DataSet))]
        public int NumberOfItemsWithDistributionGood { get; set; }
        public int NumberOfItemsWithDistributionNotSet { get; set; }
        public int NumberOfItemsWithDistributionDeficient { get; set; }
        public int NumberOfItemsWithDistributionUseable { get; set; }

        public DokPieChart DokDistributionPieChart { get; set; }


        public StatusReportViewModel(StatusReport statusReport, List<StatusReport> statusReports, string statusType)
        {
            DokReportsSelectList = CreateSelectList(statusReports);
            StatusTypeSelectList = CreateStatusTypeSelectList();
            DokHistoricalChart = new DokHistoricalChart(statusReports, statusReport, statusType);
            StatusChart = new StatusChart(statusReports, statusReport, statusType);

            if (statusReport != null)
            {
                Date = statusReport.Date;
                NumberOfItems = statusReport.NumberOfIems();

                NumberOfItemsWithMetadataGood = statusReport.NumberOfItemsWithMetadata(Good);
                NumberOfItemsWithMetadataDeficient = statusReport.NumberOfItemsWithMetadata(Deficient);
                NumberOfItemsWithMetadataNotSet = statusReport.NumberOfItemsWithMetadata(Notset);
                NumberOfItemsWithMetadataUseable = statusReport.NumberOfItemsWithMetadata(Useable);

                DokMetadataPieChart = new DokPieChart(NumberOfItemsWithMetadataGood, NumberOfItemsWithMetadataUseable, NumberOfItemsWithMetadataDeficient, NumberOfItemsWithMetadataNotSet);


                NumberOfItemsWithProductsheetGood = statusReport.NumberOfItemsWithProductsheet(Good);
                NumberOfItemsWithProductsheetDeficient = statusReport.NumberOfItemsWithProductsheet(Deficient);
                NumberOfItemsWithProductsheetNotSet = statusReport.NumberOfItemsWithProductsheet(Notset);
                NumberOfItemsWithProductsheetUseable = statusReport.NumberOfItemsWithProductsheet(Useable);

                DokProductSheetPieChart = new DokPieChart(NumberOfItemsWithProductsheetGood, NumberOfItemsWithProductsheetUseable, NumberOfItemsWithProductsheetDeficient, NumberOfItemsWithProductsheetNotSet);

                NumberOfItemsWithPresentationRulesGood = statusReport.NumberOfItemsWithPresentationRules(Good);
                NumberOfItemsWithPresentationRulesDeficient = statusReport.NumberOfItemsWithPresentationRules(Deficient);
                NumberOfItemsWithPresentationRulesNotSet = statusReport.NumberOfItemsWithPresentationRules(Notset);
                NumberOfItemsWithPresentationRulesUseable = statusReport.NumberOfItemsWithPresentationRules(Useable);

                DokPresentationRulesPieChart = new DokPieChart(NumberOfItemsWithPresentationRulesGood, NumberOfItemsWithPresentationRulesUseable, NumberOfItemsWithPresentationRulesDeficient, NumberOfItemsWithPresentationRulesNotSet);

                NumberOfItemsWithProductSpecificationGood = statusReport.NumberOfItemsWithProductSpecification(Good);
                NumberOfItemsWithProductSpecificationDeficient = statusReport.NumberOfItemsWithProductSpecification(Deficient);
                NumberOfItemsWithProductSpecificationNotSet = statusReport.NumberOfItemsWithProductSpecification(Notset);
                NumberOfItemsWithProductSpecificationUseable = statusReport.NumberOfItemsWithProductSpecification(Useable);

                DokProductSpecificationPieChart = new DokPieChart(NumberOfItemsWithProductSpecificationGood, NumberOfItemsWithProductSpecificationUseable, NumberOfItemsWithProductSpecificationDeficient, NumberOfItemsWithProductSpecificationNotSet);


                NumberOfItemsWithWmsGood = statusReport.NumberOfItemsWithWms(Good);
                NumberOfItemsWithWmsDeficient = statusReport.NumberOfItemsWithWms(Deficient);
                NumberOfItemsWithWmsNotSet = statusReport.NumberOfItemsWithWms(Notset);
                NumberOfItemsWithWmsUseable = statusReport.NumberOfItemsWithWms(Useable);

                DokWmsPieChart = new DokPieChart(NumberOfItemsWithWmsGood, NumberOfItemsWithWmsUseable, NumberOfItemsWithWmsDeficient, NumberOfItemsWithWmsNotSet);

                NumberOfItemsWithWfsGood = statusReport.NumberOfItemsWithWfs(Good);
                NumberOfItemsWithWfsDeficient = statusReport.NumberOfItemsWithWfs(Deficient);
                NumberOfItemsWithWfsNotSet = statusReport.NumberOfItemsWithWfs(Notset);
                NumberOfItemsWithWfsUseable = statusReport.NumberOfItemsWithWfs(Useable);

                DokWfsPieChart = new DokPieChart(NumberOfItemsWithWfsGood, NumberOfItemsWithWfsUseable, NumberOfItemsWithWfsDeficient, NumberOfItemsWithWfsNotSet);

                NumberOfItemsWithSosiRequirementsGood = statusReport.NumberOfItemsWithSosiRequirements(Good);
                NumberOfItemsWithSosiRequirementsDeficient = statusReport.NumberOfItemsWithSosiRequirements(Deficient);
                NumberOfItemsWithSosiRequirementsNotSet = statusReport.NumberOfItemsWithSosiRequirements(Notset);
                NumberOfItemsWithSosiRequirementsUseable = statusReport.NumberOfItemsWithSosiRequirements(Useable);

                DokSosiRequirementsPieChart = new DokPieChart(NumberOfItemsWithSosiRequirementsGood, NumberOfItemsWithSosiRequirementsUseable, NumberOfItemsWithSosiRequirementsDeficient, NumberOfItemsWithSosiRequirementsNotSet);

                NumberOfItemsWithGmlRequirementsGood = statusReport.NumberOfItemsWithGmlRequirements(Good);
                NumberOfItemsWithGmlRequirementsDeficient = statusReport.NumberOfItemsWithGmlRequirements(Deficient);
                NumberOfItemsWithGmlRequirementsNotSet = statusReport.NumberOfItemsWithGmlRequirements(Notset);
                NumberOfItemsWithGmlRequirementsUseable = statusReport.NumberOfItemsWithGmlRequirements(Useable);

                DokGmlRequirementsPieChart = new DokPieChart(NumberOfItemsWithGmlRequirementsGood, NumberOfItemsWithGmlRequirementsUseable, NumberOfItemsWithGmlRequirementsDeficient, NumberOfItemsWithGmlRequirementsNotSet);

                NumberOfItemsWithAtomFeedGood = statusReport.NumberOfItemsWithAtomFeed(Good);
                NumberOfItemsWithAtomFeedDeficient = statusReport.NumberOfItemsWithAtomFeed(Deficient);
                NumberOfItemsWithAtomFeedNotSet = statusReport.NumberOfItemsWithAtomFeed(Notset);
                NumberOfItemsWithAtomFeedUseable = statusReport.NumberOfItemsWithAtomFeed(Useable);

                DokAtomFeedPieChart = new DokPieChart(NumberOfItemsWithAtomFeedGood, NumberOfItemsWithAtomFeedUseable, NumberOfItemsWithAtomFeedDeficient, NumberOfItemsWithAtomFeedNotSet);

                NumberOfItemsWithDistributionGood = statusReport.NumberOfItemsWithDistribution(Good);
                NumberOfItemsWithDistributionDeficient = statusReport.NumberOfItemsWithDistribution(Deficient);
                NumberOfItemsWithDistributionNotSet = statusReport.NumberOfItemsWithDistribution(Notset);
                NumberOfItemsWithDistributionUseable = statusReport.NumberOfItemsWithDistribution(Useable);

                DokDistributionPieChart = new DokPieChart(NumberOfItemsWithDistributionGood, NumberOfItemsWithDistributionUseable, NumberOfItemsWithDistributionDeficient, NumberOfItemsWithDistributionNotSet);

            }
            else
            {
                ReportNotExists = true;
            }
        }


        private SelectList CreateSelectList(List<StatusReport> statusReports)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            if (statusReports == null)
            {
                items.Add(new SelectListItem() { Text = "Ingen rapporter", Value = null });
            }
            else if (statusReports.Any())
            {
                foreach (var report in statusReports.OrderByDescending(i => i.Date))
                {
                    items.Add(new SelectListItem() { Text = report.Date.ToString(), Value = report.Id.ToString() });
                }
            }
            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }

        private SelectList CreateStatusTypeSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Metadata, Value = "Metadata" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_ProductSheet, Value = "ProductSheet" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_PresentationRules, Value = "PresentationRules" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_ProductSpesification, Value = "ProductSpecification" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Wms, Value = "Wms" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Wfs, Value = "Wfs" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_SosiRequirements, Value = "SosiRequirements" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_GmlRequirements, Value = "GmlRequirements" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_AtomFeed, Value = "AtomFeed" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Distribution, Value = "Distribution" });
            items.Add(new SelectListItem() { Text = Shared.ShowAll, Value = "all" });

            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }

        public double Percent(int numberOf)
        {
            var x = Divide(numberOf, NumberOfItems);
            return Math.Round(x * 100, 2);
        }

        private double Divide(int x, int y)
        {
            try
            {
                if (y == 0)
                {
                    return 0;
                }
                else
                {
                    return (double)x / y;
                }
            }
            catch (Exception e)
            {

                return 0;
            }
        }
    }
}