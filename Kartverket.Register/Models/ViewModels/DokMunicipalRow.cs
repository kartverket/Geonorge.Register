using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class DokMunicipalRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Themegroup { get; set; }
        public string Owner { get; set; }
        public Guid OwnerId { get; set; }
        public string NationalDokStatus { get; set; }
        public string Type { get; set; }
        public bool Confirmed { get; set; }
        public string Note { get; set; }

        public DokMunicipalRow(Dataset dataset, RegisterItem municipality)
        {
            Id = dataset.systemId;
            Name = dataset.name;
            Themegroup = dataset.ThemeGroupId;
            Owner = dataset.datasetowner.name;
            OwnerId = dataset.datasetownerId;
            if (dataset.IsNationalDataset())
            {
                NationalDokStatus = dataset.dokStatus.description;
            }
            Type = dataset.DatasetType;
            Confirmed = dataset.GetCoverageConfirmedByUser(municipality.systemId);
            Note = dataset.GetCoverageNoteByUser(municipality.systemId);
        }

        public DokMunicipalRow() {

        }
    }

}