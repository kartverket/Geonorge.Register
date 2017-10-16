using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class NamespaceTranslation : Translation<NamespaceTranslation>
    {
        public Guid RegisterItemId { get; set; }

        public NamespaceTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}