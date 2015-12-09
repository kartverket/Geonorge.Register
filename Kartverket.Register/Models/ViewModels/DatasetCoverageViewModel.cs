using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class DatasetCoverageViewModel
    {
        public string ThemeGroupName { get; set; }
        public string DatasetName { get; set; }
        public string DatasetUrl { get; set; }

        public string FullName
        {
            get { return ThemeGroupName + ": " + DatasetName; }
        } 
    }
}