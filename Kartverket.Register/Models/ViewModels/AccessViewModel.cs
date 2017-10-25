using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class AccessViewModel
    {
        public bool EditRegister { get; set; }
        public bool DeleteRegister { get; set; }
        public bool AddToRegister { get; set; }
        public bool EditRegisterItems { get; set; }
    }
}