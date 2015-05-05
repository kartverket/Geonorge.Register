using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class FilteringResultItemViewModel
    {
        // Registeritem
        public Guid systemId { get; set; }
        public Guid currentVersion { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Guid? submitterId { get; set; }
        public virtual Organization submitter { get; set; }
        public DateTime dateSubmitted { get; set; }
        public DateTime modified { get; set; }
        public string statusId { get; set; }
        public virtual Status status { get; set; }
        public DateTime? dateAccepted { get; set; }
        public virtual Register register { get; set; }
        public Guid registerId { get; set; }
        public string seoname { get; set; }

        //Document
        public string thumbnail { get; set; }
        public Guid documentownerId { get; set; }
        public virtual Organization documentowner { get; set; }
        public string documentUrl { get; set; }

        // Dataset
        public string Uuid { get; set; }
        public Guid datasetownerId { get; set; }
        public virtual Organization datasetowner { get; set; }
        public string Notes { get; set; }
        public string ProductSheetUrl { get; set; }
        public string PresentationRulesUrl { get; set; }
        public string ProductSpecificationUrl { get; set; }
        public string MetadataUrl { get; set; }
        public string DistributionFormat { get; set; }
        public string DistributionUrl { get; set; }
        public string DistributionArea { get; set; }
        public string WmsUrl { get; set; }
        public string ThemeGroupId { get; set; }
        public virtual DOKTheme theme { get; set; }
        public string datasetthumbnail { get; set; }


        private FilteringResultItemViewModel(Filter item)
        {
            systemId = item.systemId;
            currentVersion = item.currentVersion;
            name = item.name;
            description = item.description;
            submitterId = item.submitterId;
            submitter = item.submitter;
            dateSubmitted = item.dateSubmitted;
            modified = item.modified;
            statusId = item.statusId;
            status = item.status;
            dateAccepted = item.dateAccepted;
            register = item.register;
            registerId = item.registerId;
            seoname = item.seoname;
            thumbnail = item.thumbnail;
            documentowner = item.documentowner;
            documentownerId = item.documentownerId;
            documentUrl = item.documentUrl;
            datasetownerId = item.datasetownerId;
            datasetowner = item.datasetowner;
            datasetthumbnail = item.datasetthumbnail;
            DistributionArea = item.DistributionArea;
            DistributionFormat = item.DistributionFormat;
            DistributionUrl = item.DistributionUrl;
            MetadataUrl = item.MetadataUrl;
            Notes = item.Notes;
            PresentationRulesUrl = item.PresentationRulesUrl;
            ProductSheetUrl = item.ProductSheetUrl;
            ProductSpecificationUrl = item.ProductSpecificationUrl;
            theme = item.theme;
            ThemeGroupId = item.ThemeGroupId;
            Uuid = item.Uuid;
            WmsUrl = item.WmsUrl;
        }

        public static List<FilteringResultItemViewModel> CreateFromListFilter(IEnumerable<Filter> items)
        {
            return items.Select(item => new FilteringResultItemViewModel(item)).ToList();
        }
    }
}