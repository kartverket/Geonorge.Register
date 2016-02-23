using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class CreateDokMunicipalViewModel
    {
        public List<MetadataItemViewModel> SelectedList { get; set; }
        public List<MetadataItemViewModel> SearchResult { get; set; }
        public Organization DatasetOwner { get; set; }
        public Register Register { get; set; }
        public string MunicipalityCode { get; set; }

        public CreateDokMunicipalViewModel() {
            //SelectedList = new List<MetadataItemViewModel>();
            //SearchResult = new List<MetadataItemViewModel>();
        }

    }
}