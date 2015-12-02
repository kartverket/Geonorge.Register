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
        Models.RegisterItem GetCurrentRegisterItem(string parentregister, string register, string name);
        Models.Version GetVersionGroup(Guid? versioningId);
        List<Models.RegisterItem> GetAllVersionsOfItembyVersioningId(Guid? versjonsGruppeId);
        List<Models.RegisterItem> GetAllVersionsOfItem(string parent, string register, string item);
        List<Models.RegisterItem> GetRegisterItemsFromOrganization(string parentname, string registername, string itemowner);
        Models.RegisterItem SetStatusId(Models.RegisterItem item, Models.RegisterItem originalItem);
        Guid NewVersioningGroup(Models.RegisterItem registerItem);
        Models.RegisterItem GetRegisterItem(string parentregister, string register, string item, int vnr = 1);
        bool validateName(Object model);
        void SaveNewRegisterItem(Models.RegisterItem registerItem);
        void SaveEditedRegisterItem(Models.RegisterItem registerItem);
        void SaveDeleteRegisterItem(Models.RegisterItem registerItem);
        SelectList GetRegisterSelectList(Guid registerId);
        SelectList GetDokStatusSelectList(string dokStatusId);
        SelectList GetSubmitterSelectList(Guid submitterId);
        SelectList GetOwnerSelectList(Guid ownerId);
        SelectList GetThemeGroupSelectList(string themeGroupId);
        SelectList GetBroaderItems();
        SelectList GetStatusSelectList(Models.RegisterItem registerItem);
        SelectList GetBroaderItems(Guid? broaderItemId);
    }
}
