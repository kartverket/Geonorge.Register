using System;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Models.Api
{
    public class InspireDataSetStatusReport
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public int NumberOfInspireDatasets { get; set; }
        public DateTime Date { get; set; }

        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }
        public NumberOfStatuses NumberOfItemsWithMetadataService { get; set; }
        public NumberOfStatuses NumberOfItemsWithDistribution { get; set; }
        public NumberOfStatuses NumberOfItemsWithWms { get; set; }
        public NumberOfStatuses NumberOfItemsWithWfs { get; set; }
        public NumberOfStatuses NumberOfItemsWithAtomFeed { get; set; }
        public NumberOfStatuses NumberOfItemsWithWfsOrAtom { get; set; }
        public NumberOfStatuses NumberOfItemsWithHarmonizedData { get; set; }
        public NumberOfStatuses NumberOfItemsWithSpatialDataService { get; set; }

        public InspireDataSetStatusReport(Models.StatusReport statusReport)
        {
            if (statusReport.IsInspireDatasetReport())
            {
                NumberOfInspireDatasets = statusReport.NumberOfInspireDataServices();
                Date = statusReport.Date;

                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfInspireDatasetsWithMetadata(Good), statusReport.NumberOfInspireDatasetsWithMetadata(Useable), statusReport.NumberOfInspireDatasetsWithMetadata(Deficient), statusReport.NumberOfInspireDatasetsWithMetadata(Notset));
                NumberOfItemsWithMetadataService = new NumberOfStatuses(statusReport.NumberOfItemsWithMetadataService(Good), statusReport.NumberOfItemsWithMetadataService(Useable), statusReport.NumberOfItemsWithMetadataService(Deficient), statusReport.NumberOfItemsWithMetadataService(Notset));
                NumberOfItemsWithDistribution = new NumberOfStatuses(statusReport.NumberOfInspireDatasetsWithDistribution(Good), statusReport.NumberOfInspireDatasetsWithDistribution(Useable), statusReport.NumberOfInspireDatasetsWithDistribution(Deficient), statusReport.NumberOfInspireDatasetsWithDistribution(Notset));
                NumberOfItemsWithWms = new NumberOfStatuses(statusReport.NumberOfInspireDatasetsWithWms(Good), statusReport.NumberOfInspireDatasetsWithWms(Useable), statusReport.NumberOfInspireDatasetsWithWms(Deficient), statusReport.NumberOfInspireDatasetsWithWms(Notset));
                NumberOfItemsWithWfs = new NumberOfStatuses(statusReport.NumberOfInspireDatasetsWithWfs(Good), statusReport.NumberOfInspireDatasetsWithWfs(Useable), statusReport.NumberOfInspireDatasetsWithWfs(Deficient), statusReport.NumberOfInspireDatasetsWithWfs(Notset));
                NumberOfItemsWithAtomFeed = new NumberOfStatuses(statusReport.NumberOfInspireDatasetsWithAtomFeed(Good), statusReport.NumberOfInspireDatasetsWithAtomFeed(Useable), statusReport.NumberOfInspireDatasetsWithAtomFeed(Deficient), statusReport.NumberOfInspireDatasetsWithAtomFeed(Notset));
                NumberOfItemsWithWfsOrAtom = new NumberOfStatuses(statusReport.NumberOfItemsWithWfsOrAtom(Good), statusReport.NumberOfItemsWithWfsOrAtom(Useable), statusReport.NumberOfItemsWithWfsOrAtom(Deficient), statusReport.NumberOfItemsWithWfsOrAtom(Notset));
                NumberOfItemsWithHarmonizedData = new NumberOfStatuses(statusReport.NumberOfItemsWithHarmonizedData(Good), statusReport.NumberOfItemsWithHarmonizedData(Useable), statusReport.NumberOfItemsWithHarmonizedData(Deficient), statusReport.NumberOfItemsWithHarmonizedData(Notset));
                NumberOfItemsWithSpatialDataService = new NumberOfStatuses(statusReport.NumberOfItemsWithSpatialDataService(Good), statusReport.NumberOfItemsWithSpatialDataService(Useable), statusReport.NumberOfItemsWithSpatialDataService(Deficient), statusReport.NumberOfItemsWithSpatialDataService(Notset));
                NumberOfInspireDatasets = statusReport.NumberOfInspireDatasets();
            }
        }
    }
}