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
            Uuid = inspireDataset.Uuid;
            Metadata = inspireDataset.InspireDeliveryMetadata.StatusId;
            MetadataService = inspireDataset.InspireDeliveryMetadataService.StatusId;
            Distribution = inspireDataset.InspireDeliveryDistribution.StatusId;
            Wms = inspireDataset.InspireDeliveryWms.StatusId;
            Wfs = inspireDataset.InspireDeliveryWfs.StatusId;
            WfsOrAtom = inspireDataset.InspireDeliveryWfsOrAtom.StatusId;
            AtomFeed = inspireDataset.InspireDeliveryAtomFeed.StatusId;
            HarmonizedData = inspireDataset.InspireDeliveryHarmonizedData.StatusId;
            SpatialDataService = inspireDataset.InspireDeliverySpatialDataService.StatusId;
        }

        public virtual InspireDataset InspireDataset { get; set; }
        public string Uuid { get; set; }
        public string Metadata { get; set; }
        public string MetadataService { get; set; }
        public string Distribution { get; set; }
        public string Wms { get; set; }
        public string Wfs { get; set; }
        public string WfsOrAtom { get; set; }
        public string AtomFeed { get; set; }
        public string HarmonizedData { get; set; }
        public string SpatialDataService { get; set; }
    }
}