using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Resources;

namespace Kartverket.Register.Models.Api
{
    public class Registeritem
    {

        // RegisterItem
        public string id { get; set; }
        public string label { get; set; }
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

        public Registeritem(Models.RegisterItem item, string baseUrl, FilterParameters filter = null)
        {
            this.versions = new HashSet<Registeritem>();
            this.narrower = new HashSet<string>();

            id = baseUrl + item.GetObjectUrl();
            label = item.name;
            lastUpdated = item.modified;
            itemclass = item.register.containedItemClass;
            if (item.submitter != null) owner = item.submitter.name;
            if (item.status != null) status = item.status.description;
            if (item.description != null) description = item.description;
            if (item.versionName != null) versionName = item.description;
            versionNumber = item.versionNumber;
            versionName = item.versionName;
            dateSubmitted = item.dateSubmitted;
            dateAccepted = item.dateAccepted.GetValueOrDefault();
            itemclass = "RegisterItem";
            uuid = item.systemId;

            if (item is EPSG)
            {
                itemclass = "EPSG";
                var d = (EPSG)item;
                epsgcode = d.epsgcode;
                sosiReferencesystem = d.sosiReferencesystem;
                documentreference = "http://www.opengis.net/def/crs/EPSG/0/" + d.epsgcode;
                inspireRequirement = d.inspireRequirement.description;
                nationalRequirement = d.nationalRequirement.description;
                nationalSeasRequirement = d.nationalSeasRequirement != null ? d.nationalSeasRequirement.description : "";
                horizontalReferenceSystem = d.horizontalReferenceSystem;
                verticalReferenceSystem = d.verticalReferenceSystem;
                dimension = d.dimension != null ? d.dimension.description : "";
            }
            else if (item is CodelistValue)
            {
                itemclass = "CodelistValue";
                var c = (CodelistValue)item;
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
                if (d.documentowner != null) owner = d.documentowner.name;
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
                dokDeliveryMetadataStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted :  d.dokDeliveryMetadataStatus.description;
                dokDeliveryProductSheetStatus = (d.restricted != null && d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryProductSheetStatus.description;
                dokDeliveryPresentationRulesStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryPresentationRulesStatus.description;

                dokDeliveryProductSpecificationStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryProductSpecificationStatus.description;
                dokDeliveryWmsStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryWmsStatus.description;
                dokDeliveryWfsStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryWfsStatus.description;
                dokDeliverySosiRequirementsStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliverySosiRequirementsStatus.description;
                dokDeliveryDistributionStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryDistributionStatus.description;
                dokDeliveryGmlRequirementsStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryGmlRequirementsStatus.description;
                dokDeliveryAtomFeedStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryAtomFeedStatus.description;

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
                serviceUrl = n.serviceUrl;
            }
            else if (item is Models.Organization)
            {
                itemclass = "Organization";
                Models.Organization organization = (Models.Organization)item;
                number = organization.number;
                MunicipalityCode = organization.MunicipalityCode;
                GeographicCenterX = organization.GeographicCenterX;
                GeographicCenterY = organization.GeographicCenterX;
                BoundingBoxNorth = organization.BoundingBoxNorth;
                BoundingBoxEast = organization.BoundingBoxEast;
                BoundingBoxSouth = organization.BoundingBoxSouth;
                BoundingBoxWest = organization.BoundingBoxWest;
            }
            else if (item is ServiceAlert)
            {
                itemclass = "ServiceAlert";
                var s = (ServiceAlert)item;
                owner = s.Owner;
                MetadataUrl = s.ServiceMetadataUrl;
                AlertDate = s.AlertDate;
                AlertType = s.AlertType;
                ServiceType = s.ServiceType;
                EffectiveDate = s.EffectiveDate;
                Note = s.Note;
                ServiceUuid = s.ServiceUuid;
            }
        }
    }
}