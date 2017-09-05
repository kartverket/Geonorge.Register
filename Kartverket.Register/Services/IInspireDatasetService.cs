using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IInspireDatasetService
    {
        void CreateNewInspireDataset(InspireDatasetViewModel inspireDatasetViewModel, string parentregister, string registername);
    }
}
