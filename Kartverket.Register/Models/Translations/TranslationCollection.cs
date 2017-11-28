using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Kartverket.Register.Models.Translations;

namespace Kartverket.Register.Models.Translations
{
    public class TranslationCollection<T> : Collection<T> where T : Translation<T>, new()
    {

        public T this[string culture]
        {
            get
            {
                var translation = this.FirstOrDefault(x => x.CultureName == culture);

                return translation;
            }
            set
            {
                var translation = this.FirstOrDefault(x => x.CultureName == culture);
                if (translation != null)
                {
                    Remove(translation);
                }

                value.CultureName = culture;
                Add(value);
            }
        }

        public bool HasCulture(string culture)
        {
            return this.Any(x => x.CultureName == culture);
        }

        public void AddMissingTranslations()
        {
            foreach (var language in Culture.Languages)
            {
                if (!this.HasCulture(language.Key))
                    Add(new T { CultureName = language.Key });
            }
        }

    }
}