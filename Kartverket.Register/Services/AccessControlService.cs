
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
        private IMunicipalityService _municipalityService;

        public AccessControlService(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
        }

        public AccessControlService()
        {
            _registerItemService = new RegisterItemService(db);
            _registerService = new RegisterService(db);
            _municipalityService = new MunicipalityService();
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
                    if (dataset.IsNationalDataset())
                    {
                        return IsOwnerOrMunicipal(userOrganization.name, dataset);
                    }
                    else {
                        return IsOwner(dataset.datasetowner.name, userOrganization.name);
                    }
                }
                else {
                    return IsOwner(registerItem.submitter.name, userOrganization.name);
                }
            }
            return false;
        }

        public bool IsAdmin()
        {
            string role = GetSecurityClaim("role");
            return role == "nd.metadata_admin";
        }

        private bool accessRegister(object model)
        {
            string role = GetSecurityClaim("role");
            Models.Register register = (Models.Register)model;
            if (register.accessId == 2)
            {
                return role == "nd.metadata" || role == "nd.metadata_editor";
            }
            else if (register.accessId == 4)
            {
                return IsMunicipalUser();
            }
            else if (register.name == "Det offentlige kartgrunnlaget")
            {
                return IsMunicipalUser();
            }
            return false;
        }

        private bool IsOwner(string owner, string user)
        {
            return owner.ToLower() == user.ToLower();
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
            string username = GetUserName();
            if (!string.IsNullOrWhiteSpace(username))
            {
                List<CodelistValue> municipalities = _registerItemService.GetMunicipalityList();

                foreach (CodelistValue item in municipalities)
                {
                    if ((username.Contains(item.value)))
                    {

                        return true;
                    }
                }
            }
            return false;
        }

        public Organization MunicipalUserOrganization()
        {
            return _registerService.GetOrganizationByUserName();
        }

        public CodelistValue Municipality()
        {
            string username = GetUserName();
            if (username != null)
            {
                List<CodelistValue> municipalities = _registerItemService.GetMunicipalityList();
                foreach (CodelistValue item in municipalities)
                {
                    if (username.Contains(item.value))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public string GetSecurityClaim(string type)
        {
            string result = null;
            foreach (var claim in System.Security.Claims.ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result = claim.Value;
                    break;
                }
            }

            // bad hack, must fix BAAT
            if (!string.IsNullOrWhiteSpace(result) && type.Equals("organization") && result.Equals("Statens kartverk"))
            {
                result = "Kartverket";
            }

            return result;
        }

        public string GetUserName()
        {
            foreach (var claim in ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == "urn:oid:0.9.2342.19200300.100.1.1" || claim.Type == "username")
                {
                    return claim.Value;
                }
            }
            return null;
        }

        public bool EditDOK(Dataset dataset)
        {
            if (dataset.DatasetType == "Nasjonalt")
            {
                if (IsAdmin())
                {
                    return true;
                }
                else
                {
                    return IsMunicipalUser();
                }
            }
            else
            {
                return Access(dataset);
            }
        }

        public bool AccessEditDOKMunicipalBySelectedMunicipality(string municipalityCode)
        {
            return IsAdmin() || UserIsSelectedMunicipality(municipalityCode);
        }

        private bool UserIsSelectedMunicipality(string municipalityCode)
        {
            Organization user = MunicipalUserOrganization();
            string organizationNrOfSelectedMunicipality = _municipalityService.LookupOrganizationNumberFromMunicipalityCode(municipalityCode);

            if (user != null)
            {
                return user.number == organizationNrOfSelectedMunicipality;
            }
            return false;
        }
    }
}