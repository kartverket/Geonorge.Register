using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Models.Translations;
using System.Data.Entity;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services.Translation
{
    public class TranslationService : ITranslationService
    {
        private readonly RegisterDbContext _dbContext;
        public TranslationService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public TranslationCollection<RegisterTranslation> AddMissingTranslations(TranslationCollection<RegisterTranslation> translations)
        {
            foreach (var language in Kartverket.Register.Models.Translations.Culture.Languages)
            {
                if (!translations.HasCulture(language.Key))
                    translations.Add(new RegisterTranslation { CultureName = language.Key });
            }

            return translations;
        }

        public TranslationCollection<CodelistValueTranslation> AddMissingTranslations(TranslationCollection<CodelistValueTranslation> translations)
        {
            foreach (var language in Kartverket.Register.Models.Translations.Culture.Languages)
            {
                if (!translations.HasCulture(language.Key))
                    translations.Add(new CodelistValueTranslation { CultureName = language.Key });
            }

            return translations;
        }
    }
}