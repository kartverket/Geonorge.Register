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
        [Display(Name = "InspireRequirementDescription_Engelsk", ResourceType = typeof(EPSGs))]
        public string InspireRequirementDescription { get; set; }

        public EPSGTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}