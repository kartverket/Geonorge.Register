using System.Collections.Generic;
using System.Web.Mvc;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IDatasetService
    {
        Dataset GetDatasetByUuid(string uuid);
        SelectList GetDokStatusSelectList(string statusId);
        Dataset UpdateDataset(Dataset dataset, Dataset originalDataset = null, CoverageDataset coverage = null);
        List<CoverageDataset> EditDatasetCoverage(CoverageDataset coverage, Dataset dataset, Dataset originalDataset = null);
    }
}
