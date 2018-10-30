using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class DatasetStatusHistory : RegisterItemStatusReport
    {
        public DatasetStatusHistory()
        {
        }

        public DatasetStatusHistory(Dataset dataset)
        {
            Dataset = dataset;
            DatasetUuid = dataset.Uuid;
            Metadata = dataset.dokDeliveryMetadataStatusId;
            ProductSheet = dataset.dokDeliveryProductSheetStatusId;
            PresentationRules = dataset.dokDeliveryPresentationRulesStatusId;
            ProductSpecification = dataset.dokDeliveryProductSpecificationStatusId;
            Wms = dataset.dokDeliveryWmsStatusId;
            Wfs = dataset.dokDeliveryWfsStatusId;
            Distribution = dataset.dokDeliveryDistributionStatusId;
            SosiRequirements = dataset.dokDeliverySosiRequirementsStatusId;
            GmlRequirements = dataset.dokDeliveryGmlRequirementsStatusId;
            AtomFeed = dataset.dokDeliveryAtomFeedStatusId;
        }

        public virtual Dataset Dataset { get; set; }
        public string DatasetUuid { get; set; }
        public string Metadata { get; set; }
        public string ProductSheet { get; set; }
        public string PresentationRules { get; set; }
        public string ProductSpecification { get; set; }
        public string Wms { get; set; }
        public string Wfs { get; set; }
        public string Distribution { get; set; }
        public string SosiRequirements { get; set; }
        public string GmlRequirements { get; set; }
        public string AtomFeed { get; set; }
    }
}