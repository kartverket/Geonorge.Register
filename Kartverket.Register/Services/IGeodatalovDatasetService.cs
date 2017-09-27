using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IGeodatalovDatasetService
    {
        GeodatalovDataset GetGeodatalovDatasetByName(string registerSeoName, string itemSeoName);
        GeodatalovDatasetViewModel NewGeodatalovDatasetViewModel(string parentregister, string registername);
        GeodatalovDataset CreateNewGeodatalovDataset(GeodatalovDatasetViewModel geodatalovViewModel, string parentregister, string registername);
    }
}
