
using System.Security.Claims;
using Kartverket.Register.Models;
using Kartverket.Register.Services.RegisterItem;
using System.Collections.Generic;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services.Register;
using Resources;

namespace Kartverket.Register.Services
{
    public class AccessControlService : IAccessControlService
    {
        private readonly IRegisterItemService _registerItemService;
        private readonly IRegisterService _registerService;
        private readonly IOrganizationService _organizationService;

        public AccessControlService(RegisterDbContext db)
        {
            _registerItemService = new RegisterItemService(db);
            _registerService = new RegisterService(db);
            _organizationService = new OrganizationsService(db);
        }

        public bool Access(object model)
        {
            if (IsAdmin())
            {
                return true;
            }
            if (model is Models.Register register)
            {
                return AccessRegister(register);
            }
            if (model is RegisterV2ViewModel registerViewModel)
            {
                return AccessRegister(registerViewModel);
            }
            if (model is Models.RegisterItem registerItem)
            {
                return AccessRegisterItem(registerItem);
            }
            if (model is RegisterItemV2ViewModel registerItemViewModel)
            {
                return AccessRegisterItem(registerItemViewModel);
            }
            return false;
        }

        public AccessViewModel AccessViewModel(RegisterV2ViewModel registerViewModel)
        {
            var accessViewModel = new AccessViewModel();
            accessViewModel.Edit = EditRegister(registerViewModel);
            accessViewModel.Add = AddToRegister(registerViewModel);
            accessViewModel.EditListOfRegisterItems = EditRegisterItemsList(registerViewModel);
            accessViewModel.Delete = DeleteRegister(registerViewModel);
            return accessViewModel;
        }

        private bool DeleteRegister(RegisterV2ViewModel registerViewModel)
        {
            if (IsAdmin())
            {
                if (registerViewModel.ParentRegister == null)
                {
                    return registerViewModel.RegisterItems == null && registerViewModel.RegisterItemsV2 == null;
                }
                return true;
            }
            return false;
        }

        public bool AddToRegister(RegisterV2ViewModel registerViewModel)
        {
            if (registerViewModel.IsDokMunicipal())
            {
                return registerViewModel.Municipality != null && AccessRegister(registerViewModel);
            }
            return AccessRegister(registerViewModel);
        }

        public bool EditRegisterItemsList(RegisterV2ViewModel registerViewModel)
        {
            return registerViewModel.Municipality != null && AccessRegister(registerViewModel);
        }

        public bool EditRegister(RegisterV2ViewModel registerViewModel)
        {
            return (IsAdmin() || IsRegisterOwner(registerViewModel.Owner.name, UserName())) && !registerViewModel.IsServiceAlertRegister();
        }

        public bool AccessRegister(Models.Register register)
        {
            if (IsAdmin()) return true;
            if (register.RegisterAccessAdminAndEditor())
            {
                if (IsEditor())
                {
                    return !register.ContainedItemClassIsCodelistValue() ||
                           IsRegisterOwner(register.owner.name, UserName());
                }
            }
            else if (register.RegisterAccessAdminMunicipalUserDokEditorAndDocAdmin())
            {
                return IsMunicipalUser() || IsDokEditor() || IsDokAdmin();
            }
            return false;
        }

        private bool AccessRegister(RegisterV2ViewModel registerViewModel)
        {
            if (IsAdmin()) return true;
            if (registerViewModel.AccessId == 2)
            {
                if (IsEditor())
                {
                    if (registerViewModel.ContainedItemClassIsCodelistValue())
                    {
                        return IsRegisterOwner(registerViewModel.Owner.name, UserName());
                    }
                    return true;
                }
            }
            else if (registerViewModel.AccessId == 4)
            {
                return UserIsSelectedMunicipality(registerViewModel.MunicipalityCode) || IsDokEditor() || IsDokAdmin();
            }
            return false;
        }

        public bool AccessRegisterItem(Models.RegisterItem registerItem)
        {
            if (IsAdmin()) return true;
            if (AccessRegister(registerItem.register))
            {
                if (registerItem is Document document)
                {
                    return IsItemOwner(document.documentowner.name, UserName()) && VersionIsEditable(registerItem.statusId);
                }
                if (registerItem is Dataset dataset)
                {
                    if (dataset.IsMunicipalDataset())
                    {
                        return IsItemOwner(dataset.datasetowner.name, UserName()) || IsDokAdmin();
                    }
                }
                else {
                    return IsItemOwner(registerItem.submitter.name, UserName()) || IsRegisterOwner(registerItem.register.owner.name, UserName()) ;
                }
            }
            return false;
        }

