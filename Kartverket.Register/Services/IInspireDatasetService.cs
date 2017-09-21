using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IInspireDatasetService
    {
        InspireDataset CreateNewInspireDataset(InspireDatasetViewModel inspireDatasetViewModel, string parentregister, string registername);
        InspireDatasetViewModel NewInspireDataset(string parentRegister, string register);
        InspireDataset GetInspireDatasetByName(string registerSeoName, string itemSeoName);
    }
}
