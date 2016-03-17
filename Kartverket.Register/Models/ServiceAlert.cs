using GeoNorgeAPI;
using Kartverket.DOK.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;

namespace Kartverket.Register.Models
{
    public class ServiceAlert : RegisterItem
    {
        public ServiceAlert()
        {
            AlertDate = DateTime.Now;
            EffectiveDate = DateTime.Now.AddMonths(3);
        }

        [Required]
        [Display(Name = "Varslingsdato:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AlertDate { get; set; }

        [Required]
        [Display(Name = "Ikrafttredelsesdato:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [Display(Name = "Type varsel:")]
        public string AlertType { get; set; }

        [Required]
        [Display(Name = "Tjenestetype:")]
        public string ServiceType { get; set; }

        [Required]
        public string Owner { get; set; }

        public string ServiceMetadataUrl { get; set; }

        [Required]
        public string ServiceUuid { get; set; }

        [Display(Name = "Varselet gjelder:")]
        public string Note { get; set; }


        public void GetMetadataByUuid()
        {
            SimpleMetadata metadata = MetadataService.FetchMetadata(ServiceUuid);
            name = metadata.Title;
            if (metadata.DistributionDetails != null) ServiceType = metadata.DistributionFormat.Name;
            if (metadata.ContactOwner != null) Owner = metadata.ContactOwner.Organization;
            ServiceMetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + ServiceUuid;
        }

        public List<string> GetAlertTypes()
        {
            return new List<string>() {
                "Endret URL",
                "Endre datakvalitet",
                "Endre datastruktur",
                "Ny tjeneste",
                "Fjernet tjeneste"
            };
        }

        public virtual string GetServiceAlertEditUrl()
        {
            if (register.parentRegister == null)
            {
                return "/tjenestevarsler/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/rediger";
            }
            else {
                return "/tjenestevarsler/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/rediger";

            }
        }

        public virtual string GetServiceAlertDeleteUrl()
        {
            if (register.parentRegister == null)
            {
                return "/tjenestevarsler/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/slett";
            }
            else {
                return "/tjenestevarsler/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/slett";

            }
        }

        public void InitializeNewServiceAlert()
        {
            InitializeNew();
        }

        public void UpdateServiceAlert(ServiceAlert serviceAlert)
        {
            serviceAlert.GetMetadataByUuid();
            name = serviceAlert.name;
            ServiceUuid = serviceAlert.ServiceUuid;
            ServiceMetadataUrl = serviceAlert.ServiceMetadataUrl;
            AlertDate = serviceAlert.AlertDate;
            AlertType = serviceAlert.AlertType;
            EffectiveDate = serviceAlert.EffectiveDate;
            Note = serviceAlert.Note;
        }
    }
}