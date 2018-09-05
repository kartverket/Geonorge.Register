using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace Kartverket.Register.Models.Api
{
    public class DokStatusReport : StatusReport
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public DokStatusReport(Models.StatusReport statusReport)
        {
            if (statusReport != null)
            {
                Date = statusReport.Date;
                NumberOfItems = statusReport.NumberOfIems();

                NumberOfItemsWithMetadataGood = statusReport.NumberOfItemsWithMetadata(Good);
                NumberOfItemsWithMetadataNotSet = statusReport.NumberOfItemsWithMetadata(Notset);
                NumberOfItemsWithMetadataDeficient = statusReport.NumberOfItemsWithMetadata(Deficient);
                NumberOfItemsWithMetadataUseable = statusReport.NumberOfItemsWithMetadata(Useable);

                NumberOfItemsWithProductsheetGood = statusReport.NumberOfItemsWithProductsheet(Good);
                NumberOfItemsWithProductsheetNotSet = statusReport.NumberOfItemsWithProductsheet(Notset);
                NumberOfItemsWithProductsheetDeficient = statusReport.NumberOfItemsWithProductsheet(Deficient);
                NumberOfItemsWithProductsheetUseable = statusReport.NumberOfItemsWithProductsheet(Useable);

                NumberOfItemsWithPresentationRulesGood = statusReport.NumberOfItemsWithPresentationRules(Good);
                NumberOfItemsWithPresentationRulesDeficient = statusReport.NumberOfItemsWithPresentationRules(Deficient);
                NumberOfItemsWithPresentationRulesUseable = statusReport.NumberOfItemsWithPresentationRules(Useable);
                NumberOfItemsWithPresentationRulesNotSet = statusReport.NumberOfItemsWithPresentationRules(Notset);

                NumberOfItemsWithProductSpecificationGood = statusReport.NumberOfItemsWithPresentationRules(Good);
                NumberOfItemsWithProductSpecificationDeficient = statusReport.NumberOfItemsWithProductSpecification(Deficient);
                NumberOfItemsWithProductSpecificationUseable = statusReport.NumberOfItemsWithProductSpecification(Useable);
                NumberOfItemsWithProductSpecificationNotSet = statusReport.NumberOfItemsWithProductSpecification(Notset);

                NumberOfItemsWithWmsGood = statusReport.NumberOfItemsWithWms(Good);
                NumberOfItemsWithWmsDeficient = statusReport.NumberOfItemsWithWms(Deficient);
                NumberOfItemsWithWmsUseable = statusReport.NumberOfItemsWithWms(Useable);
                NumberOfItemsWithWmsNotSet = statusReport.NumberOfItemsWithWms(Notset);

                NumberOfItemsWithWfsGood = statusReport.NumberOfItemsWithWfs(Good);
                NumberOfItemsWithWfsDeficient = statusReport.NumberOfItemsWithWfs(Deficient);
                NumberOfItemsWithWfsUseable = statusReport.NumberOfItemsWithWfs(Useable);
                NumberOfItemsWithWfsNotSet = statusReport.NumberOfItemsWithWfs(Notset);

                NumberOfItemsWithGmlRequirementsGood = statusReport.NumberOfItemsWithGmlRequirements(Good);
                NumberOfItemsWithGmlRequirementsDeficient = statusReport.NumberOfItemsWithGmlRequirements(Deficient);
                NumberOfItemsWithGmlRequirementsNotSet = statusReport.NumberOfItemsWithGmlRequirements(Notset);
                NumberOfItemsWithGmlRequirementsUseable = statusReport.NumberOfItemsWithGmlRequirements(Useable);

                NumberOfItemsWithSosiRequirementsGood = statusReport.NumberOfItemsWithSosiRequirements(Good);
                NumberOfItemsWithSosiRequirementsDeficient = statusReport.NumberOfItemsWithSosiRequirements(Deficient);
                NumberOfItemsWithSosiRequirementsUseable = statusReport.NumberOfItemsWithSosiRequirements(Useable);
                NumberOfItemsWithSosiRequirementsNotSet = statusReport.NumberOfItemsWithSosiRequirements(Notset);

                NumberOfItemsWithAtomFeedGood = statusReport.NumberOfItemsWithAtomFeed(Good);
                NumberOfItemsWithAtomFeedDeficient = statusReport.NumberOfItemsWithAtomFeed(Deficient);
                NumberOfItemsWithAtomFeedNotSet = statusReport.NumberOfItemsWithAtomFeed(Notset);
                NumberOfItemsWithAtomFeedUseable = statusReport.NumberOfItemsWithAtomFeed(Useable);

                NumberOfItemsWithDistributionGood = statusReport.NumberOfItemsWithDistribution(Good);
                NumberOfItemsWithDistributionDeficient = statusReport.NumberOfItemsWithDistribution(Deficient);
                NumberOfItemsWithDistributionUseable = statusReport.NumberOfItemsWithDistribution(Useable);
                NumberOfItemsWithDistributionNotSet = statusReport.NumberOfItemsWithDistribution(Notset);
            }
        }


        // Metadata
        public int NumberOfItemsWithMetadataGood { get; set; }
        public int NumberOfItemsWithMetadataNotSet { get; set; }
        public int NumberOfItemsWithMetadataDeficient { get; set; }
        public int NumberOfItemsWithMetadataUseable { get; set; }


        // ProductSheet
        public int NumberOfItemsWithProductsheetGood { get; set; }
        public int NumberOfItemsWithProductsheetNotSet { get; set; }
        public int NumberOfItemsWithProductsheetDeficient { get; set; }
        public int NumberOfItemsWithProductsheetUseable { get; set; }


        // PresentationRules
        public int NumberOfItemsWithPresentationRulesGood { get; set; }
        public int NumberOfItemsWithPresentationRulesNotSet { get; set; }
        public int NumberOfItemsWithPresentationRulesDeficient { get; set; }
        public int NumberOfItemsWithPresentationRulesUseable { get; set; }


        // ProductSpecification
        public int NumberOfItemsWithProductSpecificationGood { get; set; }
        public int NumberOfItemsWithProductSpecificationNotSet { get; set; }
        public int NumberOfItemsWithProductSpecificationDeficient { get; set; }
        public int NumberOfItemsWithProductSpecificationUseable { get; set; }


        // Wms
        public int NumberOfItemsWithWmsGood { get; set; }
        public int NumberOfItemsWithWmsNotSet { get; set; }
        public int NumberOfItemsWithWmsDeficient { get; set; }
        public int NumberOfItemsWithWmsUseable { get; set; }


        // Wfs
        public int NumberOfItemsWithWfsGood { get; set; }
        public int NumberOfItemsWithWfsNotSet { get; set; }
        public int NumberOfItemsWithWfsDeficient { get; set; }
        public int NumberOfItemsWithWfsUseable { get; set; }


        // SosiRequirements
        public int NumberOfItemsWithSosiRequirementsGood { get; set; }
        public int NumberOfItemsWithSosiRequirementsNotSet { get; set; }
        public int NumberOfItemsWithSosiRequirementsDeficient { get; set; }
        public int NumberOfItemsWithSosiRequirementsUseable { get; set; }


        // GmlRequirements
        public int NumberOfItemsWithGmlRequirementsGood { get; set; }
        public int NumberOfItemsWithGmlRequirementsNotSet { get; set; }
        public int NumberOfItemsWithGmlRequirementsDeficient { get; set; }
        public int NumberOfItemsWithGmlRequirementsUseable { get; set; }


        // AtomFeed
        public int NumberOfItemsWithAtomFeedGood { get; set; }
        public int NumberOfItemsWithAtomFeedNotSet { get; set; }
        public int NumberOfItemsWithAtomFeedDeficient { get; set; }
        public int NumberOfItemsWithAtomFeedUseable { get; set; }


        // Distribution
        public int NumberOfItemsWithDistributionGood { get; set; }
        public int NumberOfItemsWithDistributionNotSet { get; set; }
        public int NumberOfItemsWithDistributionDeficient { get; set; }
        public int NumberOfItemsWithDistributionUseable { get; set; }
    }
}