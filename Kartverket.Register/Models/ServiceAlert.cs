using GeoNorgeAPI;
using Kartverket.DOK.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Configuration;

namespace Kartverket.Register.Models
{
    public class ServiceAlert : RegisterItem
    {
        public ServiceAlert()
        {
            AlertDate = DateTime.Now;
            EffectiveDate = DateTime.Now;
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
        [Display(Name = "Eier:")]
        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }
        public virtual Organization Owner { get; set; }

        public string ServiceMetadataUrl { get; set; }

        [Required]
        public string ServiceUuid { get; set; }

        [Display(Name = "Varselet gjelder:")]
        public string Note { get; set; }


        public void GetMetadataByUuid()
        {
            SimpleMetadata metadata = MetadataService.FetchMetadata(ServiceUuid);
            name = metadata.Title;
            if (metadata.DistributionFormat != null) ServiceType = metadata.DistributionFormat.Name;
            ServiceMetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + ServiceUuid;
        }

        public List<string> GetAlertTypes()
        {
            return new List<string>() {
                "Endre URL",
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
                return "/tjenestevarsler/" + register.seoname + "/" + Owner.seoname + "/" + seoname + "/rediger";
            }
            else {
                return "/tjenestevarsler/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + Owner.seoname + "/" + seoname + "/rediger";

            }
        }

        public virtual string GetServiceAlertDeleteUrl()
        {
            if (register.parentRegister == null)
            {
                return "/tjenestevarsler/" + register.seoname + "/" + Owner.seoname + "/" + seoname + "/slett";
            }
            else {
                return "/tjenestevarsler/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + Owner.seoname + "/" + seoname + "/slett";

            }
        }

        public void InitializeNewServiceAlert()
        {
            InitializeNew();
            if (register != null)
            {
                registerId = register.systemId;
            }
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
            OwnerId = serviceAlert.OwnerId;
            Note = serviceAlert.Note;
        }
    }
}