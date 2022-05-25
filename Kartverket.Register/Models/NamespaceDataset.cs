using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class NamespaceDataset
    {
        [Key]
        public Guid SystemId { get; set; }

        public virtual NameSpace NameSpace { get; set; }
        [ForeignKey("NameSpace")]
        public Guid NameSpaceId { get; set; }

        public string MetadataUuid { get; set; }
        public string MetadataNavn { get; set; }
        public string Organisasjon { get; set; }
        public string DatasettId { get; set; }
        public string RedirectUrl { get; set; }
    }
}