using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class CodelistValueTranslation : Translation<CodelistValueTranslation>
    {
        public Guid RegisterItemId { get; set; }

        public string name { get; set; }

        public CodelistValueTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}