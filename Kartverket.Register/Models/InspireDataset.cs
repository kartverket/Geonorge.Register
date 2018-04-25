using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Kartverket.Register.Models
{
    public class InspireDataset : DatasetV2
    {
        public virtual ICollection<CodelistValue> InspireThemes { get; set; }

        //Metadata
        [ForeignKey("InspireDeliveryMetadata"), Required, Display(Name = "Metadata:")]
        public Guid InspireDeliveryMetadataId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadata { get; set; }

        //Metadat service
        [ForeignKey("InspireDeliveryMetadataService"), Required, Display(Name = "Metadatatjeneste:")]
        public Guid InspireDeliveryMetadataServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadataService { get; set; }

        //Distribution
        [ForeignKey("InspireDeliveryDistribution"), Required, Display(Name = "Deling av data:")]
        public Guid InspireDeliveryDistributionId { get; set; }
        public virtual DatasetDelivery InspireDeliveryDistribution { get; set; }

        //View service (Visningstjeneste)
        [ForeignKey("InspireDeliveryWms"), Required, Display(Name = "WMS:")]
        public Guid InspireDeliveryWmsId { get; set; }
        public virtual DatasetDelivery InspireDeliveryWms { get; set; }

        //WFS
        [ForeignKey("InspireDeliveryWfs"), Required, Display(Name = "Nedlastingstjeneste WFS:")]
        public Guid InspireDeliveryWfsId { get; set; }
        public virtual DatasetDelivery InspireDeliveryWfs { get; set; }

        //Atom-feed
        [ForeignKey("InspireDeliveryAtomFeed"), Required, Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid InspireDeliveryAtomFeedId { get; set; }
        public virtual DatasetDelivery InspireDeliveryAtomFeed { get; set; }

        //Atom or WFS
        [ForeignKey("InspireDeliveryWfsOrAtom"), Required, Display(Name = "Nedlastingstjeneste WFS eller Atom-feed:")]
        public Guid InspireDeliveryWfsOrAtomId { get; set; }
        public virtual DatasetDelivery InspireDeliveryWfsOrAtom { get; set; }

        //Harmonized data
        [ForeignKey("InspireDeliveryHarmonizedData"), Required, Display(Name = "Harmoniserte data:")]
        public Guid InspireDeliveryHarmonizedDataId { get; set; }
        public virtual DatasetDelivery InspireDeliveryHarmonizedData { get; set; }

        //Spatial data service
        [ForeignKey("InspireDeliverySpatialDataService"), Required, Display(Name = "Spatial data service:")]
        public Guid InspireDeliverySpatialDataServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliverySpatialDataService { get; set; }

        [Display(Name = "Areal")]
        public int Area { get; set; }
        [Display(Name = "Relevant areal")]
        public int RelevantArea { get; set; }

        public bool HaveMetadata()
        {
            return InspireDeliveryMetadata.IsSet();
        }

        public bool WmsIsGoodOrUseable()
        {
            if (InspireDeliveryWms != null)
            {
                return InspireDeliveryWms.IsGoodOrUseable();
            }
            return false;
        }

        internal bool MetadataIsGoodOrDeficent()
        {
            if (InspireDeliveryMetadata != null)
            {
                return InspireDeliveryMetadata.IsSet();
            }
            return false;
        }

        public void UpdateInspireTheme(ICollection<CodelistValue> inspireThemes)
        {
            RemoveInspireTheme(inspireThemes);
            AddToList(inspireThemes);
        }

        private void AddToList(ICollection<CodelistValue> inspireThemesUpdated)
        {
            foreach (var inspireTheme in inspireThemesUpdated)
            {
                if (!InspireThemes.Any(i => i.systemId == inspireTheme.systemId))
                {
                    InspireThemes.Add(inspireTheme);
                }
            }
        }

        private void RemoveInspireTheme(ICollection<CodelistValue> inspireThemesToUpdate)
        {
            var exists = false;
            var removeDatasets = new List<CodelistValue>();

            foreach (var inspireTheme in InspireThemes)
            {
                if (inspireThemesToUpdate.Any(i => i.systemId == inspireTheme.systemId))
                {
                    exists = true;
                }
                if (!exists)
                {
                    removeDatasets.Add(inspireTheme);
                }
                exists = false;
            }
            foreach (var inspireTheme in removeDatasets)
            {
                InspireThemes.Remove(inspireTheme);
            }
        }

        internal bool WmsAndWfsIsGoodOrUseable()
        {
            if (InspireDeliveryWfs != null && InspireDeliveryWms != null)
            {
                return InspireDeliveryWfs.IsGoodOrUseable() && InspireDeliveryWms.IsGoodOrUseable();
            }
            return false;
        }

        internal bool HarmonizedDataAndConformedmetadata()
        {
            if (InspireDeliveryHarmonizedData != null && InspireDeliveryMetadata != null)
            {
                return InspireDeliveryHarmonizedData.IsGood() && InspireDeliveryMetadata.IsGood();
            }
            return false;
        }

        public bool WfsIsGoodOrUseable()
        {
            if (InspireDeliveryWfs != null)
            {
                return InspireDeliveryWfs.IsGoodOrUseable();
            }
            return false;
        }

        internal bool MetadataIsGood()
        {
            if (InspireDeliveryMetadata != null)
            {
                return InspireDeliveryMetadata.IsGood();
            }
            return false;
        }

        internal bool HarmonizedIsGood()
        {
            if (InspireDeliveryHarmonizedData != null)
            {
                return InspireDeliveryHarmonizedData.IsGood();
            }
            return false;
        }

        internal bool WfsOrAtomIsGoodOrUseable()
        {
            if (InspireDeliveryWfsOrAtom != null)
            {
                return InspireDeliveryWfsOrAtom.IsGoodOrUseable();
            }
            return false;
        }

        internal bool WmsAndWfsOrAtomIsGoodOrUseable()
        {
            if (InspireDeliveryWfsOrAtom != null && InspireDeliveryWms != null)
            {
                return InspireDeliveryWfsOrAtom.IsGoodOrUseable() && InspireDeliveryWms.IsGoodOrUseable();
            }
            return false;
        }

        public string InspireThemsAsString()
        {
            string inspireTeamsString = null;
            foreach (var item in InspireThemes)
            {
                if (inspireTeamsString == null)
                {
                    inspireTeamsString += item.name;
                }
                else
                {
                    inspireTeamsString += ", " + item.name;
                }
            }
            return inspireTeamsString;
        }
    }

}//end namespace Datamodell