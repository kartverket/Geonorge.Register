using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class MunicipalDatasetsViewModel
    {
        public MunicipalDatasetsViewModel() {
            DokMunicipalDatasets = new List<DokMunicipalRow>();
        }

        public List<DokMunicipalRow> DokMunicipalDatasets { get; set; }
    }
}