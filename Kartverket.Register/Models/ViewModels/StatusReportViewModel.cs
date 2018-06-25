using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class StatusReportViewModel
    {
        private const string Good = "good";
        private const string Deficent = "deficent";
        private const string Notset = "notset";
        private const string Useable = "useable";


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


        public StatusReportViewModel(StatusReport statusReport)
        {
            NumberOfItemsWithMetadataGood = statusReport.NumberOfItemsWithMetadata(Good);
            NumberOfItemsWithMetadataDeficient = statusReport.NumberOfItemsWithMetadata(Deficent);
            NumberOfItemsWithMetadataNotSet = statusReport.NumberOfItemsWithMetadata(Notset);
            NumberOfItemsWithMetadataUseable = statusReport.NumberOfItemsWithMetadata(Useable);

            NumberOfItemsWithProductsheetGood = statusReport.NumberOfItemsWithProductsheet(Good);
            NumberOfItemsWithProductsheetDeficient = statusReport.NumberOfItemsWithProductsheet(Deficent);
            NumberOfItemsWithProductsheetNotSet = statusReport.NumberOfItemsWithProductsheet(Notset);
            NumberOfItemsWithProductsheetUseable = statusReport.NumberOfItemsWithProductsheet(Useable);

            NumberOfItemsWithPresentationRulesGood = statusReport.NumberOfItemsWithPresentationRules(Good);
            NumberOfItemsWithPresentationRulesDeficient = statusReport.NumberOfItemsWithPresentationRules(Deficent);
            NumberOfItemsWithPresentationRulesNotSet = statusReport.NumberOfItemsWithPresentationRules(Notset);
            NumberOfItemsWithPresentationRulesUseable = statusReport.NumberOfItemsWithPresentationRules(Useable);

            NumberOfItemsWithProductSpecificationGood = statusReport.NumberOfItemsWithProductSpecification(Good);
            NumberOfItemsWithProductSpecificationDeficient = statusReport.NumberOfItemsWithProductSpecification(Deficent);
            NumberOfItemsWithProductSpecificationNotSet = statusReport.NumberOfItemsWithProductSpecification(Notset);
            NumberOfItemsWithProductSpecificationUseable = statusReport.NumberOfItemsWithProductSpecification(Useable);

            NumberOfItemsWithWmsGood = statusReport.NumberOfItemsWithWms(Good);
            NumberOfItemsWithWmsDeficient = statusReport.NumberOfItemsWithWms(Deficent);
            NumberOfItemsWithWmsNotSet = statusReport.NumberOfItemsWithWms(Notset);
            NumberOfItemsWithWmsUseable = statusReport.NumberOfItemsWithWms(Useable);

            NumberOfItemsWithWfsGood = statusReport.NumberOfItemsWithWfs(Good);
            NumberOfItemsWithWfsDeficient = statusReport.NumberOfItemsWithWfs(Deficent);
            NumberOfItemsWithWfsNotSet = statusReport.NumberOfItemsWithWfs(Notset);
            NumberOfItemsWithWfsUseable = statusReport.NumberOfItemsWithWfs(Useable);

            NumberOfItemsWithSosiRequirementsGood = statusReport.NumberOfItemsWithSosiRequirements(Good);
            NumberOfItemsWithSosiRequirementsDeficient = statusReport.NumberOfItemsWithSosiRequirements(Deficent);
            NumberOfItemsWithSosiRequirementsNotSet = statusReport.NumberOfItemsWithSosiRequirements(Notset);
            NumberOfItemsWithSosiRequirementsUseable = statusReport.NumberOfItemsWithSosiRequirements(Useable);

            NumberOfItemsWithGmlRequirementsGood = statusReport.NumberOfItemsWithGmlRequirements(Good);
            NumberOfItemsWithGmlRequirementsDeficient = statusReport.NumberOfItemsWithGmlRequirements(Deficent);
            NumberOfItemsWithGmlRequirementsNotSet = statusReport.NumberOfItemsWithGmlRequirements(Notset);
            NumberOfItemsWithGmlRequirementsUseable = statusReport.NumberOfItemsWithGmlRequirements(Useable);

            NumberOfItemsWithAtomFeedGood = statusReport.NumberOfItemsWithAtomFeed(Good);
            NumberOfItemsWithAtomFeedDeficient = statusReport.NumberOfItemsWithAtomFeed(Deficent);
            NumberOfItemsWithAtomFeedNotSet = statusReport.NumberOfItemsWithAtomFeed(Notset);
            NumberOfItemsWithAtomFeedUseable = statusReport.NumberOfItemsWithAtomFeed(Useable);

            NumberOfItemsWithDistributionGood = statusReport.NumberOfItemsWithDistribution(Good);
            NumberOfItemsWithDistributionDeficient = statusReport.NumberOfItemsWithDistribution(Deficent);
            NumberOfItemsWithDistributionNotSet = statusReport.NumberOfItemsWithDistribution(Notset);
            NumberOfItemsWithDistributionUseable = statusReport.NumberOfItemsWithDistribution(Useable);
        }

    }
}