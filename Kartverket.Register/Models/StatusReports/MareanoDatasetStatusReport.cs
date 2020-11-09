using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.StatusReports
{
    public class MareanoDatasetStatusReport : RegisterItemStatusReport
    {

        public string UuidMareanoDataset { get; set; }
        public string MetadataMareanoDataset { get; set; }
        public string ProductSpesificationMareanoDataset { get; set; }
        public string SosiDataMareanoDataset { get; set; }
        public string GmlDataMareanoDataset { get; set; }
        public string WmsMareanoDataset { get; set; }
        public string WfsMareanoDataset { get; set; }
        public string AtomFeedMareanoDataset { get; set; }
        public string CommonStatusMareanoDataset { get; set; }


        public MareanoDatasetStatusReport()
        {
        }

        public MareanoDatasetStatusReport(MareanoDataset MareanoDataset)
        {
            if (MareanoDataset != null)
            {
                UuidMareanoDataset = MareanoDataset.Uuid;

                if (MareanoDataset.MetadataStatus != null)
                    MetadataMareanoDataset = MareanoDataset.MetadataStatus.StatusId;
                if (MareanoDataset.ProductSpesificationStatus != null)
                    ProductSpesificationMareanoDataset = MareanoDataset.ProductSpesificationStatus.StatusId;
                if (MareanoDataset.SosiDataStatus != null)
                    SosiDataMareanoDataset = MareanoDataset.SosiDataStatus.StatusId;
                if (MareanoDataset.GmlDataStatus != null)
                    GmlDataMareanoDataset = MareanoDataset.GmlDataStatus.StatusId;
                if (MareanoDataset.WmsStatus != null) WmsMareanoDataset = MareanoDataset.WmsStatus.StatusId;
                if (MareanoDataset.WfsStatus != null) WfsMareanoDataset = MareanoDataset.WfsStatus.StatusId;
                if (MareanoDataset.AtomFeedStatus != null)
                    AtomFeedMareanoDataset = MareanoDataset.AtomFeedStatus.StatusId;
                if (MareanoDataset.CommonStatus != null)
                    CommonStatusMareanoDataset = MareanoDataset.CommonStatus.StatusId;
            }
        }
    }
}