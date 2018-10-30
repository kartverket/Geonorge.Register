using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.StatusReports
{
    public class GeodatalovDatasetStatusReport : RegisterItemStatusReport
    {

        public string UuidGeodatalovDataset { get; set; }
        public string MetadataGeodatalovDataset { get; set; }
        public string ProductSpesificationGeodatalovDataset { get; set; }
        public string SosiDataGeodatalovDataset { get; set; }
        public string GmlDataGeodatalovDataset { get; set; }
        public string WmsGeodatalovDataset { get; set; }
        public string WfsGeodatalovDataset { get; set; }
        public string AtomFeedGeodatalovDataset { get; set; }
        public string CommonStatusGeodatalovDataset { get; set; }

        public bool InspireTheme { get; set; }
        public bool Dok { get; set; }
        public bool NationalDataset { get; set; }
        public bool Plan { get; set; }
        public bool Geodatalov { get; set; }


        public GeodatalovDatasetStatusReport()
        {
        }

        public GeodatalovDatasetStatusReport(GeodatalovDataset geodatalovDataset)
        {
            if (geodatalovDataset != null)
            {
                UuidGeodatalovDataset = geodatalovDataset.Uuid;
                InspireTheme = geodatalovDataset.InspireTheme;
                Dok = geodatalovDataset.Dok;
                NationalDataset = geodatalovDataset.NationalDataset;
                Plan = geodatalovDataset.Plan;
                Geodatalov = geodatalovDataset.Geodatalov;

                if (geodatalovDataset.MetadataStatus != null)
                    MetadataGeodatalovDataset = geodatalovDataset.MetadataStatus.StatusId;
                if (geodatalovDataset.ProductSpesificationStatus != null)
                    ProductSpesificationGeodatalovDataset = geodatalovDataset.ProductSpesificationStatus.StatusId;
                if (geodatalovDataset.SosiDataStatus != null)
                    SosiDataGeodatalovDataset = geodatalovDataset.SosiDataStatus.StatusId;
                if (geodatalovDataset.GmlDataStatus != null)
                    GmlDataGeodatalovDataset = geodatalovDataset.GmlDataStatus.StatusId;
                if (geodatalovDataset.WmsStatus != null) WmsGeodatalovDataset = geodatalovDataset.WmsStatus.StatusId;
                if (geodatalovDataset.WfsStatus != null) WfsGeodatalovDataset = geodatalovDataset.WfsStatus.StatusId;
                if (geodatalovDataset.AtomFeedStatus != null)
                    AtomFeedGeodatalovDataset = geodatalovDataset.AtomFeedStatus.StatusId;
                if (geodatalovDataset.CommonStatus != null)
                    CommonStatusGeodatalovDataset = geodatalovDataset.CommonStatus.StatusId;
            }
        }
    }
}