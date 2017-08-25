using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class RegisterTranslation : Translation<RegisterTranslation>
    {

        public Guid RegisterId { get; set; }

        public RegisterTranslation()
        {
            Id = Guid.NewGuid();
        }

    }
}