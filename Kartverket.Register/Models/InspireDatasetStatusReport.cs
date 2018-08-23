using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class InspireDatasetStatusReport : RegisterItemStatusReport
    {
        public InspireDatasetStatusReport()
        {
        }

        public InspireDatasetStatusReport(InspireDataset inspireDataset)
        {
            InspireDataset = inspireDataset;
            UuidInspireDataset = inspireDataset.Uuid;
            MetadataInspireDataset = inspireDataset.InspireDeliveryMetadata.StatusId;
            MetadataServiceInspireDataset = inspireDataset.InspireDeliveryMetadataService.StatusId;
            DistributionInspireDataset = inspireDataset.InspireDeliveryDistribution.StatusId;
            WmsInspireDataset = inspireDataset.InspireDeliveryWms.StatusId;
            WfsInspireDataset = inspireDataset.InspireDeliveryWfs.StatusId;
            WfsOrAtomInspireDataset = inspireDataset.InspireDeliveryWfsOrAtom.StatusId;
            AtomFeedInspireDataset = inspireDataset.InspireDeliveryAtomFeed.StatusId;
            HarmonizedDataInspireDataset = inspireDataset.InspireDeliveryHarmonizedData.StatusId;
            SpatialDataServiceInspireDataset = inspireDataset.InspireDeliverySpatialDataService.StatusId;
        }

        public virtual InspireDataset InspireDataset { get; set; }
        public string UuidInspireDataset { get; set; }
        public string MetadataInspireDataset { get; set; }
        public string MetadataServiceInspireDataset { get; set; }
        public string DistributionInspireDataset { get; set; }
        public string WmsInspireDataset { get; set; }
        public string WfsInspireDataset { get; set; }
        public string WfsOrAtomInspireDataset { get; set; }
        public string AtomFeedInspireDataset { get; set; }
        public string HarmonizedDataInspireDataset { get; set; }
        public string SpatialDataServiceInspireDataset { get; set; }
    }
}