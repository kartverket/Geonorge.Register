using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kartverket.Register.Services
{
    public interface IFairDatasetService
    {
        FairDataset GetFairDatasetByName(string registerSeoName, string itemSeoName);
        FairDatasetViewModel NewFairDatasetViewModel(string parentregister, string registername);
        FairDataset NewFairDataset(FairDatasetViewModel FairViewModel, string parentregister, string registername);
        FairDataset UpdateFairDatasetFromKartkatalogen(FairDataset FairDataset);
        FairDataset UpdateFairDataset(FairDatasetViewModel viewModel);
        void DeleteFairDataset(FairDataset FairDataset);
        FairDataset GetFairDatasetById(string uuid);
    }
}
