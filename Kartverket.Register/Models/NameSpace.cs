using Kartverket.Register.Models.Translations;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class NameSpace : RegisterItem 
    {
        public NameSpace()
        {
            this.Translations = new TranslationCollection<NamespaceTranslation>();
        }
        [Display(Name = "ServiceUrl", ResourceType = typeof(Namespace))]
        public string serviceUrl { get; set; }

        public virtual string GetNameSpaceEditUrl()
        {
            if (register.parentRegister == null)
            {
                return "/navnerom/" + systemId + "/rediger";
            }
            else {
                return "/navnerom/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/rediger";
            }
        }

        public virtual string GetNameSpaceDeleteUrl()
        {
            if (register.parentRegister == null)
            {
                return "/navnerom/" + systemId + "/slett";
            }
            else {
                return "/navnerom/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/slett";
            }
        }

        public virtual TranslationCollection<NamespaceTranslation> Translations { get; set; }
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
    }
}