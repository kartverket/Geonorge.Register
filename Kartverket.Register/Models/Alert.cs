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
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Register.Models
{
    public class Alert : RegisterItem
    {
        public Alert(string category = null)
        {
            AlertDate = DateTime.Now;
            if (category == "Driftsmelding")
                EffectiveDate = AlertDate;
            else
                EffectiveDate = DateTime.Now.AddMonths(3);
            Translations = new TranslationCollection<AlertTranslation>();
        }
        public Alert() { }

        [Required(ErrorMessageResourceType = typeof(Alerts), ErrorMessageResourceName = "AlertDateErrorMessage")]
        [Display(Name = "AlertDate", ResourceType = typeof(Alerts))]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime AlertDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Alerts), ErrorMessageResourceName = "EffectiveDateErrorMessage")]
        [Display(Name = "EffectiveDate", ResourceType = typeof(Alerts))]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; } //validFrom

        [Required(ErrorMessageResourceType = typeof(Alerts), ErrorMessageResourceName = "AlertTypeErrorMessage")]
        [Display(Name = "AlertType", ResourceType = typeof(Alerts))]
        public string AlertType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Alerts), ErrorMessageResourceName = "TypeErrorMessage")]
        [Display(Name = "Type", ResourceType = typeof(Alerts))]
        public string Type { get; set; }

        public string AlertCategory { get; set; }

        [Required]
        public string Owner { get; set; }

        public string UrlExternal{ get; set; }

        [Required(ErrorMessageResourceType = typeof(Alerts), ErrorMessageResourceName = "ErrorMessage")]
        public string UuidExternal { get; set; }

        [Required(ErrorMessageResourceType = typeof(Alerts), ErrorMessageResourceName = "NoteErrorMessage")]
        [StringLength(500, MinimumLength = 3)]
        [Display(Name = "Note", ResourceType = typeof(Alerts))]
        public string Note { get; set; }

        [Display(Name = "Tags", ResourceType = typeof(Alerts))]
        public virtual ICollection<Tag> Tags { get; set; }

        [ForeignKey("department")]
        public string departmentId { get; set; }
        [Display(Name = "Department", ResourceType = typeof(Alerts))]
        public virtual Department department { get; set; }

        [Display(Name = "Department", ResourceType = typeof(Alerts))]
        public virtual ICollection<Department> Departments { get; set; }

        public string StationName { get; set; }
        public string StationType { get; set; }
        [ForeignKey("StationName,StationType")]
        [Display(Name = "Station", ResourceType = typeof(Alerts))]
        public virtual Station station { get; set; }

        [Display(Name = "Summary", ResourceType = typeof(Alerts))]
        public string Summary { get; set; }
        public string Link { get; set; }
        [Display(Name = "Image1", ResourceType = typeof(Alerts))]
        public string Image1 { get; set; }
        [Display(Name = "Image2", ResourceType = typeof(Alerts))]
        public string Image2 { get; set; }
        public string Image1Thumbnail { get; set; }
        public string Image2Thumbnail { get; set; }
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "DateResolved", ResourceType = typeof(Alerts))]
        public DateTime? DateResolved { get; set; } // validTo


        public void GetMetadataByUuid()
        {
            SimpleMetadata metadata = MetadataService.FetchMetadata(UuidExternal);
            Type = "Ikke satt";
            if (metadata != null)
            {
                name = metadata.Title;
                if (Translations.Count == 0)
                    Translations.Add(new AlertTranslation());
                Translations[0].Name = metadata.EnglishTitle;
                if (metadata.DistributionDetails != null && metadata.DistributionDetails?.Protocol != null) 
                {
                    Type = metadata.DistributionDetails.Protocol;
                }

                if (metadata.ContactOwner != null)
                {
                    Owner = metadata.ContactOwner.Organization;
                    Translations[0].Owner = metadata.ContactOwner.OrganizationEnglish;
                }
                UrlExternal = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + UuidExternal;
            }
        }

        public virtual TranslationCollection<AlertTranslation> Translations { get; set; }
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

        public virtual string GetAlertEditUrl()
        {
            if (register.parentRegister == null)
            {
                return "/varsler/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/rediger";
            }
            else {
                return "/varsler/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/rediger";

            }
        }

        public virtual string GetAlertDeleteUrl()
        {
            if (register.parentRegister == null)
            {
                return "/varsler/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/slett";
            }
            else {
                return "/varsler/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/slett";

            }
        }

        public void InitializeNewAlert()
        {
            InitializeNew(false);
        }

        public void UpdateAlert(Alert alert)
        {
            alert.GetMetadataByUuid();
            name = alert.name;
            UuidExternal = alert.UuidExternal;
            UrlExternal = alert.UrlExternal;
            AlertDate = alert.AlertDate;
            AlertType = alert.AlertType;
            EffectiveDate = alert.EffectiveDate;
            Note = alert.Note;
        }

        public string GetAlertUrl()
        {
            return register.GetObjectUrl() + "/" + seoname + "/" + systemId;
        }
    }
}