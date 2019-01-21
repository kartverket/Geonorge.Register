using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class AlertTranslation : Translation<AlertTranslation>
    {
        public Guid RegisterItemId { get; set; }

        [Display(Name = "AlertType", ResourceType = typeof(Alerts))]
        public string AlertType { get; set; }
        [Display(Name = "NoteTranslated", ResourceType = typeof(Alerts))]
        public string Note { get; set; }
        public string Owner { get; set; }

        public AlertTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}