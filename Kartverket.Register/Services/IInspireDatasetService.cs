﻿using System;
using System.Collections.Generic;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IInspireDatasetService
    {
        InspireDataset GetInspireDatasetByName(string registerSeoName, string itemSeoName);
        InspireDataset NewInspireDataset(InspireDatasetViewModel inspireDatasetViewModel, string parentregister, string registername);
        InspireDatasetViewModel NewInspireDatasetViewModel(string parentRegister, string register);
        InspireDataset UpdateInspireDataset(InspireDatasetViewModel viewModel);
        InspireDataset UpdateInspireDatasetFromKartkatalogen(InspireDataset inspireDataset);
        void DeleteInspireDataset(InspireDataset inspireDataset);
        List<InspireDataService> GetInspireDataService();
        ICollection<InspireDataServiceViewModel> ConvertToViewModel(ICollection<InspireDataService> getInspireDataService);
        InspireDataService GetInspireDataServiceByName(string registername, string itemname);
        InspireDataService GetInspireDataServiceById(string systemId);
        InspireDataService UpdateInspireDataService(InspireDataServiceViewModel viewModel);
        InspireDataService UpdateInspireDataServiceFromKartkatalogen(InspireDataService originalInspireDataService);
        void DeleteInspireDataService(InspireDataService inspireDataset);
        InspireDataset GetInspireDatasetById(string systemId);
    }
}
