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
        void UpdateTranslations(Models.Register register, Models.Register originalRegister);
        void UpdateTranslations(Models.CodelistValue register, Models.CodelistValue originalRegister);
        void UpdateTranslations(Models.EPSG register, Models.EPSG originalRegister);
    }
}
