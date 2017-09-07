using System.Web.Mvc;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IDatasetService
    {
        Dataset GetDatasetByUuid(string uuid);
        SelectList GetDokStatusSelectList(string statusId);
    }
}
