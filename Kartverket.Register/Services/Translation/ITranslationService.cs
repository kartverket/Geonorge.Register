using Kartverket.Register.Models;
using Kartverket.Register.Models.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kartverket.Register.Services.Translation
{
    public interface ITranslationService
    {
        TranslationCollection<RegisterTranslation> AddMissingTranslations(TranslationCollection<RegisterTranslation> translations);
        TranslationCollection<CodelistValueTranslation> AddMissingTranslations(TranslationCollection<CodelistValueTranslation> translations);
    }
}
