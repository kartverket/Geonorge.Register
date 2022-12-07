using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.StatusReports
{
    public class MareanoDatasetStatusReport : RegisterItemStatusReport
    {

        public string UuidMareanoDataset { get; set; }
        public string FindableMareanoDataset { get; set; }
        public string AccesibleMareanoDataset { get; set; }
        public string InteroperableMareanoDataset { get; set; }
        public string MetadataMareanoDataset { get; set; }
        public string ReUsableMareanoDataset { get; set; }
        public string ProductSpesificationMareanoDataset { get; set; }
        public string ProductSheetMareanoDataset { get; set; }
        public string PresentationRulesMareanoDataset { get; set; }
        public string SosiDataMareanoDataset { get; set; }
        public string GmlDataMareanoDataset { get; set; }
        public string WmsMareanoDataset { get; set; }
        public string WfsMareanoDataset { get; set; }
        public string AtomFeedMareanoDataset { get; set; }
        public string CommonStatusMareanoDataset { get; set; }
        public float? Grade { get; set; }

        public MareanoDatasetStatusReport()
        {
        }

        public MareanoDatasetStatusReport(MareanoDataset MareanoDataset)
        {
            if (MareanoDataset != null)
            {
                UuidMareanoDataset = MareanoDataset.Uuid;
                OrganizationSeoName = MareanoDataset.Owner.seoname;

                if (MareanoDataset.FindableStatus != null)
                    FindableMareanoDataset = MareanoDataset.FindableStatus.StatusId;
                if (MareanoDataset.AccesibleStatus != null)
                    AccesibleMareanoDataset = MareanoDataset.AccesibleStatus.StatusId;
                if (MareanoDataset.InteroperableStatus != null)
                    InteroperableMareanoDataset = MareanoDataset.InteroperableStatus.StatusId;
                if (MareanoDataset.ReUseableStatus != null)
                    ReUsableMareanoDataset = MareanoDataset.ReUseableStatus.StatusId;
                if (MareanoDataset.MetadataStatus != null)
                    MetadataMareanoDataset = MareanoDataset.MetadataStatus.StatusId;
                if (MareanoDataset.ProductSpesificationStatus != null)
                    ProductSpesificationMareanoDataset = MareanoDataset.ProductSpesificationStatus.StatusId;
                if (MareanoDataset.ProductSheetStatus != null)
                    ProductSheetMareanoDataset = MareanoDataset.ProductSheetStatus.StatusId;
                if (MareanoDataset.PresentationRulesStatus != null)
                    PresentationRulesMareanoDataset = MareanoDataset.PresentationRulesStatus.StatusId;
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

                if (MareanoDataset.Grade.HasValue)
                    Grade = MareanoDataset.Grade.Value;
            }
        }
    }
}