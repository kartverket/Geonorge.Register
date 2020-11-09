using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Services.Register
{
    public interface IRegisterService
    {
        Models.Register FilterRegisterItems(Models.Register register, FilterParameters filter);
        string ContentNegotiation(ControllerContext context);
        Models.Register GetRegisterByName(string name);
        Models.Register GetSubregisterByName(string parentName, string registerName);
        List<Models.Register> GetRegisters();
        List<Models.Register> GetSubregisters(Models.Register register);
        Models.Register GetRegisterBySystemId(Guid systemId);
        Models.Register GetRegister(string parentRegister, string register);
        Guid GetRegisterId(string parentRegister, string register);
        Models.Register SetStatus(Models.Register register, Models.Register originalRegister);
        bool RegisterHasChildren(string parentname, string registername);
        bool validationName(Object model);
        Organization GetOrganizationByUserName();
        Guid GetOrganizationIdByUserName();
        Organization GetOrganizationByOrganizationNr(string number);
        Organization GetOrganizationByMunicipalityCode(string municipalityCode);
        List<Models.RegisterItem> GetDatasetBySelectedMunicipality(Models.Register register, Models.RegisterItem municipality);
        Models.Register GetDokMunicipalRegister();
        void UpdateDOKStatus();
        string GetDOKStatus(string url, bool autoUpdate, string currentStatus);
        string GetDOKStatusPresentationRules(string url, bool dokDeliveryPresentationRulesStatusAutoUpdate, string dokDeliveryPresentationRulesStatusId, string metadataUuid);

        string GetDeliveryDownloadStatus(string uuid, bool autoUpdate, string currentStatus, string wfsStatus, string atomStatus);
        string GetSosiRequirements(string uuid, string url, bool autoUpdate, string currentStatus);
        string GetGmlRequirements(string uuid, bool dokDeliveryGmlRequirementsStatusAutoUpdate, string dokDeliveryGmlRequirementsStatusId);
        RegisterGrouped GetRegistersGrouped();
        ICollection<Models.Register> OrderBy(ICollection<Models.Register> registers, string sorting);
        Guid GetInspireStatusRegisterId();
        Models.Register GetInspireStatusRegister();
        Guid GetGeodatalovStatusRegisterId();
        Guid GetMareanoStatusRegisterId();
        List<Models.Register> GetCodelistRegisters();
        void DeleteRegister(Models.Register register);
        bool RegisterNameIsValid(Object register);
        Models.Register CreateNewRegister(Models.Register register);
        void UpdateDateModified(Models.Register register);
        Models.Register GetDokStatusRegister();
        void UpdateRegisterItemV2Translations();
        Models.Register GetGeodatalovDatasetRegister();
    }
}
