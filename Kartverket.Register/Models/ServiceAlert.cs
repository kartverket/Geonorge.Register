using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Register.Models
{
    public class ServiceAlert : RegisterItem
    {
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AlertDate { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; }
        public string AlertType { get; set; }
        public string ServiceType { get; set; }
        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }
        public virtual Organization Owner { get; set; }
        public string Note { get; set; }
        public string ServiceMetadataUrl { get; set; }
        public string ServiceUuid { get; set; }

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

        public virtual string GetDocumentEditUrl()
        {
            if (register.parentRegister == null)
            {
                return "/tjenestevarsler/" + register.seoname + "/" + Owner.seoname + "/" + seoname + "/rediger";
            }
            else {
                return "/tjenestevarsler/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + register.seoname + "/" + Owner.seoname + "/" + seoname + "/rediger";

            }
        }

        public virtual string GetDocumentDeleteUrl()
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
    }
}