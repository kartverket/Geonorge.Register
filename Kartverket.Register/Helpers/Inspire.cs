using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Helpers
{
    public static class Inspire
    {
        public const string AnnexI = "AnnexI";
        public const string AnnexII = "AnnexII";
        public const string AnnexIII = "AnnexIII";

        public static bool HaveThemeOfTypeAnnexI(ICollection<CodelistValue> inspireThems)
        {
            try
            {
                foreach (var inspireTheme in inspireThems)
                {
                    if (IsAnnexI(inspireTheme))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        private static bool IsAnnexI(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(Eu.Europa.Ec.Jrc.Inspire.AnnexI), inspireThemeValueCamelCase);
            }
            return false;
        }

        private static string CreateCamelCase(string inspireTheme)
        {
            System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;

            var inspireThemeCamelCase = textInfo.ToTitleCase(inspireTheme);
            inspireThemeCamelCase = Char.ToLowerInvariant(inspireThemeCamelCase[0]) + inspireThemeCamelCase.Substring(1);
            inspireThemeCamelCase = inspireThemeCamelCase.Replace("/", " ");
            inspireThemeCamelCase = inspireThemeCamelCase.Replace(" ", "");
            inspireThemeCamelCase = inspireThemeCamelCase.Replace("-", "");

            return inspireThemeCamelCase;
        }

        public static bool HaveThemeOfTypeAnnexII(ICollection<CodelistValue> inspireThemes)
        {
            foreach (var inspireTheme in inspireThemes)
            {
                if (IsAnnexII(inspireTheme))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAnnexII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexII), inspireThemeValueCamelCase);
            }
            return false;
        }

        internal static bool HaveThemeOfTypeAnnexIII(ICollection<CodelistValue> inspireThemes)
        {
            foreach (var inspireTheme in inspireThemes)
            {
                if (IsAnnexIII(inspireTheme))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAnnexIII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexIII), inspireThemeValueCamelCase);
            }
            return false;
        }
    }
}