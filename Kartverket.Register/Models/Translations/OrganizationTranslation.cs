using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class OrganizationTranslation : Translation<OrganizationTranslation>
    {
        public Guid RegisterItemId { get; set; }

        public OrganizationTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}