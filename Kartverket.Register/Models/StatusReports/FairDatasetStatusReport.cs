using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.StatusReports
{
    public class FairDatasetStatusReport : RegisterItemStatusReport
    {

        public string UuidFairDataset { get; set; }
        public string FindableFairDataset { get; set; }
        public string AccesibleFairDataset { get; set; }
        public string InteroperableFairDataset { get; set; }
        public string MetadataFairDataset { get; set; }
        public string ReUsableFairDataset { get; set; }
        public string ProductSpesificationFairDataset { get; set; }
        public string ProductSheetFairDataset { get; set; }
        public string PresentationRulesFairDataset { get; set; }
        public string SosiDataFairDataset { get; set; }
        public string GmlDataFairDataset { get; set; }
        public string WmsFairDataset { get; set; }
        public string WfsFairDataset { get; set; }
        public string AtomFeedFairDataset { get; set; }
        public string CommonStatusFairDataset { get; set; }

        public FairDatasetStatusReport()
        {
        }

        public FairDatasetStatusReport(FairDataset FairDataset)
        {
            if (FairDataset != null)
            {
                UuidFairDataset = FairDataset.Uuid;
                if(FairDataset.Owner != null)
                    OrganizationSeoName = FairDataset.Owner.seoname;

                if (FairDataset.FindableStatus != null)
                    FindableFairDataset = FairDataset.FindableStatus.StatusId;
                if (FairDataset.AccesibleStatus != null)
                    AccesibleFairDataset = FairDataset.AccesibleStatus.StatusId;
                if (FairDataset.InteroperableStatus != null)
                    InteroperableFairDataset = FairDataset.InteroperableStatus.StatusId;
                if (FairDataset.ReUseableStatus != null)
                    ReUsableFairDataset = FairDataset.ReUseableStatus.StatusId;
                if (FairDataset.MetadataStatus != null)
                    MetadataFairDataset = FairDataset.MetadataStatus.StatusId;
                if (FairDataset.ProductSpesificationStatus != null)
                    ProductSpesificationFairDataset = FairDataset.ProductSpesificationStatus.StatusId;
                if (FairDataset.ProductSheetStatus != null)
                    ProductSheetFairDataset = FairDataset.ProductSheetStatus.StatusId;
                if (FairDataset.PresentationRulesStatus != null)
                    PresentationRulesFairDataset = FairDataset.PresentationRulesStatus.StatusId;
                if (FairDataset.SosiDataStatus != null)
                    SosiDataFairDataset = FairDataset.SosiDataStatus.StatusId;
                if (FairDataset.GmlDataStatus != null)
                    GmlDataFairDataset = FairDataset.GmlDataStatus.StatusId;
                if (FairDataset.WmsStatus != null) WmsFairDataset = FairDataset.WmsStatus.StatusId;
                if (FairDataset.WfsStatus != null) WfsFairDataset = FairDataset.WfsStatus.StatusId;
                if (FairDataset.AtomFeedStatus != null)
                    AtomFeedFairDataset = FairDataset.AtomFeedStatus.StatusId;
                if (FairDataset.CommonStatus != null)
                    CommonStatusFairDataset = FairDataset.CommonStatus.StatusId;

                if (FairDataset.Grade.HasValue)
                    Grade = FairDataset.Grade.Value;

                FindableStatusPerCent = FairDataset.FindableStatusPerCent;
                AccessibleStatusPerCent = FairDataset.AccesibleStatusPerCent;
                InteroperableStatusPerCent = FairDataset.InteroperableStatusPerCent;
                ReUseableStatusPerCent = FairDataset.ReUseableStatusPerCent;
            }
        }
    }
}