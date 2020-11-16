using System;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IMareanoDatasetService
    {
        MareanoDataset GetMareanoDatasetByName(string registerSeoName, string itemSeoName);
        MareanoDatasetViewModel NewMareanoDatasetViewModel(string parentregister, string registername);
        MareanoDataset NewMareanoDataset(MareanoDatasetViewModel MareanoViewModel, string parentregister, string registername);
        MareanoDataset UpdateMareanoDatasetFromKartkatalogen(MareanoDataset MareanoDataset);
        MareanoDataset UpdateMareanoDataset(MareanoDatasetViewModel viewModel);
        void DeleteMareanoDataset(MareanoDataset MareanoDataset);
        MareanoDataset GetMareanoDatasetById(string uuid);
    }
}
