using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class DocumentTranslation : Translation<DocumentTranslation>
    {
        public Guid RegisterItemId { get; set; }

        public DocumentTranslation()
        {
            Id = Guid.NewGuid();
        }
    }
}