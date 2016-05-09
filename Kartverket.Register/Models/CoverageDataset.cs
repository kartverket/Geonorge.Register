using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Register.Models
{
    public class CoverageDataset
    {
        [Key]
        public Guid CoverageId { get; set; }

        [Display(Name = "Kommune")]
        [ForeignKey("Municipality")]
        public Guid MunicipalityId { get; set; }
        public virtual Organization Municipality { get; set; }

        [Display(Name = "Bekreftet")]
        public bool ConfirmedDok { get; set; }

        [Display(Name = "Dekning")]
        public bool Coverage { get; set; }

        [ForeignKey("dataset")]
        [Display(Name = "Datasett")]
        public Guid DatasetId { get; set; }
        public virtual Dataset dataset { get; set; }

        [Display(Name = "Merknad")]
        public string Note { get; set; }

        [ForeignKey("CoverageDOKStatus")]
        [Display(Name = "DOK-status")]
        public string CoverageDOKStatusId { get; set; }
        public virtual DokStatus CoverageDOKStatus { get; set; }
    }
}