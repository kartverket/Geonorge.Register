using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Kartverket.DOK.Models
{
    public class DokDataset
    {
        public int Id { get; set; }

        [Display(Name = "Uuid")]
        public string Uuid { get; set; }

        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Display(Name = "Eier / leverandør")]
        public string Publisher { get; set; }

        [Display(Name = "Merknad")]
        public string Notes { get; set; }

        [Display(Name = "Beskrivelse")]
        public string Description { get; set; }

        [Display(Name = "Produktark url")]
        public string ProductSheetUrl { get; set; }

        [Display(Name = "Presentasjonsregler url")]
        public string PresentationRulesUrl { get; set; }

        [Display(Name = "Produktspesifikasjon url")]
        public string ProductSpecificationUrl { get; set; }

        [Display(Name = "Metadata url")]
        public string MetadataUrl { get; set; }

        [Display(Name = "Distribusjonsformat")]
        public string DistributionFormat { get; set; }

        [Display(Name = "Distribusjon url")]
        public string DistributionUrl { get; set; }

        [Display(Name = "Distribusjonsområde")]
        public string DistributionArea { get; set; }

        [Display(Name = "WMS url")]
        public string WmsUrl { get; set; }

        [Display(Name = "Temagruppe")]
        public int ThemeGroupId { get; set; }
        public virtual ThemeGroup ThemeGroup { get; set; }

        [Display(Name = "Miniatyrbilde")]
        public string ThumbnailUrl { get; set; }

        [ForeignKey("status")]
        public string statusId { get; set; }
        public virtual Kartverket.Register.Models.Status status { get; set; }

        public string ShortDescription()
        {
            string d = Description;
            if (d.Length>80) d=d.Substring(0, 80);
            return d;
        }

        public string ToSeoUrl()
        {
            // make the url lowercase
            string encodedUrl = (Name ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "a").Replace("æ","ae").Replace("ø", "o");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }
    }
}