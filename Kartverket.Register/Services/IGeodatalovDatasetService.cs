using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IGeodatalovDatasetService
    {
        GeodatalovDataset GetGeodatalovDatasetByName(string registerSeoName, string itemSeoName);
    }
}
