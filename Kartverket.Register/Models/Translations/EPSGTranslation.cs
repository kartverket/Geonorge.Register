using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class EPSGTranslation : Translation<EPSGTranslation>
    {
        public Guid RegisterItemId { get; set; }

        public string inspireRequirementDescription { get; set; }

        public EPSGTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}