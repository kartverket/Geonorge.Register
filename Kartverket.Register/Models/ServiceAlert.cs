using GeoNorgeAPI;
using Kartverket.DOK.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;
using System.Linq;
using Resources;
using Kartverket.Register.Models.Translations;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Models
{
    public class ServiceAlert : RegisterItem
    {
        public ServiceAlert()
        {
            AlertDate = DateTime.Now;
            EffectiveDate = DateTime.Now.AddMonths(3);
            Translations = new TranslationCollection<ServiceAlertTranslation>();
        }

        [Required(ErrorMessageResourceType = typeof(ServiceAlerts), ErrorMessageResourceName = "AlertDateErrorMessage")]
        [Display(Name = "AlertDate", ResourceType = typeof(ServiceAlerts))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AlertDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ServiceAlerts), ErrorMessageResourceName = "EffectiveDateErrorMessage")]
        [Display(Name = "EffectiveDate", ResourceType = typeof(ServiceAlerts))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ServiceAlerts), ErrorMessageResourceName = "AlertTypeErrorMessage")]
        [Display(Name = "AlertType", ResourceType = typeof(ServiceAlerts))]
        public string AlertType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ServiceAlerts), ErrorMessageResourceName = "ServiceTypeErrorMessage")]
        [Display(Name = "ServiceType", ResourceType = typeof(ServiceAlerts))]
        public string ServiceType { get; set; }

        [Required]
        public string Owner { get; set; }

        public string ServiceMetadataUrl { get; set; }

        [Required(ErrorMessageResourceType = typeof(ServiceAlerts), ErrorMessageResourceName = "ServiceTypeErrorMessage")]
        public string ServiceUuid { get; set; }

        [Required(ErrorMessageResourceType = typeof(ServiceAlerts), ErrorMessageResourceName = "NoteErrorMessage")]
        [StringLength(500, MinimumLength = 3)]
        [Display(Name = "Note", ResourceType = typeof(ServiceAlerts))]
        public string Note { get; set; }


        public void GetMetadataByUuid()
        {
            SimpleMetadata metadata = MetadataService.FetchMetadata(ServiceUuid);
            name = metadata.Title;
            Translations[0].Name = metadata.EnglishTitle;
            if (metadata.DistributionDetails != null) ServiceType = metadata.DistributionDetails.Protocol;
            if (metadata.ContactOwner != null)
            {
                Owner = metadata.ContactOwner.Organization;
                Translations[0].Owner = metadata.ContactOwner.OrganizationEnglish;
            }
            ServiceMetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + ServiceUuid;
        }

        public virtual TranslationCollection<ServiceAlertTranslation> Translations { get; set; }
        public void AddMissingTranslations()
        {
            Translations.AddMissingTranslations();
        }

        public new string NameTranslated()
        {
            return base.NameTranslated();
        }

        public new string DescriptionTranslated()
        {
            return base.DescriptionTranslated();
        }

        public string AlertTypeTranslated()
        {
            var cultureName = CultureHelper.GetCurrentCulture();
            var alertTypeTranslated = AlertType;
            alertTypeTranslated = this.Translations[cultureName]?.AlertType;
            if (string.IsNullOrEmpty(alertTypeTranslated))
                alertTypeTranslated = AlertType;

            return alertTypeTranslated;
        }

        public string NoteTranslated()
        {
            var cultureName = CultureHelper.GetCurrentCulture();
            var noteTranslated = Note;
            noteTranslated = this.Translations[cultureName]?.Note;
            if (string.IsNullOrEmpty(noteTranslated))
                noteTranslated = Note;

            return noteTranslated;
        }

        public string OwnerTranslated()
        {
            var cultureName = CultureHelper.GetCurrentCulture();
            var ownerTranslated = Owner;
            ownerTranslated = this.Translations[cultureName]?.Owner;
            if (string.IsNullOrEmpty(ownerTranslated))
                ownerTranslated = Owner;

            return ownerTranslated;
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