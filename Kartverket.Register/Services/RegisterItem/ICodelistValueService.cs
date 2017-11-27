using System.IO;
using System.Web;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services.RegisterItem
{
    public interface ICodelistValueService
    {
        CodelistValue NewCodelistValueFromImport(Models.Register register, string[] codelistValueImport);
    }
}
