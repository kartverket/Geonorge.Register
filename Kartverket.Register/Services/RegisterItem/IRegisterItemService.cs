using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Services.RegisterItem
{
    public interface IRegisterItemService
    {
        void SetNarrowerItems(List<Guid> narrowerItems, CodelistValue codelistValue);
        void SetBroaderItem(Guid broader, CodelistValue codelistValue);
        void SetBroaderItem(CodelistValue codelistValue);
        void RemoveBroaderAndNarrower(CodelistValue codelistValue);
        Models.RegisterItem GetCurrentRegisterItem(string register, string name);
        Models.RegisterItem GetCurrentSubregisterItem(string parentregister, string register, string name);
        Models.Version GetVersionGroup(Guid? versioningId);
        Models.RegisterItem GetRegisterItemByVersionNr(string register, string item, int? vnr);
        Models.RegisterItem GetSubregisterItemByVersionNr(string parentRegister, string register, string item, int? vnr);
        List<Models.RegisterItem> GetAllVersionsOfItembyVersioningId(Guid? versjonsGruppeId);
        List<Models.RegisterItem> GetAllVersionsOfItem(string parent, string register, string item);
        List<Models.RegisterItem> GetRegisterItemsFromOrganization(string parentname, string registername, string itemowner);
        Models.RegisterItem SetStatusId(Models.RegisterItem item, Models.RegisterItem originalItem);
        Guid NewVersioningGroup(Models.RegisterItem registerItem, Models.Register register);
        Models.RegisterItem GetRegisterItemByVersionNr(string parentregister, string registername, string documentname, int versionNumber);
        Models.RegisterItem GetRegisterItem(string parentregister, string register, string item, int vnr = 1);
        bool validateName(Object model);
    }
}
