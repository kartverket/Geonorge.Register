using GeoNorgeAPI;
using Kartverket.DOK.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;
using System.Linq;

namespace Kartverket.Register.Models
{
    public class ServiceAlert : RegisterItem
    {
        public ServiceAlert()
        {
            AlertDate = DateTime.Now;
            EffectiveDate = DateTime.Now.AddMonths(3);
        }

        [Required(ErrorMessage = "Varslingsdato er påkrevd")]
        [Display(Name = "Varslingsdato")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AlertDate { get; set; }

        [Required(ErrorMessage = "Ikrafttredelsesdato er påkrevd")]
        [Display(Name = "Ikrafttredelsesdato")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; }

        [Required(ErrorMessage = "Valg av type varsel er påkrevd")]
        [Display(Name = "Type varsel")]
        public string AlertType { get; set; }

        [Required(ErrorMessage = "Valg av tjeneste er påkrevd")]
        [Display(Name = "Tjenestetype")]
        public string ServiceType { get; set; }

        [Required]
        public string Owner { get; set; }

        public string ServiceMetadataUrl { get; set; }

        [Required(ErrorMessage = "Valg av tjeneste er påkrevd")]
        public string ServiceUuid { get; set; }

        [Required(ErrorMessage = "Det er påkrevd å skrive hva varselet gjelder")]
        [StringLength(500, MinimumLength = 3)]
        [Display(Name = "Varselet gjelder")]
        public string Note { get; set; }


        public void GetMetadataByUuid()
        {
            SimpleMetadata metadata = MetadataService.FetchMetadata(ServiceUuid);
            name = metadata.Title;
            if (metadata.DistributionDetails != null) ServiceType = metadata.DistributionDetails.Protocol;
            if (metadata.ContactOwner != null) Owner = metadata.ContactOwner.Organization;
            ServiceMetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + ServiceUuid;
        }

        public List<string> GetAlertTypes()
        {
            var typeList = 
             new List<string>() {
                "Endret URL",
                "Endret datakvalitet",
                "Endret datastruktur",
                "Ny tjeneste",
                "Fjernet tjeneste",
                "Endret datainnhold",
                "Endret kodelister"
            };
            return typeList.OrderBy(o => o).ToList();
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

        public string GetServiceAlertUrl()
        {
            if (register.parentRegisterId == Guid.Empty || register.parentRegister == null)
            {
                return "/register/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/" + systemId.ToString();
            }
            else {
                return "/subregister/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/" + systemId.ToString();
            }
        }
    }
}