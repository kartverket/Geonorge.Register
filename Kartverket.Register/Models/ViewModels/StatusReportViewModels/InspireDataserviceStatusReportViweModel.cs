using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels.StatusReportViewModels;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireDataserviceStatusReportViweModel
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public int NumberOfInspireDataservices { get; set; }
        public SelectList StatusTypeSelectList { get; set; }

        public InspireDataServiceLineChart LineChart { get; set; }

        // Metadata
        [Display(Name = "Metadata", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }

        //Metadata in search service
        [Display(Name = "MetadataServiceStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithMetadataInSearchService { get; set; }

        //Service status
        [Display(Name = "ServiceStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithServiceStatus { get; set; }

        //Spatial data service
        [Display(Name = "SpatialDataServiceStatus", ResourceType = typeof(InspireDataSet))]
        public int NumberOfItemsWithSds { get; set; }

        //Network service
        [Display(Name = "NetworkService", ResourceType = typeof(InspireDataSet))]
        public int NumberOfItemsWithNetworkService { get; set; }


        public InspireDataserviceStatusReportViweModel(StatusReport statusReport, List<StatusReport> statusReports)
        {
            StatusTypeSelectList = CreateStatusTypeSelectList();
            LineChart = new InspireDataServiceLineChart(statusReports, statusReport);
            if (statusReport != null)
            {
                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfInspireDataServiceWithMetadata(Good), statusReport.NumberOfInspireDataServiceWithMetadata(Useable), statusReport.NumberOfInspireDataServiceWithMetadata(Deficient), statusReport.NumberOfInspireDataServiceWithMetadata(Notset));
                NumberOfItemsWithMetadataInSearchService = new NumberOfStatuses(statusReport.NumberOfInspireDataServicesWithMetadataService(Good), statusReport.NumberOfInspireDataServicesWithMetadataService(Useable), statusReport.NumberOfInspireDataServicesWithMetadataService(Deficient), statusReport.NumberOfInspireDataServicesWithMetadataService(Notset));
                NumberOfItemsWithServiceStatus = new NumberOfStatuses(statusReport.NumberOfItemsWithServiceStatus(Good), statusReport.NumberOfItemsWithServiceStatus(Useable), statusReport.NumberOfItemsWithServiceStatus(Deficient), statusReport.NumberOfItemsWithServiceStatus(Notset));
                NumberOfItemsWithSds = statusReport.NumberOfInspireDataServiceWithSds() ;
                NumberOfItemsWithNetworkService = statusReport.NumberOfInspireDataServiceWithNetworkService();
                NumberOfInspireDataservices = statusReport.NumberOfInspireDataServices();
            }
        }

        private SelectList CreateStatusTypeSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Text = Shared.ShowAll, Value = "all" });
            items.Add(new SelectListItem() { Text = InspireDataSet.Metadata, Value = "MetadataInspireDataService" });
            items.Add(new SelectListItem() { Text = InspireDataSet.MetadataServiceStatus, Value = "MetadataSearchServiceStatus" });
            items.Add(new SelectListItem() { Text = InspireDataSet.ServiceStatus, Value = "DistributionInspireDataService" });
            items.Add(new SelectListItem() { Text = InspireDataSet.SpatialDataServiceStatus, Value = "Sds" });
            items.Add(new SelectListItem() { Text = InspireDataSet.NetworkService, Value = "NetworkService" });

            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }


        /// <summary>
        /// Percent of inspire datasets
        /// </summary>
        /// <param name="numberOf"></param>
        /// <returns></returns>
        public double Percent(int numberOf)
        {
            return HtmlHelperExtensions.Percent(numberOf, NumberOfInspireDataservices);
        }

        public int NumberOfItemsWithoutSds()
        {
            return NumberOfInspireDataservices - NumberOfItemsWithSds;
        }

        public int NumberOfItemsWithoutNetworkService()
        {
            return NumberOfInspireDataservices - NumberOfItemsWithNetworkService;
        }
    }
}