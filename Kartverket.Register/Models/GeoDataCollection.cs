﻿using System;
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
        [Display(Name = "Url", ResourceType = typeof(GeodataCollection))]
        public string Link { get; set; }
        [Display(Name = "AnchorText", ResourceType = typeof(GeodataCollection))] 
        public string LinkLabel { get; set; }
        [Display(Name = "Purpose", ResourceType = typeof(GeodataCollection))]
        public string Purpose { get; set; }
        [Display(Name = "Organization", ResourceType = typeof(GeodataCollection))]
        public Organization Organization { get; set; }

        [Display(Name = "Responsible", ResourceType = typeof(GeodataCollection))]
        public Organization Responsible { get; set; }

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
        [Display(Name = "Url", ResourceType = typeof(GeodataCollection))]
        public string ProcessHistory { get; set; }
        [Display(Name = "AnchorText", ResourceType = typeof(GeodataCollection))]
        public string ProcessHistoryLabel { get; set; }
        [Display(Name = "RegistrationRequirements", ResourceType = typeof(GeodataCollection))]
        public string RegistrationRequirements { get; set; }
        [Display(Name = "Url", ResourceType = typeof(GeodataCollection))]
        public string MappingRequirementsLink { get; set; }
        [Display(Name = "AnchorText", ResourceType = typeof(GeodataCollection))]
        public string MappingRequirementsLinkLabel { get; set; }
        [Display(Name = "MappingRequirementsLink", ResourceType = typeof(GeodataCollection))]
        public string MappingRequirements { get; set; }
        [Display(Name = "MethodologyDocumentLink", ResourceType = typeof(GeodataCollection))]
        public string MethodologyDocumentLink { get; set; }
        [Display(Name = "MethodologyLinkWebPage", ResourceType = typeof(GeodataCollection))]
        public string MethodologyLinkWebPage { get; set; }
        [Display(Name = "SupportSchemes", ResourceType = typeof(GeodataCollection))]
        public string SupportSchemes { get; set; }

        [Display(Name = "OtherOrganizationsInvolved", ResourceType = typeof(GeodataCollection))]
        public string OtherOrganizationsInvolved { get; set; }
        [Display(Name = "Url", ResourceType = typeof(GeodataCollection))]
        public string LinkToMapSolution { get; set; }
        [Display(Name = "AnchorText", ResourceType = typeof(GeodataCollection))]
        public string LinkToMapSolutionLabel { get; set; }
        [Display(Name = "Url", ResourceType = typeof(GeodataCollection))]
        public string LinkInfoPage { get; set; }
        [Display(Name = "AnchorText", ResourceType = typeof(GeodataCollection))]
        public string LinkInfoPageLabel { get; set; }

        [Display(Name = "AidAndSubsidies", ResourceType = typeof(GeodataCollection))]
        public string AidAndSubsidies { get; set; }

        [Display(Name = "OtherInfo", ResourceType = typeof(GeodataCollection))]
        public string OtherInfo { get; set; }

        [Display(Name = "LinkOtherInfo", ResourceType = typeof(GeodataCollection))]
        public string LinkOtherInfo { get; set; }
        [Display(Name = "MethodForMappingShort", ResourceType = typeof(GeodataCollection))]
        public string MethodForMappingShort { get; set; }
        [Display(Name = "Url", ResourceType = typeof(GeodataCollection))]
        public string OtherWebInfoAboutMappingMethodology { get; set; }
        [Display(Name = "AnchorText", ResourceType = typeof(GeodataCollection))]
        public string OtherWebInfoAboutMappingMethodologyLabel { get; set; }
        [Display(Name = "Url", ResourceType = typeof(GeodataCollection))]
        public string LinkToRequirementsForDelivery { get; set; }
        [Display(Name = "AnchorText", ResourceType = typeof(GeodataCollection))]
        public string LinkToRequirementsForDeliveryLabel { get; set; }
        [Display(Name = "OrganizationInfo", ResourceType = typeof(GeodataCollection))]
        public string OrganizationInfo { get; set; }
        [Display(Name = "ContactEmail", ResourceType = typeof(GeodataCollection))]
        public string ContactEmail { get; set; }
        [Display(Name = "Image", ResourceType = typeof(GeodataCollection))]
        public string ImageFileName { get; set; }
        public string ThumbnailFileName { get; set; }
        public string Owner { get; set; }
    }
}