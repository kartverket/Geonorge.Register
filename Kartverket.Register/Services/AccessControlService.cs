
using System;
using System.Security.Claims;
using Kartverket.Register.Models;
using Kartverket.Register.Services.RegisterItem;
using System.Collections.Generic;
using Kartverket.Register.Services.Register;

namespace Kartverket.Register.Services
{
    public class AccessControlService : IAccessControlService
    {
        private ClaimsPrincipal _claimsPrincipal;
        private RegisterDbContext db = new RegisterDbContext();
        private IRegisterItemService _registerItemService;
        private IRegisterService _registerService;
        private IOrganizationService _organizationService;

        public AccessControlService(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
        }

        public AccessControlService()
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
            if (model is Models.Register)
            {
                return accessRegister(model);
            }
            else if (model is Models.RegisterItem)
            {
                return accessRegisterItem(model);
            }
            return false;
        }

        private bool accessRegisterItem(object model)
        {
            Organization userOrganization = _registerService.GetOrganizationByUserName();
            RegisterDbContext db = new RegisterDbContext();
            Models.RegisterItem registerItem = (Models.RegisterItem)model;

            if (accessRegister(registerItem.register))
            {
                if (registerItem is Document)
                {
                    Document document = (Document)registerItem;
                    return IsOwner(document.documentowner.name, userOrganization.name);
                }
                else if (registerItem is Dataset)
                {
                    Dataset dataset = (Dataset)registerItem;
                    if (dataset.IsMunicipalDataset())
                    {
                        return IsOwner(dataset.datasetowner.name, userOrganization.name) || IsDokAdmin();
                    }
                }
                else {
                    return IsOwner(registerItem.submitter.name, userOrganization.name);
                }
            }
            return false;
        }

        private bool IsDokEditor()
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

        private bool accessRegister(object model)
        {
            Models.Register register = (Models.Register)model;
            if (register.accessId == 2)
            {
                if (IsEditor())
                {
                    return true;
                }
            }
            else if (register.accessId == 4)
            {
                return IsMunicipalUser() || IsDokEditor() || IsDokAdmin();
            }
            return false;
        }

        private bool IsEditor()
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

        private bool IsDokAdmin()
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

        public bool IsOwner(string owner, string user)
        {
            return (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(user)) && (owner.ToLower() == user.ToLower());
        }

        private bool IsOwnerOrMunicipal(string user, Dataset dataset)
        {
            if (IsMunicipalUser())
            {
                return true;
            }
            else if (IsAdmin())
            {
                return true;
            }
            return false;
        }

        public bool IsMunicipalUser()
        {
            return GetMunicipalityCode() != null;
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
            string organizationNumber = GetOrganizationNumber();
            if (organizationNumber == null)
            {
                return null;
            }

            var org = _organizationService.GetOrganizationByNumber(organizationNumber);
            return org != null ? org.MunicipalityCode : null;
        }

        public List<string> GetSecurityClaim(string type)
        {
            List<string> result = new List<string>();
            foreach (var claim in ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result.Add(claim.Value);
                }
            }

            // bad hack, must fix BAAT
            if (result.Count == 0 && type.Equals("organization") && result.Equals("Statens kartverk"))
            {
                result.Add("Kartverket");
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

        private bool UserIsSelectedMunicipality(string municipalityCode)
        {
            string currentUserMunicipalityCode = GetMunicipalityCode();
            if (municipalityCode == null || currentUserMunicipalityCode == null)
            {
                return false;
            }

            return currentUserMunicipalityCode == municipalityCode;
        }

        public bool AccessCreateNewMunicipalDataset(string municipalityCode)
        {
            return IsAdmin() || UserIsSelectedMunicipality(municipalityCode) || IsDokAdmin();
        }
    }
}