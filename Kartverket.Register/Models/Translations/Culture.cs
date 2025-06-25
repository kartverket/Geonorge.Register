using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Translations
{
    public class Culture
    {
        public const string EnglishCode = "en";
        public const string EnglishName = "English";

        public const string NorwegianCode = "no";
        public const string NorwegianName = "Norsk";

        public const string NynorskCode = "nn";
        public const string NynorskName = "Nynorsk";

        public static readonly Dictionary<string, string> Languages = new Dictionary<string, string>()
        {
            {EnglishCode, EnglishName},
            {NynorskCode, NynorskName},
        };
    }
}