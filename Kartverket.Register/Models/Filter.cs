using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class Filter
    {

        // Registeritem
        public Guid systemId { get; set; }
        public Guid? currentVersion { get; set; }
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
        
    }
}