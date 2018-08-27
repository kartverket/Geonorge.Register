using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireDatasetStatusReportViweModel
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public int NumberOfInspireDatasets { get; set; }

        // Metadata
        [Display(Name = "Metadata", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }

        //Metadat service
        [Display(Name = "MetadataServiceStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithMetadataService { get; set; }

        //Distribution
        [Display(Name = "Distribution", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithDistribution { get; set; }

        //View service (Visningstjeneste)
        [Display(Name = "WmsStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithWms { get; set; }

        //Wfs
        [Display(Name = "WfsStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithWfs { get; set; }

        //AtomFeed
        [Display(Name = "AtomFeedStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithAtomFeed { get; set; }

        //Atom or WFS
        [Display(Name = "WfsOrAtomStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithWfsOrAtom { get; set; }

        //Harmonized data
        [Display(Name = "HarmonizedDataStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithHarmonizedData { get; set; }

        //Spatial data service
        [Display(Name = "SpatialDataServiceStatus", ResourceType = typeof(InspireDataSet))]
        public NumberOfStatuses NumberOfItemsWithSpatialDataService { get; set; }


        public InspireDatasetStatusReportViweModel(StatusReport statusReport)
        {
            if (statusReport != null)
            {
                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfInspireDatasetsItemsWithMetadata(Good), statusReport.NumberOfInspireDatasetsItemsWithMetadata(Useable), statusReport.NumberOfInspireDatasetsItemsWithMetadata(Deficient), statusReport.NumberOfInspireDatasetsItemsWithMetadata(Notset));
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