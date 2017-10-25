using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class AccessViewModel
    {
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Add { get; set; }
        public bool EditListOfRegisterItems { get; set; }
    }
}