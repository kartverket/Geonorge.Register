using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Register.Models
{
    public class CoverageDataset
    {
        [Key]
        public Guid CoverageId { get; set; }

        [ForeignKey("Municipality")]
        public Guid MunicipalityId { get; set; }
        public virtual Organization Municipality { get; set; }

        public bool ConfirmedDok { get; set; }

        [ForeignKey("dataset")]
        public Guid DatasetId { get; set; }
        public virtual Dataset dataset { get; set; }

        public string Note { get; set; }

        [ForeignKey("CoverageDOKStatus")]
        public string CoverageDOKStatusId { get; set; }
        public virtual DokStatus CoverageDOKStatus { get; set; }
    }
}