        private bool VersionIsEditable(string statusId)
        {
            return statusId == "Submitted" || statusId == "Draft" || IsAdmin();
        }

        private bool AccessRegisterItem(RegisterItemV2ViewModel registerItemViewModel)
        {
            if (!registerItemViewModel.Register.IsServiceAlertRegister())
            {
                if (AccessRegister(registerItemViewModel.Register))
                {
                    if (registerItemViewModel is DocumentViewModel docuementViewModel)
                    {
                        return IsItemOwner(registerItemViewModel.Owner.name, UserName()) && VersionIsEditable(docuementViewModel.StatusId);
                    }
                }

            }
            return false;
        }

        public bool AccessCreateNewVersion(RegisterItemV2ViewModel registerItemViewModel)
        {
            return AccessRegister(registerItemViewModel.Register) && IsItemOwner(registerItemViewModel.Owner.name, UserName()) || IsAdmin();
        }


        public bool IsAdmin()
        {
            List<string> roles = GetSecurityClaim("role");
            foreach (string role in roles)
            {
                if (role == "nd.metadata_admin")
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsEditor()
        {
            List<string> roles = GetSecurityClaim("role");
            foreach (string role in roles)
            {
                if (role == "nd.metadata" || role == "nd.metadata_editor")
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDokAdmin()
        {
            List<string> roles = GetSecurityClaim("role");
            foreach (string role in roles)
            {
                if (role == "nd.dok_admin")
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDokEditor()
        {
            List<string> roles = GetSecurityClaim("role");
            foreach (string role in roles)
            {
                if (role == "nd.dok_editor")
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsMunicipalUser()
        {
            return GetMunicipalityCode() != null;
        }


        public bool IsItemOwner(string owner, string user)
        {
            return owner.ToLower() == user.ToLower();
        }

        public bool IsRegisterOwner(string registerOwner, string userName)
        {
            return registerOwner == userName;
        }


        public Organization MunicipalUserOrganization()
        {
            return _registerService.GetOrganizationByUserName();
        }

        public string GetOrganizationNumber()
        {
            Claim orgnrClaim = ClaimsPrincipal.Current.FindFirst("orgnr");
            return orgnrClaim?.Value;
        }

        public bool AccessEdit(object model)
        {
            throw new System.NotImplementedException();
        }

        public CodelistValue GetMunicipality()
        {
            string municipalityCode = GetMunicipalityCode();
            if (municipalityCode == null)
            {
                return null;
            }

            List<CodelistValue> municipalities = _registerItemService.GetMunicipalityList();
            foreach (CodelistValue item in municipalities)
            {
                if (municipalityCode.Equals(item.value))
                {
                    return item;
                }
            }
            return null;
        }

        private string GetMunicipalityCode()
        {
            var organizationNumber = GetOrganizationNumber();
            if (organizationNumber == null)
            {
                return null;
            }

            var org = _organizationService.GetOrganizationByNumber(organizationNumber);
            return org?.MunicipalityCode;
        }

        public List<string> GetSecurityClaim(string type)
        {
            var result = new List<string>();
            foreach (var claim in ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result.Add(claim.Value);
                }
            }
            return result;
        }

        public bool EditDOK(Dataset dataset)
        {
            if (dataset.IsNationalDataset())
            {
                if (IsAdmin())
                {
                    return true;
                }
                return false;
            }
            else
            {
                return Access(dataset);
            }
        }

        public bool AccessEditOrCreateDOKMunicipalBySelectedMunicipality(string municipalityCode)
        {
            return IsAdmin() || UserIsSelectedMunicipality(municipalityCode) || IsDokAdmin();
        }

        public bool UserIsSelectedMunicipality(string municipalityCode)
        {
            var currentUserMunicipalityCode = GetMunicipalityCode();
            if (municipalityCode == null || currentUserMunicipalityCode == null)
            {
                return false;
            }
            return currentUserMunicipalityCode == municipalityCode;
        }

        public string UserName()
        {
            var user = _registerService.GetOrganizationByUserName();
            return user?.name;
        }
    }
}