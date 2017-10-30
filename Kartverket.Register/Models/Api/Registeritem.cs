﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Resources;
using Kartverket.Register.Models.Translations;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Models.Api
{
    public class Registeritem
    {

        // RegisterItem
        public string id { get; set; }
        public string label { get; set; }
        public string lang { get; set; } = Culture.NorwegianCode;
        public string itemclass { get; set; }
        public Guid uuid { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string seoname { get; set; }
        public string owner { get; set; }
        public string versionName { get; set; }
        public int versionNumber { get; set; }
        public ICollection<Registeritem> versions { get; set; }
        public DateTime lastUpdated { get; set; }
        public DateTime dateSubmitted { get; set; }
        public DateTime dateAccepted { get; set; }
        public string ApplicationSchema { get; set; }
        public string GMLApplicationSchema { get; set; }
        public string CartographyFile { get; set; }

        // Organization
        public string number { get; set; }
        public string OrganizationNumber { get; set; }
        public string LogoFilename { get; set; }
        public string LogoLargeFilename { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public bool? NorgeDigitaltMember { get; set; }
        public int? AgreementYear { get; set; }
        public string AgreementDocumentUrl { get; set; }
        public string PriceFormDocument { get; set; }
        public string ShortName { get; set; }
        public string OrganizationType { get; set; }
        public string MunicipalityCode { get; set; }
        public string GeographicCenterX { get; set; }
        public string GeographicCenterY { get; set; }
        public string BoundingBoxNorth { get; set; }
        public string BoundingBoxSouth { get; set; }
        public string BoundingBoxEast { get; set; }
        public string BoundingBoxWest { get; set; }


        // EPSG
        public string documentreference { get; set; }
        public string epsgcode { get; set; }
        public string sosiReferencesystem { get; set; }
        public string inspireRequirement { get; set; }
        public string nationalRequirement { get; set; }
        public string nationalSeasRequirement { get; set; }
        public string verticalReferenceSystem { get; set; }
        public string horizontalReferenceSystem { get; set; }
        public string dimension { get; set; }

        // CodelistValue
        public string codevalue { get; set; }
        public string broader { get; set; }
        public ICollection<string> narrower { get; set; }

        //NameSpace
        public string serviceUrl { get; set; }

        // Dataset
        public string theme { get; set; }
        public string dokStatus { get; set; }
        public DateTime? dokStatusDateAccepted { get; set; }
        public DateTime? Kandidatdato { get; set; }

        public string dokDeliveryMetadataStatus { get; set; }
        public string dokDeliveryProductSheetStatus { get; set; }
        public string dokDeliveryPresentationRulesStatus { get; set; }
        public string dokDeliveryProductSpecificationStatus { get; set; }
        public string dokDeliveryWmsStatus { get; set; }
        public string dokDeliveryWfsStatus { get; set; }
        public string dokDeliverySosiRequirementsStatus { get; set; }
        public string dokDeliveryDistributionStatus { get; set; }
        public string dokDeliveryGmlRequirementsStatus { get; set; }
        public string dokDeliveryAtomFeedStatus { get; set; }
        public bool? restricted { get; set; }

        public string DatasetType { get; set; }

        //MunicipalDataset
        public string ConfirmedDok { get; set; }
        public string Coverage { get; set; }
        public string NoteMunicipal { get; set; }

        // ServiceAlert
        public string MetadataUrl { get; set; }
        public DateTime AlertDate { get; set; }
        public string AlertType { get; set; }
        public string ServiceType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Note { get; set; }
        public string ServiceUuid { get; set; }

        // InspireDataset
        public string MetadataStatus { get; set; }
        public string MetadataServiceStatus { get; set; }
        public string DistributionStatus { get; set; }
        public string WmsStatus { get; set; }
        public string WfsStatus { get; set; }
        public string AtomFeedStatus { get; set; }
        public string WfsOrAtomStatus { get; set; }
        public string HarmonizedDataStatus { get; set; }
        public string SpatialDataServiceStatus { get; set; }

        // GeodatalovDataset
        public string CommonStatus { get; set; }
        public string GmlStatus { get; set; }
        public string SosiStatus { get; set; }
        public string ProductspesificationStatus { get; set; }

        public Registeritem(Object item, string baseUrl, FilterParameters filter = null)
        {
            this.versions = new HashSet<Registeritem>();
            this.narrower = new HashSet<string>();

            if (item is RegisterItem registerItem)
            {
                id = baseUrl + registerItem.GetObjectUrl();
                label = registerItem.name;
                lang = CultureHelper.GetCurrentCulture();
                lastUpdated = registerItem.modified;
                itemclass = registerItem.register.containedItemClass;
                if (registerItem.submitter != null) owner = registerItem.submitter.name;
                if (registerItem.status != null)
                {
                    if (CultureHelper.IsNorwegian())
                        status = registerItem.status.description;
                    else
                        status = registerItem.status.value;
                }
                if (registerItem.description != null) description = registerItem.description;
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
            }
            if (item is InspireDataset inspireDataset)
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
                codevalue = c.value;
                if (c.broaderItemId != null)
                    broader = baseUrl + c.broaderItem.GetObjectUrl();
                foreach (var codelistvalue in c.narrowerItems)
                {
                    narrower.Add(baseUrl + codelistvalue.GetObjectUrl());
                }
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
                dokDeliveryMetadataStatus = (d.restricted.HasValue && d.restricted == true) ? DataSet.DOK_Delivery_Restricted :  d.dokDeliveryMetadataStatus.description;
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
                            Coverage = coverage.Coverage ? "JA" : "NEI";
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
            }
            else if (item is ServiceAlert)
            {
                itemclass = "ServiceAlert";
                var s = (ServiceAlert)item;
                label = GetNameLocale(s);
                owner = GetOwnerLocale(s);
                MetadataUrl = s.ServiceMetadataUrl;
                AlertDate = s.AlertDate;
                AlertType = GetAlertTypeLocale(s);
                ServiceType = s.ServiceType;
                EffectiveDate = s.EffectiveDate;
                Note = GetNoteLocale(s);
                ServiceUuid = s.ServiceUuid;
            }
        }

        private string GetNameLocale(Models.Register item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.EPSG item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.CodelistValue item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.Organization item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.Document item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetNameLocale(Models.ServiceAlert item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }

        private string GetOwnerLocale(Models.ServiceAlert item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].Owner;
            if (string.IsNullOrEmpty(name))
                name = item.Owner;

            return name;
        }

        private string GetAlertTypeLocale(Models.ServiceAlert item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].AlertType;
            if (string.IsNullOrEmpty(name))
                name = item.AlertType;

            return name;
        }

        private string GetNoteLocale(Models.ServiceAlert item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var name = item.Translations[culture].Note;
            if (string.IsNullOrEmpty(name))
                name = item.Note;

            return name;
        }

        private string GetDescriptionLocale(Models.Register item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture].Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
        private string GetDescriptionLocale(Models.EPSG item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture].Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
        private string GetDescriptionLocale(Models.CodelistValue item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture].Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
        private string GetDescriptionLocale(Models.Organization item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture].Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
        private string GetDescriptionLocale(Models.Document item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture].Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }

        private string GetDescriptionLocale(Models.NameSpace item)
        {
            var culture = CultureHelper.GetCurrentCulture();
            var description = item.Translations[culture].Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
    }
}