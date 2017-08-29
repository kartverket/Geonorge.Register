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

        public void UpdateTranslations(Models.Register register, Models.Register originalRegister)
        {
            originalRegister.Translations.Where(t => t.RegisterId != Guid.Empty).ToList().ForEach(x => _dbContext.Entry(x).State = EntityState.Deleted);
            originalRegister.Translations = register.Translations;
        }

        public void UpdateTranslations(CodelistValue register, CodelistValue originalRegister)
        {
            originalRegister.Translations.Where(t => t.RegisterItemId != Guid.Empty).ToList().ForEach(x => _dbContext.Entry(x).State = EntityState.Deleted);
            originalRegister.Translations = register.Translations;
        }

        public void UpdateTranslations(EPSG register, EPSG originalRegister)
        {
            originalRegister.Translations.Where(t => t.RegisterItemId != Guid.Empty).ToList().ForEach(x => _dbContext.Entry(x).State = EntityState.Deleted);
            originalRegister.Translations = register.Translations;
        }

        public void UpdateTranslations(Models.Organization register, Models.Organization originalRegister)
        {
            originalRegister.Translations.Where(t => t.RegisterItemId != Guid.Empty).ToList().ForEach(x => _dbContext.Entry(x).State = EntityState.Deleted);
            originalRegister.Translations = register.Translations;
        }
    }
}