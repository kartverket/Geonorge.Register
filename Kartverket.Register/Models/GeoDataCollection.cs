using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Kartverket.Register.Resources;

namespace Kartverket.Register.Models
{
    public class GeoDataCollection
    {
        [Key]
        public Guid systemId { get; set; }
        [Display(Name = "Title", ResourceType = typeof(GeodataCollection))]
        public string Title { get; set; }
        [Index("SeoName", IsUnique = true), StringLength(255)]
        public string SeoName { get; set; }
        [Display(Name = "Link", ResourceType = typeof(GeodataCollection))]
        public string Link { get; set; }
        [Display(Name = "Purpose", ResourceType = typeof(GeodataCollection))]
        public string Purpose { get; set; }
        [Display(Name = "Organization", ResourceType = typeof(GeodataCollection))]
        public Organization Organization { get; set; }

        [Display(Name = "DatasetTitle", ResourceType = typeof(GeodataCollection))]
        public string DatasetTitle { get; set; }
        [Display(Name = "DatasetLink", ResourceType = typeof(GeodataCollection))]
        public string DatasetLink { get; set; }
        [Display(Name = "Mapper", ResourceType = typeof(GeodataCollection))]
        public string Mapper { get; set; }
        [Display(Name = "DataOwner", ResourceType = typeof(GeodataCollection))]
        public string DataOwner { get; set; }
        [Display(Name = "Distributor", ResourceType = typeof(GeodataCollection))]
        public string Distributor { get; set; }
        [Display(Name = "Methodology", ResourceType = typeof(GeodataCollection))]
        public string Methodology { get; set; }
        [Display(Name = "ProcessHistory", ResourceType = typeof(GeodataCollection))]
        public string ProcessHistory { get; set; }
        [Display(Name = "RegistrationRequirements", ResourceType = typeof(GeodataCollection))]
        public string RegistrationRequirements { get; set; }
        [Display(Name = "MappingRequirements", ResourceType = typeof(GeodataCollection))]
        public string MappingRequirements { get; set; }
        [Display(Name = "MethodologyDocumentLink", ResourceType = typeof(GeodataCollection))]
        public string MethodologyDocumentLink { get; set; }
        [Display(Name = "MethodologyLinkWebPage", ResourceType = typeof(GeodataCollection))]
        public string MethodologyLinkWebPage { get; set; }
        [Display(Name = "SupportSchemes", ResourceType = typeof(GeodataCollection))]
        public string SupportSchemes { get; set; }


    }
}