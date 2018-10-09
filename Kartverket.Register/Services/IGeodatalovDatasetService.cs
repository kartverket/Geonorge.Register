using System;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IGeodatalovDatasetService
    {
        GeodatalovDataset GetGeodatalovDatasetByName(string registerSeoName, string itemSeoName);
        GeodatalovDatasetViewModel NewGeodatalovDatasetViewModel(string parentregister, string registername);
        GeodatalovDataset NewGeodatalovDataset(GeodatalovDatasetViewModel geodatalovViewModel, string parentregister, string registername);
        GeodatalovDataset UpdateGeodatalovDatasetFromKartkatalogen(GeodatalovDataset geodatalovDataset);
        GeodatalovDataset UpdateGeodatalovDataset(GeodatalovDatasetViewModel viewModel);
        void DeleteGeodatalovDataset(GeodatalovDataset geodatalovDataset);
        GeodatalovDataset GetGeodatalovDatasetById(string uuid);
    }
}
