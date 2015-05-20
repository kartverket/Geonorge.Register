//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Kartverket.Register.Models.ViewModels
//{
//    public class RegisterViewModel
//    {
//        public virtual Register register { get; set; }
//        public virtual List<RegisterItem> registerItems { get; set; }

//        public RegisterViewModel(FilterItems filterRegister)
//        {
//            register = filterRegister.register;
//            registerItems = filterRegister.registerItems;
//        }
        
//        public RegisterViewModel(Register register)
//        {
//            this.register = register;
//            registerItems = register.items.ToList();        
//        }

//    }
//}