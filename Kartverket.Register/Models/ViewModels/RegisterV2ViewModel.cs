using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        private ICollection<RegisterItemV2ViewModel> GetRegisterItems(string containedItemClass, ICollection<RegisterItemV2> registerItems)
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
                case "GeodatalovDataset":
                    foreach (GeodatalovDataset geodatalovDataset in registerItems)
                    {
                        registerItemsViewModel.Add(new GeodatalovDatasetViewModel(geodatalovDataset));
                    }
                    break;
            }
            return registerItemsViewModel;
        }
    }
}
