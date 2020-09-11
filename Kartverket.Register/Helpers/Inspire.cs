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

        public static string CreateCamelCase(string inspireTheme)
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

        public static bool IncludeInFilter(dynamic inspireRegisterItem, FilterParameters filter)
        {
            if (!string.IsNullOrEmpty(filter.filterOrganization))
                filter.filterOrganization = filter.filterOrganization.ToLower();

            dynamic item = GetInspireData(inspireRegisterItem);

            if (!string.IsNullOrEmpty(filter.InspireRegisteryType))
            {
                if (item is InspireDataset && filter.InspireRegisteryType == "service")
                    return false;
                else if(item is InspireDataService && filter.InspireRegisteryType == "dataset")
                    return false;
            }

            if (!string.IsNullOrEmpty(filter.filterOrganization) && !string.IsNullOrEmpty(filter.InspireAnnex))
            {
                if (filter.filterOrganization == inspireRegisterItem.Owner.seoname && filter.InspireAnnex == Inspire.AnnexI)
                    return Inspire.HaveThemeOfTypeAnnexI(item.InspireThemes);
                else if (filter.filterOrganization == inspireRegisterItem.Owner.seoname && filter.InspireAnnex == Inspire.AnnexII)
                    return Inspire.HaveThemeOfTypeAnnexII(item.InspireThemes);
                else if (filter.filterOrganization == inspireRegisterItem.Owner.seoname && filter.InspireAnnex == Inspire.AnnexIII)
                    return Inspire.HaveThemeOfTypeAnnexIII(item.InspireThemes);
                else
                    return false;

            }
            else if (!string.IsNullOrEmpty(filter.filterOrganization) && string.IsNullOrEmpty(filter.InspireAnnex))
            {
                return filter.filterOrganization == inspireRegisterItem.Owner.seoname;
            }
            else if (!string.IsNullOrEmpty(filter.InspireAnnex) && string.IsNullOrEmpty(filter.filterOrganization))
            {
                if (filter.InspireAnnex == Inspire.AnnexI)
                    return Inspire.HaveThemeOfTypeAnnexI(item.InspireThemes);
                else if (filter.InspireAnnex == Inspire.AnnexII)
                    return Inspire.HaveThemeOfTypeAnnexII(item.InspireThemes);
                else if (filter.InspireAnnex == Inspire.AnnexIII)
                    return Inspire.HaveThemeOfTypeAnnexIII(item.InspireThemes);
                else
                    return false;

            }
            else
            {
                return true;
            }
        }

        private static object GetInspireData(dynamic inspireRegisterItem)
        {
            var dataset = inspireRegisterItem as InspireDataset;
            if (dataset != null)
                return dataset;
            else
            {
                var service = inspireRegisterItem as InspireDataService;
                if (service != null)
                    return service;
            }

            return inspireRegisterItem;
        }

    }
}