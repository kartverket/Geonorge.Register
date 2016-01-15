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
        [Display(Name = "URL til tjeneste")]
        public string serviceUrl { get; set; }

        public virtual string GetNameSpaceEditUrl()
        {
            if (register.parentRegister == null)
            {
                return "/navnerom/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/rediger";
            }
            else {
                return "/navnerom/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/rediger";
            }
        }

        public virtual string GetNameSpaceDeleteUrl()
        {
            if (register.parentRegister == null)
            {
                return "/navnerom/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/slett";
            }
            else {
                return "/navnerom/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + submitter.seoname + "/" + seoname + "/slett";
            }
        }
    }
}