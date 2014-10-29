using System.ComponentModel.DataAnnotations;
using Resources;

namespace Kartverket.Register.Models
{
    public class Organization
    {
        [Key]
        [Display(Name="Organization_Number", ResourceType = typeof(UI) )]
        public string Number { get; set; }

        [Display(Name = "Organization_Name", ResourceType = typeof(UI))]
        public string Name { get; set; }

        [Display(Name = "Organization_Logo", ResourceType = typeof(UI))]
        public string LogoFilename { get; set; }
    }
}