using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class DokMunicipalRow
    {
        private Dataset dataset;

        public DokMunicipalRow(Dataset dataset)
        {
            Id = dataset.systemId;
            Name = dataset.name;
            Themegroup = dataset.ThemeGroupId;
            Owner = dataset.datasetowner.name;
            if (dataset.IsNationalDataset())
            {
                NationalDokStatus = dataset.dokStatus.description;
            }
            Type = dataset.DatasetType;
            Confirmed = dataset.GetCoverageConfirmedByUser(dataset.datasetownerId);
            Note = dataset.GetCoverageNoteByUser(dataset.datasetownerId);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Themegroup { get; set; }
        public string Owner { get; set; }
        public string NationalDokStatus { get; set; }
        public string Type { get; set; }
        public bool Confirmed { get; set; }
        public string Note { get; set; } 
    }
}