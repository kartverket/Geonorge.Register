using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;
using Kartverket.Register.Models.Translations;
using Kartverket.Register.Helpers;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace Kartverket.Register.Models.Api
{
    [DataContractAttribute]
    public class Registeritem
    {

        public Registeritem()
        {
        }

        // RegisterItem
        [DataMemberAttribute]
        public string id { get; set; }
        [DataMemberAttribute]
        public string label { get; set; }
        [DataMemberAttribute]
        public string lang { get; set; } = Culture.NorwegianCode;
        [DataMemberAttribute]
        public string itemclass { get; set; }
        [DataMemberAttribute]
        public Guid uuid { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string status { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string description { get; set; }
        [DataMemberAttribute]
        public string seoname { get; set; }
        [DataMemberAttribute]
        public string owner { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string versionName { get; set; }
        [DataMemberAttribute]
        public int versionNumber { get; set; }
        [DataMemberAttribute]
        [XmlIgnore]
        public ICollection<Registeritem> versions { get; set; }
        public bool ShouldSerializeversions()
        {
            return versions != null && versions.Count() > 0;
        }
        [DataMemberAttribute]
        public DateTime lastUpdated { get; set; }
        [DataMemberAttribute]
        public DateTime dateSubmitted { get; set; }
        [DataMemberAttribute]
        public DateTime dateAccepted { get; set; }
        public bool ShouldSerializedateAccepted()
        {
            return dateAccepted != null && dateAccepted != DefaultDate;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationSchema { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GMLApplicationSchema { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CartographyFile { get; set; }

        // Organization
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string number { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OrganizationNumber { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LogoFilename { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LogoLargeFilename { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactPerson { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? NorgeDigitaltMember { get; set; }
        public bool ShouldSerializeNorgeDigitaltMember()
        {
            return NorgeDigitaltMember.HasValue;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? AgreementYear { get; set; }
        public bool ShouldSerializeAgreementYear()
        {
            return AgreementYear.HasValue;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AgreementDocumentUrl { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PriceFormDocument { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ShortName { get; set; }
        public string OrganizationType { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MunicipalityCode { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GeographicCenterX { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GeographicCenterY { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BoundingBoxNorth { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BoundingBoxSouth { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BoundingBoxEast { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BoundingBoxWest { get; set; }


        // EPSG
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string documentreference { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string epsgcode { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sosiReferencesystem { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string inspireRequirement { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string nationalRequirement { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string nationalSeasRequirement { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string verticalReferenceSystem { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string horizontalReferenceSystem { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dimension { get; set; }

        // CodelistValue
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string codevalue { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string broader { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> narrower { get; set; }
        public bool ShouldSerializenarrower()
        {
            return narrower != null && narrower.Count() > 0;
        }

        [DataMemberAttribute]
        [XmlIgnore]
        public List<NarrowerDetails> narrowerdetails { get; set; }
        public bool ShouldSerializenarrowerdetails()
        {
            return narrowerdetails != null && narrowerdetails.Count() > 0;
        }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ValidFrom { get; set; }
        public bool ShouldSerializeValidFrom()
        {
            return ValidFrom.HasValue;
        }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ValidTo { get; set; }
        public bool ShouldSerializeValidTo()
        {
            return ValidTo.HasValue;
        }

        //NameSpace
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string serviceUrl { get; set; }

        // Dataset
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string theme { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? dokStatusDateAccepted { get; set; }
        public bool ShouldSerializedokStatusDateAccepted()
        {
            return dokStatusDateAccepted.HasValue;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Kandidatdato { get; set; }
        public bool ShouldSerializeKandidatdato()
        {
            return Kandidatdato.HasValue;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryMetadataStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryProductSheetStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryPresentationRulesStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryProductSpecificationStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryWmsStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryWfsStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliverySosiRequirementsStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryDistributionStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryGmlRequirementsStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dokDeliveryAtomFeedStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? restricted { get; set; }
        public bool ShouldSerializerestricted()
        {
            return restricted.HasValue;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DatasetType { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UuidMetadata { get; set; }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Suitability Suitability { get; set; }


        //MunicipalDataset
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConfirmedDok { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Coverage { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NoteMunicipal { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Measure { get; set; }

        // Alert
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MetadataUrl { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime AlertDate { get; set; }
        public bool ShouldSerializeAlertDate()
        {
            return AlertDate != null && AlertDate != DefaultDate;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AlertType { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceType { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime EffectiveDate { get; set; }
        public bool ShouldSerializeEffectiveDate()
        {
            return EffectiveDate != null && EffectiveDate != DefaultDate;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceUuid { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AlertCategory { get; set; }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Departements { get; set; }
        public bool ShouldSerializeDepartement()
        {
            return Departements != null && Departements.Count > 0;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Station { get; set; }
        public bool ShouldSerializeStation()
        {
            return !string.IsNullOrEmpty(Station);
        }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StationName { get; set; }
        public bool ShouldSerializeStationName()
        {
            return !string.IsNullOrEmpty(StationName);
        }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StationType { get; set; }
        public bool ShouldSerializeStationType()
        {
            return !string.IsNullOrEmpty(StationType);
        }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tags { get; set; }
        public bool ShouldSerializeTags()
        {
            return Tags != null && Tags.Count > 0;
        }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateResolved { get; set; }
        public bool ShouldSerializeDateResolved()
        {
            return DateResolved.HasValue;
        }

        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Summary { get; set; }
        public bool ShouldSerializeSummary()
        {
            return !string.IsNullOrEmpty(Summary);
        }

        // InspireDataset
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MetadataStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MetadataServiceStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DistributionStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string WmsStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string WfsStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AtomFeedStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string WfsOrAtomStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HarmonizedDataStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SpatialDataServiceStatus { get; set; }

        // InspireDataService
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InspireDataType { get; set; }
        public string MetadataInSearchServiceStatus { get; set; }
        public string ServiceStatus { get; set; }
        public int? Requests { get; set; }
        public bool ShouldSerializeRequests()
        {
            return Requests.HasValue;
        }
        public string ServiceUrl { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InspireTheme { get; set; }
        public bool? NetworkService { get; set; }
        public bool ShouldSerializeNetworkService()
        {
            return NetworkService.HasValue;
        }
        public bool? Sds { get; set; }
        public bool ShouldSerializeSds()
        {
            return Sds.HasValue;
        }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InspireStatus { get; set; }

        // GeodatalovDataset
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CommonStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GmlStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SosiStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ProductspesificationStatus { get; set; }

        //Mareano
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FindableStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AccesibleStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InteroperableStatus { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ReUsableStatus { get; set; }

        [IgnoreDataMember]
        [XmlIgnore]
        public DateTime DefaultDate { get; set; } = new DateTime(1, 1, 1, 0, 0, 0) ;
        public string ProductSheetStatus { get; set; }
        public string PresentationRulesStatus { get; set; }

        public Registeritem(Object item, string baseUrl, FilterParameters filter = null)
        {
            this.versions = new HashSet<Registeritem>();
            this.narrower = new List<string>();
            this.narrowerdetails = new List<NarrowerDetails>();

            if (item is RegisterItem registerItem)
            {
                id = baseUrl + registerItem.GetObjectUrl();
                label = registerItem.NameTranslated();
                seoname = registerItem.seoname;
                lang = CultureHelper.GetCurrentCulture();
                lastUpdated = registerItem.modified;
                if (registerItem.submitter != null) owner = registerItem.submitter.name;
                if (registerItem.register.owner != null) owner = registerItem.register.owner.NameTranslated();
                if (registerItem.status != null)
                {
                    if (CultureHelper.IsNorwegian())
                        status = registerItem.status.description;
                    else
                        status = registerItem.status.value;
                }
                if (registerItem.description != null) description = registerItem.DescriptionTranslated();
                if (registerItem.versionName != null) versionName = registerItem.description;
                versionNumber = registerItem.versionNumber;
                versionName = registerItem.versionName;
                dateSubmitted = registerItem.dateSubmitted;
                dateAccepted = registerItem.dateAccepted.GetValueOrDefault();
                itemclass = "RegisterItem";
                uuid = registerItem.systemId;
            }
            if (item is RegisterItemV2 registerItemV2)
            {
                id = baseUrl + registerItemV2.DetailPageUrl();
                label = registerItemV2.Name;
                lang = CultureHelper.GetCurrentCulture();
                lastUpdated = registerItemV2.Modified;
                itemclass = registerItemV2.Register.containedItemClass;
                if (registerItemV2.Owner != null) owner = registerItemV2.Owner.name;
                if (registerItemV2.Status != null)
                {
                    if (CultureHelper.IsNorwegian())
                        status = registerItemV2.Status.description;
                    else
                        status = registerItemV2.Status.value;
                }
                description = registerItemV2.Description;
                versionNumber = registerItemV2.VersionNumber;
                versionName = registerItemV2.VersionName;
                dateSubmitted = registerItemV2.DateSubmitted;
                dateAccepted = registerItemV2.DateAccepted.GetValueOrDefault();
                uuid = registerItemV2.SystemId;
            }
            if (item is DatasetV2 datasetV2)
            {
                if (datasetV2.DokStatus != null) dokStatus = datasetV2.DokStatus.description;
                if (datasetV2.Theme != null) theme = datasetV2.Theme.description;
                if (datasetV2.DokStatusDateAccepted != null) dateAccepted = datasetV2.DokStatusDateAccepted.Value;
                MetadataUrl = datasetV2.MetadataUrl;
                UuidMetadata = datasetV2.Uuid;
            }
            if (item is Models.InspireDataset inspireDataset)
            {
                if (inspireDataset.InspireDeliveryMetadata?.Status != null)
                {
                    MetadataStatus = inspireDataset.InspireDeliveryMetadata.Status.value;
                }
                if (inspireDataset.InspireDeliveryMetadataService?.Status != null)
                {
                    MetadataServiceStatus = inspireDataset.InspireDeliveryMetadataService.Status.value;
                }
                if (inspireDataset.InspireDeliveryDistribution?.Status != null)
                {
                    DistributionStatus = inspireDataset.InspireDeliveryDistribution.Status.value;
                }
                if (inspireDataset.InspireDeliveryWms?.Status != null)
                {
                    WmsStatus = inspireDataset.InspireDeliveryWms.Status.value;
                }
                if (inspireDataset.InspireDeliveryWfs?.Status != null)
                {
                    WfsStatus = inspireDataset.InspireDeliveryWfs.Status.value;
                }
                if (inspireDataset.InspireDeliveryAtomFeed?.Status != null)
                {
                    AtomFeedStatus = inspireDataset.InspireDeliveryAtomFeed.Status.value;
                }
                if (inspireDataset.InspireDeliveryWfsOrAtom?.Status != null)
                {
                    WfsOrAtomStatus = inspireDataset.InspireDeliveryWfsOrAtom.Status.value;
                }
                if (inspireDataset.InspireDeliveryHarmonizedData?.Status != null)
                {
                    HarmonizedDataStatus = inspireDataset.InspireDeliveryHarmonizedData.Status.value;
                }
                if (inspireDataset.InspireDeliverySpatialDataService?.Status != null)
                {
                    SpatialDataServiceStatus = inspireDataset.InspireDeliverySpatialDataService.Status.value;
                }

                InspireTheme = inspireDataset.InspireThemsAsString();

                InspireStatus = "Mangler";
            }
            if (item is InspireDataService inspireDataService)
            {
                InspireDataType = inspireDataService.InspireDataType;
                MetadataStatus = inspireDataService.InspireDeliveryMetadata.Status.description;
                MetadataInSearchServiceStatus = inspireDataService.InspireDeliveryMetadataInSearchService.Status.description;
                ServiceStatus = inspireDataService.InspireDeliveryServiceStatus.Status.description;
                Requests = inspireDataService.Requests;
                ServiceUrl = inspireDataService.Url;
                InspireTheme = inspireDataService.InspireThemsAsString();
                NetworkService = inspireDataService.IsNetworkService();
                Sds = inspireDataService.IsSds();
                UuidMetadata = inspireDataService.Uuid;
                itemclass = "InspireDataService";
                MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + UuidMetadata;
                InspireStatus = "Mangler";
            }
            if (item is GeodatalovDataset geodatalovDataset)
            {
                if (geodatalovDataset.MetadataStatus?.Status != null)
                {
                    MetadataStatus = geodatalovDataset.MetadataStatus.Status.value;
                }
                if (geodatalovDataset.ProductSpesificationStatus?.Status != null)
                {
                    ProductspesificationStatus = geodatalovDataset.ProductSpesificationStatus.Status.value;
                }
                if (geodatalovDataset.SosiDataStatus?.Status != null)
                {
                    SosiStatus = geodatalovDataset.SosiDataStatus.Status.value;
                }
                if (geodatalovDataset.GmlDataStatus?.Status != null)
                {
                    GmlStatus = geodatalovDataset.GmlDataStatus.Status.value;
                }
                if (geodatalovDataset.WmsStatus?.Status != null)
                {
                    WmsStatus = geodatalovDataset.WmsStatus.Status.value;
                }
                if (geodatalovDataset.WfsStatus?.Status != null)
                {
                    WfsStatus = geodatalovDataset.WfsStatus.Status.value;
                }
                if (geodatalovDataset.AtomFeedStatus?.Status != null)
                {
                    AtomFeedStatus = geodatalovDataset.AtomFeedStatus.Status.value;
                }
                if (geodatalovDataset.CommonStatus?.Status != null)
                {
                    CommonStatus = geodatalovDataset.CommonStatus.Status.value;
                }
            }
            if (item is MareanoDataset mareanoDataset)
            {
                if (mareanoDataset.FindableStatus?.Status != null)
                {
                    FindableStatus = mareanoDataset.FindableStatus.Status.value;
                }
                if (mareanoDataset.AccesibleStatus?.Status != null)
                {
                    AccesibleStatus = mareanoDataset.AccesibleStatus.Status.value;
                }
                if (mareanoDataset.InteroperableStatus?.Status != null)
                {
                    InteroperableStatus = mareanoDataset.InteroperableStatus.Status.value;
                }
                if (mareanoDataset.ReUseableStatus?.Status != null)
                {
                    ReUsableStatus = mareanoDataset.ReUseableStatus.Status.value;
                }
                if (mareanoDataset.MetadataStatus?.Status != null)
                {
                    MetadataStatus = mareanoDataset.MetadataStatus.Status.value;
                }
                if (mareanoDataset.ProductSpesificationStatus?.Status != null)
                {
                    ProductspesificationStatus = mareanoDataset.ProductSpesificationStatus.Status.value;
                }
                if (mareanoDataset.ProductSheetStatus?.Status != null)
                {
                    ProductSheetStatus = mareanoDataset.ProductSheetStatus.Status.value;
                }
                if (mareanoDataset.PresentationRulesStatus?.Status != null)
                {
                    PresentationRulesStatus = mareanoDataset.PresentationRulesStatus.Status.value;
                }
                if (mareanoDataset.SosiDataStatus?.Status != null)
                {
                    SosiStatus = mareanoDataset.SosiDataStatus.Status.value;
                }
                if (mareanoDataset.GmlDataStatus?.Status != null)
                {
                    GmlStatus = mareanoDataset.GmlDataStatus.Status.value;
                }
                if (mareanoDataset.WmsStatus?.Status != null)
                {
                    WmsStatus = mareanoDataset.WmsStatus.Status.value;
                }
                if (mareanoDataset.WfsStatus?.Status != null)
                {
                    WfsStatus = mareanoDataset.WfsStatus.Status.value;
                }
                if (mareanoDataset.AtomFeedStatus?.Status != null)
                {
                    AtomFeedStatus = mareanoDataset.AtomFeedStatus.Status.value;
                }
                if (mareanoDataset.CommonStatus?.Status != null)
                {
                    CommonStatus = mareanoDataset.CommonStatus.Status.value;
                }
            }
            if (item is EPSG epsg)
            {
                itemclass = "EPSG";
                label = GetNameLocale(epsg);
                if (epsg.description != null) description = GetDescriptionLocale(epsg);
                epsgcode = epsg.epsgcode;
                sosiReferencesystem = epsg.sosiReferencesystem;
                documentreference = "http://www.opengis.net/def/crs/EPSG/0/" + epsg.epsgcode;
                if (epsg.inspireRequirement != null)
                {
                    if (CultureHelper.IsNorwegian())
                        inspireRequirement = epsg.inspireRequirement.description;
                    else
                        inspireRequirement = epsg.inspireRequirement.value;
                }
                if (epsg.nationalRequirement != null)
                {
                    if (CultureHelper.IsNorwegian())
                        nationalRequirement = epsg.nationalRequirement.description;
                    else
                        nationalRequirement = epsg.nationalRequirement.value;
                }
                if (epsg.nationalSeasRequirement != null)
                {
                    if (CultureHelper.IsNorwegian())
                        nationalSeasRequirement = epsg.nationalSeasRequirement.description;
                    else
                        nationalSeasRequirement = epsg.nationalSeasRequirement.value;
                }
                horizontalReferenceSystem = epsg.horizontalReferenceSystem;
                verticalReferenceSystem = epsg.verticalReferenceSystem;
                dimension = epsg.dimension != null ? epsg.dimension.DescriptionTranslated() : "";
            }
            else if (item is CodelistValue)
            {
                itemclass = "CodelistValue";
                var c = (CodelistValue)item;
                label = GetNameLocale(c);
                if (c.description != null) description = GetDescriptionLocale(c);
                codevalue = GetCodevalueLocale(c);
                if (string.IsNullOrEmpty(codevalue))
                    codevalue = label;
                if (c.broaderItemId != null)
                    broader = baseUrl + c.broaderItem.GetObjectUrl();
                foreach (var codelistvalue in c.narrowerItems)
                {
                    narrower.Add(baseUrl + codelistvalue.GetObjectUrl());
                }


                this.narrowerdetails = GetNarrowerDetails(c.narrowerItems.ToList(), baseUrl);

                ValidFrom = c.ValidFromDate;
                ValidTo = c.ValidToDate;
            }
            else if (item is Document)
            {
                itemclass = "Document";
                var d = (Document)item;
                label = GetNameLocale(d);
                if (d.description != null) description = GetDescriptionLocale(d);
                if (d.documentowner != null) owner = d.documentowner.NameTranslated();
                documentreference = d.documentUrl;
                ApplicationSchema = d.ApplicationSchema;
                GMLApplicationSchema = d.GMLApplicationSchema;
                CartographyFile = d.CartographyFile;
            }

            else if (item is Dataset)
            {
                itemclass = "Dataset";
                var d = (Dataset)item;
                if (d.datasetowner != null) owner = d.datasetowner.name;
                if (d.theme != null) theme = d.theme.description;
                if (d.dokStatus != null) dokStatus = d.dokStatus.description;
                if (d.dokStatusDateAccepted != null) dokStatusDateAccepted = d.dokStatusDateAccepted;
                if (d.Kandidatdato != null) Kandidatdato = d.Kandidatdato;
                if (d.DatasetType != null) DatasetType = d.DatasetType;
                dokDeliveryMetadataStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryMetadataStatus.description;
                dokDeliveryProductSheetStatus = (d.restricted != null && d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryProductSheetStatus.description;
                dokDeliveryPresentationRulesStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryPresentationRulesStatus.description;

                dokDeliveryProductSpecificationStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryProductSpecificationStatus.description;
                dokDeliveryWmsStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryWmsStatus.description;
                dokDeliveryWfsStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryWfsStatus.description;
                dokDeliverySosiRequirementsStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliverySosiRequirementsStatus.description;
                dokDeliveryDistributionStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryDistributionStatus.description;
                dokDeliveryGmlRequirementsStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryGmlRequirementsStatus.description;
                dokDeliveryAtomFeedStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted : d.dokDeliveryAtomFeedStatus.description;

                MetadataUrl = d.MetadataUrl;

                Suitability = new Suitability();

                Suitability.BuildingMatter = d.BuildingMatter.HasValue ? d.BuildingMatter.Value : 0;
                Suitability.BuildingMatterNote = d.BuildingMatterNote;

                Suitability.ImpactAssessmentPlanningBuildingAct = d.ImpactAssessmentPlanningBuildingAct.HasValue ? d.ImpactAssessmentPlanningBuildingAct.Value : 0;
                Suitability.ImpactAssessmentPlanningBuildingActNote = d.ImpactAssessmentPlanningBuildingActNote;

                Suitability.MunicipalLandUseElementPlan = d.MunicipalLandUseElementPlan.HasValue ? d.MunicipalLandUseElementPlan.Value : 0;
                Suitability.MunicipalLandUseElementPlanNote = d.MunicipalLandUseElementPlanNote;

                Suitability.MunicipalSocialPlan = d.MunicipalSocialPlan.HasValue ? d.MunicipalSocialPlan.Value : 0;
                Suitability.MunicipalSocialPlanNote = d.MunicipalSocialPlanNote;

                Suitability.RegionalPlan = d.RegionalPlan.HasValue ? d.RegionalPlan.Value : 0;
                Suitability.RegionalPlanNote = d.RegionalPlanNote;

                Suitability.RiskVulnerabilityAnalysisPlanningBuildingAct = d.RiskVulnerabilityAnalysisPlanningBuildingAct.HasValue ?
                    d.RiskVulnerabilityAnalysisPlanningBuildingAct.Value : 0;
                Suitability.RiskVulnerabilityAnalysisPlanningBuildingActNote = d.RiskVulnerabilityAnalysisPlanningBuildingActNote;

                Suitability.ZoningPlan = d.ZoningPlan.HasValue ? d.ZoningPlan.Value : 0;
                Suitability.ZoningPlanNote = d.ZoningPlanNote;


                ConfirmedDok = "NEI";
                Coverage = "NEI";
                if (filter != null && !string.IsNullOrEmpty(filter.municipality))
                {
                    Services.RegisterItem.RegisterItemService regItemService = new Services.RegisterItem.RegisterItemService(new RegisterDbContext());
                    Models.Organization org = regItemService.GetMunicipalityOrganizationByNr(filter.municipality);
                    if (org != null)
                    {
                        var coverage = d.Coverage.Where(c => c.DatasetId == d.systemId && c.MunicipalityId == org.systemId).FirstOrDefault();
                        if (coverage != null)
                        {
                            NoteMunicipal = coverage.Note;
                            ConfirmedDok = coverage.ConfirmedDok ? "JA" : "NEI";
                            Coverage = !coverage.Coverage.HasValue ? "Ukjent" : coverage.Coverage.Value == true ? "JA" : "NEI";
                            Measure = coverage?.MeasureDOKStatus?.description;
                            if (Measure == null) Measure = "";
                        }
                    }
                }
            }
            else if (item is NameSpace)
            {
                itemclass = "NameSpace";
                var n = (NameSpace)item;
                if (n.submitter != null) owner = n.submitter.NameTranslated();
                if (n.description != null) description = GetDescriptionLocale(n);
                serviceUrl = n.serviceUrl;
            }
            else if (item is Models.Organization)
            {
                itemclass = "Organization";
                Models.Organization organization = (Models.Organization)item;
                label = GetNameLocale(organization);
                if (organization.description != null) description = GetDescriptionLocale(organization);
                number = organization.number;
                MunicipalityCode = organization.MunicipalityCode;
                GeographicCenterX = organization.GeographicCenterX;
                GeographicCenterY = organization.GeographicCenterX;
                BoundingBoxNorth = organization.BoundingBoxNorth;
                BoundingBoxEast = organization.BoundingBoxEast;
                BoundingBoxSouth = organization.BoundingBoxSouth;
                BoundingBoxWest = organization.BoundingBoxWest;
                ShortName = organization.shortname;
                NorgeDigitaltMember = organization.member;
            }
            else if (item is Alert)
            {
                itemclass = "Alert";
                var s = (Alert)item;
                label = GetNameLocale(s);
                owner = GetOwnerLocale(s);
                MetadataUrl = s.UrlExternal;
                AlertDate = s.AlertDate;
                AlertType = GetAlertTypeLocale(s);
                if (s.AlertCategory != "Driftsmelding")
                {
                    ServiceType = s.Type;
                    ServiceUuid = s.UuidExternal;
                }
                EffectiveDate = s.EffectiveDate;
                Note = GetNoteLocale(s);
                AlertCategory = s.AlertCategory;
                if(!string.IsNullOrEmpty(s.StationName))
                    Station = s.StationName + " " + s.StationType?.ToLower();
                if (!string.IsNullOrEmpty(s.StationName))
                    StationName = s.StationName;
                if (!string.IsNullOrEmpty(s.StationType))
                    StationType = s.StationType?.ToLower();
                if (s.DateResolved.HasValue)
                    DateResolved = s.DateResolved;
                Summary = s.Summary;
                if (s.Departments != null)
                    Departements = s.Departments.Select(t => t.value).ToList();
                if (s.Tags != null)
                    Tags = s.Tags.Select(t => t.value).ToList();
                
            }
        }

        private List<NarrowerDetails> GetNarrowerDetails(List<CodelistValue> narrowerItems, string baseUrl)
        {
            List<NarrowerDetails> details = new List<NarrowerDetails>();
            foreach (var codelistvalue in narrowerItems)
            {
                details.Add(GetNarrowerDetail(codelistvalue, baseUrl));
            }
            return details;
        }

        private NarrowerDetails GetNarrowerDetail(CodelistValue codelistvalue, string baseUrl)
        {
            var itemNarrower = new NarrowerDetails { id= baseUrl + codelistvalue.GetObjectUrl(), label = codelistvalue.name, codevalue = codelistvalue.value };

            if(codelistvalue.narrowerItems != null && codelistvalue.narrowerItems.Any())
                itemNarrower.narrowerdetails = new List<NarrowerDetails>();

            foreach (var codelistvalue2 in codelistvalue.narrowerItems)
            {
                NarrowerDetails narrowerDetails = GetNarrowerDetail(codelistvalue2, baseUrl);
                itemNarrower.narrowerdetails.Add(narrowerDetails);
                narrowerdetails.Add(itemNarrower);
            }

            return itemNarrower;
        }

        public bool IsInspireDataset()
        {
            return itemclass == "InspireDataset";
        }

        private string GetNameLocale(Models.Register item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.EPSG item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.CodelistValue item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.Organization item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.Document item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.Alert item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetOwnerLocale(Models.Alert item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.Owner;
            if (string.IsNullOrEmpty(name))
                name = item.Owner;

            return name;
        }

        private string GetAlertTypeLocale(Models.Alert item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.AlertType;
            if (string.IsNullOrEmpty(name))
                name = item.AlertType;

            return name;
        }

        private string GetNoteLocale(Models.Alert item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture]?.Note;
            if (string.IsNullOrEmpty(name))
                name = item.Note;

            return name;
        }

        private string GetDescriptionLocale(Models.Register item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture]?.Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
        private string GetDescriptionLocale(Models.EPSG item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture]?.Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
        private string GetDescriptionLocale(Models.CodelistValue item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture]?.Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
        private string GetDescriptionLocale(Models.Organization item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture]?.Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
        private string GetDescriptionLocale(Models.Document item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture]?.Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }

        private string GetDescriptionLocale(Models.NameSpace item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture]?.Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }

        private string GetCodevalueLocale(Models.CodelistValue item)
        {
            return item.CodelistvalueTranslated();
        }
    }

    public class NarrowerDetails {
        public string id { get; set; }
        public string label { get; set; }
        public string codevalue { get; set; }
        public List<NarrowerDetails> narrowerdetails { get; set; }
    }

    public class Suitability
    {
        public int RegionalPlan { get; set; }
        public string RegionalPlanNote { get; set; }

        public int MunicipalSocialPlan { get; set; }
        public string MunicipalSocialPlanNote { get; set; }

        public int MunicipalLandUseElementPlan { get; set; }
        public string MunicipalLandUseElementPlanNote { get; set; }

        public int ZoningPlan { get; set; }
        public string ZoningPlanNote { get; set; }

        public int BuildingMatter { get; set; }
        public string BuildingMatterNote { get; set; }

        public int ImpactAssessmentPlanningBuildingAct { get; set; }
        public string ImpactAssessmentPlanningBuildingActNote { get; set; }

        public int RiskVulnerabilityAnalysisPlanningBuildingAct { get; set; }
        public string RiskVulnerabilityAnalysisPlanningBuildingActNote { get; set; }
    }
}