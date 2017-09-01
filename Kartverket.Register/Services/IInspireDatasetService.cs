using Kartverket.Register.Models;
using System.Collections.Generic;

namespace Kartverket.Register.Services
{
    public interface IInspireDatasetService
    {
        void CreateNewInspireDataset(InspireDataset inspireDataset, string parentregister, string registername);
    }
}
