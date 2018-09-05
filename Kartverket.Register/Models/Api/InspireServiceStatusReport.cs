using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models.ViewModels;
using Resources;

namespace Kartverket.Register.Models.Api
{
    public class InspireServiceStatusReport
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public InspireServiceStatusReport(Models.StatusReport statusReport)
        {
            if (statusReport != null)
            {
                NumberOfInspireServices = statusReport.NumberOfInspireDataServices();
                Date = statusReport.Date;

                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfInspireDataServiceWithMetadata(Good), statusReport.NumberOfInspireDataServiceWithMetadata(Useable), statusReport.NumberOfInspireDataServiceWithMetadata(Deficient), statusReport.NumberOfInspireDataServiceWithMetadata(Notset));
                NumberOfItemsWithMetadataInSearchService = new NumberOfStatuses(statusReport.NumberOfInspireDataServicesWithMetadataService(Good), statusReport.NumberOfInspireDataServicesWithMetadataService(Useable), statusReport.NumberOfInspireDataServicesWithMetadataService(Deficient), statusReport.NumberOfInspireDataServicesWithMetadataService(Notset));
                NumberOfItemsWithServiceStatus = new NumberOfStatuses(statusReport.NumberOfItemsWithServiceStatus(Good), statusReport.NumberOfItemsWithServiceStatus(Useable), statusReport.NumberOfItemsWithServiceStatus(Deficient), statusReport.NumberOfItemsWithServiceStatus(Notset));
                NumberOfItemsWithSds = statusReport.NumberOfInspireDataServiceWithSds();
                NumberOfItemsWithNetworkService = statusReport.NumberOfInspireDataServiceWithNetworkService();
            }
        }

        public int NumberOfInspireServices { get; set; }
        public DateTime Date { get; set; }

        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }
        public NumberOfStatuses NumberOfItemsWithMetadataInSearchService { get; set; }
        public NumberOfStatuses NumberOfItemsWithServiceStatus { get; set; }
        public int NumberOfItemsWithSds { get; set; }
        public int NumberOfItemsWithNetworkService { get; set; }
    }
}