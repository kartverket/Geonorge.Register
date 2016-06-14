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
        public Guid systemId { get; set; }
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

        // Organization
        public string logo { get; set; }
        public string number { get; set; }

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

        public string dokDeliveryMetadataStatus { get; set; }
        public string dokDeliveryProductSheetStatus { get; set; }
        public string dokDeliveryPresentationRulesStatus { get; set; }
        public string dokDeliveryProductSpecificationStatus { get; set; }
        public string dokDeliveryWmsStatus { get; set; }
        public string dokDeliveryWfsStatus { get; set; }
        public string dokDeliveryDistributionAreaStatus { get; set; }
        public string dokDeliveryDistributionStatus { get; set; }
        public string dokDeliveryServiceAlertStatus { get; set; }
        public string dokDeliveryGeodataLawStatus { get; set; }
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
            systemId = item.systemId;

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
            }

            else if (item is Dataset)
            {
                itemclass = "Dataset";
                var d = (Dataset)item;
                if (d.datasetowner != null) owner = d.datasetowner.name;
                if (d.theme != null) theme = d.theme.description;
                if (d.dokStatus != null) dokStatus = d.dokStatus.description;
                if (d.DatasetType != null) DatasetType = d.DatasetType;
                dokDeliveryMetadataStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted :  d.dokDeliveryMetadataStatus.description;
                dokDeliveryProductSheetStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryProductSheetStatus.description;
                dokDeliveryPresentationRulesStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryPresentationRulesStatus.description;

                dokDeliveryProductSpecificationStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryProductSpecificationStatus.description;
                dokDeliveryWmsStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryWmsStatus.description;
                dokDeliveryWfsStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryWfsStatus.description;
                dokDeliveryDistributionAreaStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryDistributionAreaStatus.description;
                dokDeliveryDistributionStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryDistributionStatus.description;
                dokDeliveryServiceAlertStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryServiceAlertStatus.description;
                dokDeliveryGeodataLawStatus = (d.restricted.HasValue && d.restricted == true) ? UI.DOK_Delivery_Restricted : d.dokDeliveryGeodataLawStatus.description;

                MetadataUrl = d.MetadataUrl;
                ConfirmedDok = "NEI";
                Coverage = "NEI";
                if (filter != null && !string.IsNullOrEmpty(filter.municipality))
                {
                    Services.RegisterItem.RegisterItemService regItemService = new Services.RegisterItem.RegisterItemService(new RegisterDbContext());
                    Models.Organization org = regItemService.GetMunicipalOrganizationByNr(filter.municipality);
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
                var o = (Models.Organization)item;
                number = o.number;
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