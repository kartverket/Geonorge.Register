using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class ServiceAlertTranslation : Translation<ServiceAlertTranslation>
    {
        public Guid RegisterItemId { get; set; }

        [Display(Name = "AlertType", ResourceType = typeof(ServiceAlerts))]
        public string AlertType { get; set; }
        [Display(Name = "NoteTranslated", ResourceType = typeof(ServiceAlerts))]
        public string Note { get; set; }
        public string Owner { get; set; }

        public ServiceAlertTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}