using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Kartverket.Register.Models.Translations;
using Resources;

namespace Kartverket.Register.Helpers
{
    /// <summary>
    /// Prepares values for analytics capture. The search box is free text, so a term is
    /// normalised before it leaves the application and dropped entirely when it looks like
    /// it could carry personal data.
    /// </summary>
    public static class TelemetryHelper
    {
        /// <summary>
        /// Real search terms are short. Longer input is pasted content, which is where
        /// personal data realistically ends up.
        /// </summary>
        private const int MaxSearchTermLength = 100;

        /// <summary>Fødselsnummer and D-nummer, also written as 6 digits + space + 5.</summary>
        private static readonly Regex NationalIdentityNumber = new Regex(@"\d{6}\s?\d{5}");

        private static readonly Regex EmailAddress = new Regex(@"\S+@\S+\.\S+");
        private static readonly Regex ConsecutiveWhitespace = new Regex(@"\s+");

        /// <summary>
        /// Returns the search term normalised for analytics, or an empty string when it must
        /// not be captured. An empty result means "no term recorded", not "empty search".
        /// </summary>
        public static string SanitizeSearchTerm(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var term = ConsecutiveWhitespace.Replace(text.Trim(), " ").ToLowerInvariant();

            if (term.Length > MaxSearchTermLength)
                return string.Empty;

            if (NationalIdentityNumber.IsMatch(term) || EmailAddress.IsMatch(term))
                return string.Empty;

            return term;
        }

        /// <summary>
        /// Returns the search scope for analytics as a seoname, or an empty string for a
        /// search across all registers. Two things are normalised away: the search bar puts a
        /// translated label in the register field when no register is chosen, and a global
        /// search reports the register name where an in-register search reports the seoname.
        /// Slugifying is idempotent, so an already-slugified value passes through unchanged.
        /// </summary>
        public static string NormalizeSearchScope(string register)
        {
            if (string.IsNullOrWhiteSpace(register))
                return string.Empty;

            var scope = register.Trim();
            if (IsAllRegistersLabel(scope))
                return string.Empty;

            return RegisterUrls.MakeSeoFriendlyString(scope);
        }

        private static bool IsAllRegistersLabel(string scope)
        {
            foreach (var cultureCode in new[] { Culture.NorwegianCode, Culture.EnglishCode })
            {
                var label = Shared.ResourceManager.GetString("Search_AllRegisters", new CultureInfo(cultureCode));
                if (string.Equals(scope, label, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
