using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class EPSGTranslation : Translation<EPSGTranslation>
    {
        public Guid RegisterItemId { get; set; }
        [Display(Name = "inspireRequirementDescription_Engelsk", ResourceType = typeof(UI))]
        public string InspireRequirementDescription { get; set; }

        public EPSGTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}