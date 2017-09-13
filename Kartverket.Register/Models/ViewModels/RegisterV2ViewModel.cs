using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kartverket.Register.Models.Translations;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterV2ViewModel
    {
        public Register Register { get; set; }

        public ICollection<RegisterItemV2ViewModel> RegisterItems { get; set; }

        public string OrderBy { get; set; }


        public RegisterV2ViewModel(Register register)
        {
            Register = register;
            RegisterItems = GetRegisterItems(register.containedItemClass, register.RegisterItems);

            if (register.IsServiceAlertRegister())
            {
                if (string.IsNullOrWhiteSpace(OrderBy))
                {
                    OrderBy = "dateSubmitted_desc";
                }
            }
        }

        private Collection<RegisterItemV2ViewModel> GetRegisterItems(string containedItemClass, ICollection<RegisterItemV2> registerItems)
        {
            var registerItemsViewModel = new Collection<RegisterItemV2ViewModel>();
            switch (containedItemClass)
            {
                case "InspireDataset":
                    foreach (InspireDataset inspireDataset in registerItems)
                    {
                        registerItemsViewModel.Add(new InspireDatasetViewModel(inspireDataset));
                    }
                    break;
            }
            return registerItemsViewModel;
        }
    }
}
