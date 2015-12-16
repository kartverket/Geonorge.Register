using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Kartverket.Register.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static readonly RegisterDbContext db = new RegisterDbContext();
        private static IRegisterItemService _registeritemService = new RegisterItemService(db);
        private static IAccessControlService _accessControl = new AccessControlService();
        private static IMunicipalityService _municipalityService = new MunicipalityService();
        private static IRegisterService _registerService = new RegisterService(db);

        public static string ApplicationVersionNumber(this HtmlHelper helper)
        {
            string versionNumber = WebConfigurationManager.AppSettings["BuildVersionNumber"];
            return versionNumber;
        }

        public static bool Access(object model)
        {
            return _accessControl.Access(model);
        }

        public static bool IsAdmin()
        {
            return _accessControl.IsAdmin();
        }

        public static bool AccessEdit(RegisterItem item)
        {
            if (item.register.containedItemClass != "Document")
            {
                if (item is Dataset)
                {
                    return _accessControl.EditDOK((Dataset)item);
                }
                else
                {
                    return Access(item);
                }
            }
            return false;
        }
        public static bool IsMunicipalUser()
        {
            return _accessControl.IsMunicipalUser();
        }

        public static CoverageDataset GetMunicipalCoverage(Dataset model)
        {
            return _registeritemService.GetMunicipalityCoverage(model);
        }

        public static string GetSecurityClaim(this HtmlHelper helper, IEnumerable<System.Security.Claims.Claim> claims, string type)
        {
            string result = null;
            foreach (var claim in claims)
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


        public static bool accessRegister(Models.Register register)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin")
            {
                return true;
            }
            if (register.accessId == 2 && (role == "nd.metadata" || role == "nd.metadata_editor"))
            {
                return true;
            }
            return false;
        }

        public static bool accessRegisterOwner(Models.Register register)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin")
            {
                return true;
            }
            if (register.accessId == 2 && (role == "nd.metadata" || role == "nd.metadata_editor") && register.owner.name.ToLower() == user.ToLower())
            {
                return true;
            }
            return false;
        }

        public static bool accessRegisterItem(RegisterItem item)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin")
            {
                return true;
            }

            if (item.register.accessId == 2 && (role == "nd.metadata" || role == "nd.metadata_editor"))
            {
                if (item.register.containedItemClass == "Document")
                {
                    Kartverket.Register.Models.Document document = db.Documents.Find(item.systemId);
                    if (document.documentowner.name.ToLower() == user.ToLower())
                    {
                        return true;
                    }
                    return false;
                }

                if (item.register.containedItemClass == "Dataset")
                {
                    Kartverket.Register.Models.Dataset dataset = db.Datasets.Find(item.systemId);
                    if (dataset.datasetowner.name.ToLower() == user.ToLower())
                    {
                        return true;
                    }
                    return false;
                }

                if (item.submitter.name.ToLower() == user.ToLower())
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool IsGeonorgeAdmin(this HtmlHelper helper, IEnumerable<System.Security.Claims.Claim> claims)
        {
            bool isInRole = false;
            foreach (var c in claims)
            {
                if (c.Type == "role")
                {
                    if (c.Value == "nd.metadata_admin")
                    {
                        isInRole = true;
                        break;
                    }
                }
            }

            return isInRole;
        }

        public static string lovligInnhold(string containedItemClass)
        {
            if (containedItemClass == "Document")
            {
                return "Dokumenter";
            }
            else if (containedItemClass == "Dataset")
            {
                return "Datasett";
            }
            else if (containedItemClass == "EPSG")
            {
                return "EPSG-koder";
            }
            else if (containedItemClass == "Organization")
            {
                return "Organisasjoner";
            }
            else if (containedItemClass == "CodelistValue")
            {
                return "Kodeverdier";
            }
            else if (containedItemClass == "Register")
            {
                return "Registre";
            }
            else if (containedItemClass == "NameSpace")
            {
                return "Navnerom";
            }
            return "";
        }

        public static string Type(string containedItemClass)
        {
            if (containedItemClass == "Document")
            {
                return "Dokument";
            }
            else if (containedItemClass == "Dataset")
            {
                return "Datasett";
            }
            else if (containedItemClass == "EPSG")
            {
                return "EPSG-kode";
            }
            else if (containedItemClass == "Organization")
            {
                return "Organisasjon";
            }
            else if (containedItemClass == "CodelistValue")
            {
                return "Kodeverdi";
            }
            else if (containedItemClass == "Register")
            {
                return "Register";
            }
            else if (containedItemClass == "NameSpace")
            {
                return "Navnerom";
            }
            return "";
        }

        public static List<Models.Register> Registers()
        {
            var queryResults = from o in db.Registers
                               where o.parentRegisterId == null
                               select o;

            List<Models.Register> RegistersList = new List<Models.Register>();
            foreach (var item in queryResults)
            {
                RegistersList.Add(item);
            }
            RegistersList.OrderBy(r => r.name);

            return RegistersList;
        }

        public static List<Models.Register> CodelistRegister()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResults = from o in db.Registers
                               where o.containedItemClass == "CodelistValue"
                               select o;

            List<Models.Register> RegistersList = new List<Models.Register>();
            foreach (var item in queryResults)
            {
                RegistersList.Add(item);
            }
            RegistersList.OrderBy(r => r.name);

            return RegistersList;
        }

        public static bool IsGeonorgeEditor(this HtmlHelper helper, IEnumerable<System.Security.Claims.Claim> claims)
        {
            bool isInRole = false;
            foreach (var c in claims)
            {
                if (c.Type == "role")
                {
                    if (c.Value == "nd.metadata_editor" || c.Value == "nd.metadata")
                    {
                        isInRole = true;
                        break;
                    }
                }
            }

            return isInRole;
        }

        // SORTERING av registeritems
        public static List<RegisterItem> SortingRegisterItems(Models.Register register, string sortingType)
        {
            string text = HttpContext.Current.Request.QueryString["text"] != null ? HttpContext.Current.Request.QueryString["text"].ToString() : "";
            string filterVertikalt = HttpContext.Current.Request.QueryString["filterVertikalt"] != null ? HttpContext.Current.Request.QueryString["filterVertikalt"].ToString() : "";
            string municipality = HttpContext.Current.Request.QueryString["municipality"] != null ? HttpContext.Current.Request.QueryString["municipality"].ToString() : "";
            string filterHorisontalt = HttpContext.Current.Request.QueryString["filterHorisontalt"] != null ? HttpContext.Current.Request.QueryString["filterHorisontalt"].ToString() : "";
            string InspireRequirementParam = HttpContext.Current.Request.QueryString["InspireRequirement"] != null ? HttpContext.Current.Request.QueryString["InspireRequirement"].ToString() : "";
            string nationalRequirementParam = HttpContext.Current.Request.QueryString["nationalRequirement"] != null ? HttpContext.Current.Request.QueryString["nationalRequirement"].ToString() : "";
            string nationalSeaRequirementParam = HttpContext.Current.Request.QueryString["nationalSeaRequirement"] != null ? HttpContext.Current.Request.QueryString["nationalSeaRequirement"].ToString() : "";

            if (HttpContext.Current.Request.QueryString.Count < 1)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["sortingType"] != null && string.IsNullOrEmpty(sortingType))
                        sortingType = HttpContext.Current.Session["sortingType"].ToString();

                    if (HttpContext.Current.Session["text"] != null && string.IsNullOrEmpty(text))
                        text = HttpContext.Current.Session["text"].ToString();

                    if (HttpContext.Current.Session["filterVertikalt"] != null && string.IsNullOrEmpty(filterVertikalt))
                        filterVertikalt = HttpContext.Current.Session["filterVertikalt"].ToString();

                    if (HttpContext.Current.Session["filterHorisontalt"] != null && string.IsNullOrEmpty(filterHorisontalt))
                        filterHorisontalt = HttpContext.Current.Session["filterHorisontalt"].ToString();

                    if (HttpContext.Current.Session["InspireRequirement"] != null && string.IsNullOrEmpty(InspireRequirementParam))
                        InspireRequirementParam = HttpContext.Current.Session["InspireRequirement"].ToString();

                    if (HttpContext.Current.Session["nationalRequirement"] != null && string.IsNullOrEmpty(nationalRequirementParam))
                        nationalRequirementParam = HttpContext.Current.Session["nationalRequirement"].ToString();

                    if (HttpContext.Current.Session["nationalSeaRequirement"] != null && string.IsNullOrEmpty(nationalSeaRequirementParam))
                        nationalSeaRequirementParam = HttpContext.Current.Session["nationalSeaRequirement"].ToString();

                    if (HttpContext.Current.Session["municipality"] != null && string.IsNullOrEmpty(municipality))
                        municipality = HttpContext.Current.Session["municipality"].ToString();

                    string redirect = HttpContext.Current.Request.Path + "?sorting=" + sortingType;
                    bool shallRedirect = false;

                    if (text != "")
                    {
                        redirect = redirect + "&text=" + text;
                        shallRedirect = true;
                    }

                    if (filterVertikalt != "")
                    {
                        if (filterVertikalt.Contains(","))
                            filterVertikalt = filterVertikalt.Replace(",false", "");
                        redirect = redirect + "&filterVertikalt=" + filterVertikalt;
                        shallRedirect = true;
                    }

                    if (filterHorisontalt != "")
                    {
                        if (filterHorisontalt.Contains(","))
                            filterHorisontalt = filterHorisontalt.Replace(",false", "");
                        redirect = redirect + "&filterHorisontalt=" + filterHorisontalt;
                        shallRedirect = true;
                    }

                    if (InspireRequirementParam != "")
                    {
                        redirect = redirect + "&inspireRequirement=" + InspireRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalRequirementParam != "")
                    {
                        redirect = redirect + "&nationalRequirement=" + nationalRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalSeaRequirementParam != "")
                    {
                        redirect = redirect + "&nationalSeaRequirement=" + nationalSeaRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalSeaRequirementParam != "")
                    {
                        redirect = redirect + "&municipality=" + municipality;
                        shallRedirect = true;
                    }

                    if (shallRedirect)
                    {
                        HttpContext.Current.Response.Redirect(redirect);
                    }

                }
            }
            HttpContext.Current.Session["sortingType"] = sortingType;
            HttpContext.Current.Session["municipality"] = municipality;
            HttpContext.Current.Session["text"] = text;
            HttpContext.Current.Session["filterVertikalt"] = filterVertikalt;
            HttpContext.Current.Session["filterHorisontalt"] = filterHorisontalt;
            HttpContext.Current.Session["InspireRequirement"] = InspireRequirementParam;
            HttpContext.Current.Session["nationalRequirement"] = nationalRequirementParam;
            HttpContext.Current.Session["nationalSeaRequirement"] = nationalSeaRequirementParam;


            var sortedList = register.items.OrderBy(o => o.name).ToList();
            if (sortingType == "name_desc")
            {
                sortedList = register.items.OrderByDescending(o => o.name).ToList();
            }
            else if (sortingType == "submitter")
            {
                sortedList = register.items.OrderBy(o => o.submitter.name).ToList();
            }
            else if (sortingType == "submitter_desc")
            {
                sortedList = register.items.OrderByDescending(o => o.submitter.name).ToList();
            }
            else if (sortingType == "status")
            {
                sortedList = register.items.OrderBy(o => o.status.description).ToList();
            }
            else if (sortingType == "status_desc")
            {
                sortedList = register.items.OrderByDescending(o => o.status.description).ToList();
            }
            else if (sortingType == "dateSubmitted")
            {
                sortedList = register.items.OrderBy(o => o.dateSubmitted).ToList();
            }
            else if (sortingType == "dateSubmitted_desc")
            {
                sortedList = register.items.OrderByDescending(o => o.dateSubmitted).ToList();
            }
            else if (sortingType == "modified")
            {
                sortedList = register.items.OrderBy(o => o.modified).ToList();
            }
            else if (sortingType == "modified_desc")
            {
                sortedList = register.items.OrderByDescending(o => o.modified).ToList();
            }
            else if (sortingType == "dateAccepted")
            {
                sortedList = register.items.OrderBy(o => o.dateAccepted).ToList();
            }
            else if (sortingType == "dateAccepted_desc")
            {
                sortedList = register.items.OrderByDescending(o => o.dateAccepted).ToList();
            }
            else if (sortingType == "documentOwner")
            {
                var documentOwner = register.items.OfType<Document>().OrderBy(o => o.documentowner.name);
                sortedList = documentOwner.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "documentOwner_desc")
            {
                var documentOwner = register.items.OfType<Document>().OrderByDescending(o => o.documentowner.name);
                sortedList = documentOwner.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "datasetOwner")
            {
                var datasetOwner = register.items.OfType<Dataset>().OrderBy(o => o.datasetowner.name);
                sortedList = datasetOwner.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "datasetOwner_desc")
            {
                var datasetOwner = register.items.OfType<Dataset>().OrderByDescending(o => o.datasetowner.name);
                sortedList = datasetOwner.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokStatus")
            {
                var dokStatus = register.items.OfType<Dataset>().OrderBy(o => o.dokStatus.description);
                sortedList = dokStatus.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokStatus_desc")
            {
                var dokStatus = register.items.OfType<Dataset>().OrderByDescending(o => o.dokStatus.description);
                sortedList = dokStatus.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "distributionFormat")
            {
                var distributionFormat = register.items.OfType<Dataset>().OrderBy(o => o.DistributionFormat);
                sortedList = distributionFormat.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "distributionFormat_desc")
            {
                var distributionFormat = register.items.OfType<Dataset>().OrderByDescending(o => o.DistributionFormat);
                sortedList = distributionFormat.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "wmsUrl")
            {
                var wmsUrl = register.items.OfType<Dataset>().OrderBy(o => o.WmsUrl);
                sortedList = wmsUrl.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "wmsUrl_desc")
            {
                var wmsUrl = register.items.OfType<Dataset>().OrderByDescending(o => o.WmsUrl);
                sortedList = wmsUrl.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "theme")
            {
                var theme = register.items.OfType<Dataset>().OrderBy(o => o.theme.value);
                sortedList = theme.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "theme_desc")
            {
                var theme = register.items.OfType<Dataset>().OrderByDescending(o => o.theme.value);
                sortedList = theme.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "epsg")
            {
                var epsg = register.items.OfType<EPSG>().OrderBy(o => o.epsgcode);
                sortedList = epsg.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "epsg_desc")
            {
                var epsg = register.items.OfType<EPSG>().OrderByDescending(o => o.epsgcode);
                sortedList = epsg.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "sosiReferencesystem")
            {
                var sosiReferencesystem = register.items.OfType<EPSG>().OrderBy(o => o.sosiReferencesystem);
                sortedList = sosiReferencesystem.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "sosiReferencesystem_desc")
            {
                var sosiReferencesystem = register.items.OfType<EPSG>().OrderByDescending(o => o.sosiReferencesystem);
                sortedList = sosiReferencesystem.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "inspireRequirement")
            {
                var inspireRequirement = register.items.OfType<EPSG>().OrderBy(o => o.inspireRequirement.sortOrder);
                sortedList = inspireRequirement.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "inspireRequirement_desc")
            {
                var inspireRequirement = register.items.OfType<EPSG>().OrderByDescending(o => o.inspireRequirement.sortOrder);
                sortedList = inspireRequirement.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "nationalRequirement")
            {
                var nationalRequirement = register.items.OfType<EPSG>().OrderBy(o => o.nationalRequirement.sortOrder);
                sortedList = nationalRequirement.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "nationalRequirement_desc")
            {
                var nationalRequirement = register.items.OfType<EPSG>().OrderByDescending(o => o.nationalRequirement.sortOrder);
                sortedList = nationalRequirement.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "nationalSeasRequirement")
            {
                var nationalSeasRequirement = register.items.OfType<EPSG>().OrderBy(o => o.nationalSeasRequirement.sortOrder);
                sortedList = nationalSeasRequirement.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "nationalSeasRequirement_desc")
            {
                var nationalSeasRequirement = register.items.OfType<EPSG>().OrderByDescending(o => o.nationalSeasRequirement.sortOrder);
                sortedList = nationalSeasRequirement.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "description")
            {
                sortedList = register.items.OrderBy(o => o.description).ToList();
            }
            else if (sortingType == "description_desc")
            {
                sortedList = register.items.OrderByDescending(o => o.description).ToList();
            }
            else if (sortingType == "codevalue")
            {
                var codevalue = register.items.OfType<CodelistValue>().OrderBy(o => o.value);
                sortedList = codevalue.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "codevalue_desc")
            {
                var codevalue = register.items.OfType<CodelistValue>().OrderByDescending(o => o.value);
                sortedList = codevalue.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "number")
            {
                var number = register.items.OfType<Organization>().OrderBy(o => o.number);
                sortedList = number.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "number_desc")
            {
                var number = register.items.OfType<Organization>().OrderByDescending(o => o.number);
                sortedList = number.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "verticalReferenceSystem")
            {
                var verticalReferenceSystem = register.items.OfType<EPSG>().OrderBy(o => o.verticalReferenceSystem);
                sortedList = verticalReferenceSystem.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "verticalReferenceSystem_desc")
            {
                var verticalReferenceSystem = register.items.OfType<EPSG>().OrderByDescending(o => o.verticalReferenceSystem);
                sortedList = verticalReferenceSystem.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "horizontalReferenceSystem")
            {
                var horizontalReferenceSystem = register.items.OfType<EPSG>().OrderBy(o => o.horizontalReferenceSystem);
                sortedList = horizontalReferenceSystem.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "horizontalReferenceSystem_desc")
            {
                var horizontalReferenceSystem = register.items.OfType<EPSG>().OrderByDescending(o => o.horizontalReferenceSystem);
                sortedList = horizontalReferenceSystem.Cast<RegisterItem>().ToList();
            }

            else if (sortingType == "dimension")
            {
                var dimension = register.items.OfType<EPSG>().OrderBy(o => o.dimension == null ? "" : o.dimension.description);
                sortedList = dimension.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dimension_desc")
            {
                var dimension = register.items.OfType<EPSG>().OrderByDescending(o => o.dimension == null ? "" : o.dimension.description);
                sortedList = dimension.Cast<RegisterItem>().ToList();
            }

            return sortedList;
        }

        // SORTERING av Register
        public static List<Models.Register> SortingRegisters(Models.Register Model, String sortingType)
        {
            string text = HttpContext.Current.Request.QueryString["text"] != null ? HttpContext.Current.Request.QueryString["text"].ToString() : "";
            string filterVertikalt = HttpContext.Current.Request.QueryString["filterVertikalt"] != null ? HttpContext.Current.Request.QueryString["filterVertikalt"].ToString() : "";
            string municipality = HttpContext.Current.Request.QueryString["municipality"] != null ? HttpContext.Current.Request.QueryString["municipality"].ToString() : "";
            string filterHorisontalt = HttpContext.Current.Request.QueryString["filterHorisontalt"] != null ? HttpContext.Current.Request.QueryString["filterHorisontalt"].ToString() : "";
            string InspireRequirementParam = HttpContext.Current.Request.QueryString["InspireRequirement"] != null ? HttpContext.Current.Request.QueryString["InspireRequirement"].ToString() : "";
            string nationalRequirementParam = HttpContext.Current.Request.QueryString["nationalRequirement"] != null ? HttpContext.Current.Request.QueryString["nationalRequirement"].ToString() : "";
            string nationalSeaRequirementParam = HttpContext.Current.Request.QueryString["nationalSeaRequirement"] != null ? HttpContext.Current.Request.QueryString["nationalSeaRequirement"].ToString() : "";

            if (HttpContext.Current.Request.QueryString.Count < 1)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["sortingType"] != null && string.IsNullOrEmpty(sortingType))
                        sortingType = HttpContext.Current.Session["sortingType"].ToString();

                    if (HttpContext.Current.Session["text"] != null && string.IsNullOrEmpty(text))
                        text = HttpContext.Current.Session["text"].ToString();

                    if (HttpContext.Current.Session["filterVertikalt"] != null && string.IsNullOrEmpty(filterVertikalt))
                        filterVertikalt = HttpContext.Current.Session["filterVertikalt"].ToString();

                    if (HttpContext.Current.Session["filterHorisontalt"] != null && string.IsNullOrEmpty(filterHorisontalt))
                        filterHorisontalt = HttpContext.Current.Session["filterHorisontalt"].ToString();

                    if (HttpContext.Current.Session["InspireRequirement"] != null && string.IsNullOrEmpty(InspireRequirementParam))
                        InspireRequirementParam = HttpContext.Current.Session["InspireRequirement"].ToString();

                    if (HttpContext.Current.Session["nationalRequirement"] != null && string.IsNullOrEmpty(nationalRequirementParam))
                        nationalRequirementParam = HttpContext.Current.Session["nationalRequirement"].ToString();

                    if (HttpContext.Current.Session["nationalSeaRequirement"] != null && string.IsNullOrEmpty(nationalSeaRequirementParam))
                        nationalSeaRequirementParam = HttpContext.Current.Session["nationalSeaRequirement"].ToString();

                    if (HttpContext.Current.Session["municipality"] != null && string.IsNullOrEmpty(municipality))
                        municipality = HttpContext.Current.Session["municipality"].ToString();

                    string redirect = HttpContext.Current.Request.Path + "?sorting=" + sortingType;
                    bool shallRedirect = false;

                    if (text != "")
                    {
                        redirect = redirect + "&text=" + text;
                        shallRedirect = true;
                    }

                    if (filterVertikalt != "")
                    {
                        if (filterVertikalt.Contains(","))
                            filterVertikalt = filterVertikalt.Replace(",false", "");
                        redirect = redirect + "&filterVertikalt=" + filterVertikalt;
                        shallRedirect = true;
                    }

                    if (filterHorisontalt != "")
                    {
                        if (filterHorisontalt.Contains(","))
                            filterHorisontalt = filterHorisontalt.Replace(",false", "");
                        redirect = redirect + "&filterHorisontalt=" + filterHorisontalt;
                        shallRedirect = true;
                    }

                    if (InspireRequirementParam != "")
                    {
                        redirect = redirect + "&inspireRequirement=" + InspireRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalRequirementParam != "")
                    {
                        redirect = redirect + "&nationalRequirement=" + nationalRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalSeaRequirementParam != "")
                    {
                        redirect = redirect + "&nationalSeaRequirement=" + nationalSeaRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalSeaRequirementParam != "")
                    {
                        redirect = redirect + "&municipality=" + municipality;
                        shallRedirect = true;
                    }

                    if (shallRedirect)
                    {
                        HttpContext.Current.Response.Redirect(redirect);
                    }

                }
            }
            HttpContext.Current.Session["sortingType"] = sortingType;
            HttpContext.Current.Session["municipality"] = municipality;
            HttpContext.Current.Session["text"] = text;
            HttpContext.Current.Session["filterVertikalt"] = filterVertikalt;
            HttpContext.Current.Session["filterHorisontalt"] = filterHorisontalt;
            HttpContext.Current.Session["InspireRequirement"] = InspireRequirementParam;
            HttpContext.Current.Session["nationalRequirement"] = nationalRequirementParam;
            HttpContext.Current.Session["nationalSeaRequirement"] = nationalSeaRequirementParam;


            var sortedList = Model.subregisters.OrderBy(o => o.name).ToList();
            if (sortingType == "name_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.name).ToList();
            }
            else if (sortingType == "submitter")
            {
                sortedList = Model.subregisters.OrderBy(o => o.owner.name).ToList();
            }
            else if (sortingType == "submitter_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.owner.name).ToList();
            }
            else if (sortingType == "status")
            {
                sortedList = Model.subregisters.OrderBy(o => o.status.description).ToList();
            }
            else if (sortingType == "status_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.status.description).ToList();
            }
            else if (sortingType == "dateSubmitted")
            {
                sortedList = Model.subregisters.OrderBy(o => o.dateSubmitted).ToList();
            }
            else if (sortingType == "dateSubmitted_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.dateSubmitted).ToList();
            }
            else if (sortingType == "modified")
            {
                sortedList = Model.subregisters.OrderBy(o => o.modified).ToList();
            }
            else if (sortingType == "modified_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.modified).ToList();
            }
            else if (sortingType == "dateAccepted")
            {
                sortedList = Model.subregisters.OrderBy(o => o.dateAccepted).ToList();
            }
            else if (sortingType == "dateAccepted_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.dateAccepted).ToList();
            }

            else if (sortingType == "description")
            {
                sortedList = Model.subregisters.OrderBy(o => o.description).ToList();
            }
            else if (sortingType == "description_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.description).ToList();
            }

            else if (sortingType == "owner")
            {
                sortedList = Model.subregisters.OrderBy(o => o.owner.name).ToList();
            }
            else if (sortingType == "owner_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.owner.name).ToList();
            }

            return sortedList;
        }

        public static Models.Register mainRegister(Models.Register register)
        {
            Models.Register parentRegister;
            if (register.parentRegister != null)
            {
                parentRegister = register.parentRegister;
                parentRegister = getParentRegister(parentRegister);

                return parentRegister;
            }
            else
            {
                return register;
            }

        }

        private static Models.Register getParentRegister(Models.Register register)
        {
            Models.Register parentRegister;
            if (register.parentRegisterId != null)
            {
                parentRegister = register.parentRegister;
                parentRegister = getParentRegister(parentRegister);
                return parentRegister;
            }
            else
            {
                return register;
            }

        }

        private static Models.Register hasParentRegister(Models.Register model, List<Models.Register> parentsList, Models.Register register)
        {
            if (register.parentRegister != null)
            {
                parentsList.Add(model.parentRegister);
                register = model.parentRegister;

                register = hasParentRegister(model, parentsList, register);
            }
            return register;
        }

        public static string GetSecurityClaim(string type)
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


        public static string StatusBeskrivelse()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResults = from s in db.Statuses
                               select s;

            string forslag = "Forslag:&#013";
            string gyldig = "Gyldig:&#013";
            string historiske = "Historiske:&#013";

            foreach (Status s in queryResults)
            {
                if (s.group == "suggested")
                {
                    forslag += "- " + s.description + "&#013";
                }
                if (s.group == "historical")
                {
                    historiske += "- " + s.description + "&#013";
                }
                if (s.group == "current")
                {
                    gyldig += "- " + s.description + "&#013";
                }
            }

            string beskrivelse = "Mulige statuser:&#013&#013" + forslag + "&#013" + gyldig + "&#013" + historiske;
            return beskrivelse;
        }

        public static object DokStatusBeskrivelse()
        {
            return "Forslag&#013Kandidat&#013I Prosess&#013Godkjent&#013";

        }

        public static string HasParent(Models.Register Model)
        {
            if (Model.parentRegister != null) return Model.parentRegister.seoname;
            else return null;
        }

        public static string ErrorMessageValidationName()
        {
            return "Navnet finnes fra før!";
        }


        public static CodelistValue GetSelectedMunicipality(string selectedMunicipality)
        {
            if (selectedMunicipality == null)
            {
                string username = _accessControl.GetUserName();
                return _accessControl.Municipality();
            }
            else
            {
                return _registeritemService.GetMunicipalByNr(selectedMunicipality.ToString());
            }
        }

        public static string selectedMunicipalName(CodelistValue selectedMunicipal)
        {
            if (selectedMunicipal != null)
            {
                return selectedMunicipal.name;
            }
            else
            {
                return "Velg kommune";
            }
        }

        public static string GetDokStatusFromCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            if (item.register.name == "Det offentlige kartgrunnlaget - Kommunalt")
            {
                return item.dokStatus.description;
            }
            else
            {
                CoverageDataset coverage = Coverage(item, selectedMunicipality);
                if (coverage != null)
                {
                    if (coverage.CoverageDOKStatusId != null)
                    {
                        return coverage.CoverageDOKStatus.description;
                    }
                    else return "Forslag";
                }
                else return "Forslag";
            }
        }

        private static CoverageDataset Coverage(Dataset item, CodelistValue selectedMunicipality)
        {
            string organizationNr = _municipalityService.LookupOrganizationNumberFromMunicipalityCode(selectedMunicipality.value);
            Organization munizipality = _registerService.GetOrganizationByOrganizationNr(organizationNr);

            foreach (CoverageDataset coverage in item.Coverage)
            {
                if (munizipality.systemId == coverage.MunicipalityId)
                {
                    return coverage;
                }
            }
            return null;
        }

        public static string GetConfirmedFromCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            if (item.register.name == "Det offentlige kartgrunnlaget - Kommunalt")
            {
                return "Ja";
            }
            else
            {
                CoverageDataset coverage = Coverage(item, selectedMunicipality);
                if (coverage != null)
                {
                    if (coverage.ConfirmedDok)
                    {
                        return "Ja";
                    }
                    else
                    {
                        return "Nei";
                    }
                }
                else return "Nei";
            }
        }

        public static string GetNoteFromCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            if (item.register.name == "Det offentlige kartgrunnlaget - Kommunalt")
            {
                return item.Notes;
            }
            else
            {
                CoverageDataset coverage = Coverage(item, selectedMunicipality);
                if (coverage != null)
                {
                    return coverage.Note;
                }
                else return null;
            }
        }

        public static CoverageDataset NewCoverage(Dataset dataset)
        {
            return new CoverageDataset()
            {
                dataset = dataset,
                DatasetId = dataset.systemId,
                Municipality = _accessControl.MunicipalUserOrganization()
            };
        }

        public static string IsWMSURL(string WmsUrl)
        {
            if (string.IsNullOrWhiteSpace(WmsUrl))
            {
                return "Nei";
            }
            else
            {
                return "Ja";
            }
        }

    }
}