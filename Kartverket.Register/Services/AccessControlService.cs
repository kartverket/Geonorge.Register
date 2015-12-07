
using System;
using System.Security.Claims;
using Kartverket.Register.Models;
using Kartverket.Register.Services.RegisterItem;
using System.Collections.Generic;

namespace Kartverket.Register.Services
{
    public class AccessControlService : IAccessControlService
    {
        private ClaimsPrincipal _claimsPrincipal;
        private RegisterDbContext db = new RegisterDbContext();
        private IRegisterItemService _registerItemService;

        public AccessControlService(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
        }

        public AccessControlService()
        {
            _registerItemService = new RegisterItemService(db);
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
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            RegisterDbContext db = new RegisterDbContext();
            Models.RegisterItem registerItem = (Models.RegisterItem)model;

            if (accessRegister(registerItem.register))
            {
                if (IsDocument(registerItem))
                {
                    Document document = (Document)registerItem;
                    return IsOwner(document.documentowner.name, user);
                }
                else if (IsDataset(registerItem))
                {
                    Dataset dataset = (Dataset)registerItem;
                    return IsOwnerOrMunicipal(dataset.datasetowner.name, user);
                }
                else
                {
                    IsOwner(registerItem.submitter.name, user);
                }
            }
            return false;
        }

        private bool IsAdmin()
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
                //Innlogget bruker må være en kommune
                return MunicipalUser();
            }
            return false;
        }

        private bool IsDataset(Models.RegisterItem item)
        {
            return item.register.containedItemClass == "Dataset";
        }

        private bool IsDocument(Models.RegisterItem item)
        {
            return item.register.containedItemClass == "Document";
        }

        private bool IsOwner(string owner, string user)
        {
            return owner.ToLower() == user.ToLower();
        }

        private bool IsOwnerOrMunicipal(string owner, string user)
        {
            if (MunicipalUser())
            {
                return true;
            }
            return owner.ToLower() == user.ToLower();
        }


        private bool MunicipalUser()
        {
            string user = GetSecurityClaim("organization");
            List<CodelistValue> municipalities = _registerItemService.GetMunicipalityList();
            foreach (CodelistValue item in municipalities)
            {
                if (user == item.name)
                {
                    return true;
                }
            }
            return false;
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
    }
}