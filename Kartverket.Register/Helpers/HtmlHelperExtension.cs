using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Kartverket.Register.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string ApplicationVersionNumber(this HtmlHelper helper)
        {
            string versionNumber = WebConfigurationManager.AppSettings["BuildVersionNumber"];
            return versionNumber;
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


        public static bool accessRegister(Kartverket.Register.Models.Register register)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if ((register.accessId == 1) && role == "nd.metadata_admin")
            {
                return true;
            }
            if (register.accessId == 2 && (role == "nd.metadata_admin" || role == "nd.metadata" || role == "nd.metadata_editor"))
            {
                return true;
            }
            return false;
        }

        public static bool accessRegisterItem(RegisterItem item)
        {
            RegisterDbContext db = new RegisterDbContext();

            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if ((item.register.accessId == 1 || item.register.accessId == 2) && role == "nd.metadata_admin")
            {
                return true;
            }

            if (item.register.accessId == 2 && (role == "nd.metadata" || role == "nd.metadata_editor"))
            {
                if (item.register.containedItemClass == "Document")
                {
                    Kartverket.Register.Models.Document document = db.Documents.Find(item.systemId);
                    if (document.documentowner.name == user)
                    {
                        return true;
                    }
                    return false;
                }

                if (item.register.containedItemClass == "Dataset")
                {
                    Kartverket.Register.Models.Dataset dataset = db.Datasets.Find(item.systemId);
                    if (dataset.datasetowner.name == user)
                    {
                        return true;
                    }
                    return false;
                }

                if (item.submitter.name == user)
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

            return "";
        }

        public static List<Kartverket.Register.Models.Register> Registers()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResults = from o in db.Registers
                               where o.parentRegisterId == null
                               select o;

            List<Kartverket.Register.Models.Register> RegistersList = new List<Kartverket.Register.Models.Register>();
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

        public static string GeonorgeUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["GeonorgeUrl"];
        }
        public static string GeonorgeArtiklerUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["GeonorgeArtiklerUrl"];
        }
        public static string NorgeskartUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["NorgeskartUrl"];
        }
        public static string RegistryUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["RegistryUrl"];
        }
        public static string ObjektkatalogUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["ObjektkatalogUrl"];
        }
        public static string KartkatalogenUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["KartkatalogenUrl"];
        }

        public static string DokRegisterUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["DokRegisterUrl"];
        }

        public static string ToUrl(this HtmlHelper helper, string name)
        {
            return MakeSeoFriendlyString(name);
        }

        public static string MakeSeoFriendlyString(string input)
        {
            string encodedUrl = (input ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "a").Replace("æ", "ae").Replace("ø", "o");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }


        // SORTERING av registeritems
        public static List<Kartverket.Register.Models.RegisterItem> SortingRegisterItems(Kartverket.Register.Models.Register register, String sortingType)
        {

            string text = HttpContext.Current.Request.QueryString["text"] != null ? HttpContext.Current.Request.QueryString["text"].ToString() : "";
            string filterVertikalt = HttpContext.Current.Request.QueryString["filterVertikalt"] != null ? HttpContext.Current.Request.QueryString["filterVertikalt"].ToString() : "";
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

                    string redirect = HttpContext.Current.Request.Path + "?sorting=" + sortingType;

                    if (text != "")
                        redirect = redirect + "&text=" + text;

                    if (filterVertikalt != "")
                    {
                        if (filterVertikalt.Contains(","))
                            filterVertikalt = filterVertikalt.Replace(",false", "");
                        redirect = redirect + "&filterVertikalt=" + filterVertikalt;
                    }

                    if (filterHorisontalt != "")
                    {
                        if (filterHorisontalt.Contains(","))
                            filterHorisontalt = filterHorisontalt.Replace(",false", "");
                        redirect = redirect + "&filterHorisontalt=" + filterHorisontalt;
                    }

                    if (InspireRequirementParam != "")
                        redirect = redirect + "&inspireRequirement=" + InspireRequirementParam;

                    if (nationalRequirementParam != "")
                        redirect = redirect + "&nationalRequirement=" + nationalRequirementParam;

                    if (nationalSeaRequirementParam != "")
                        redirect = redirect + "&nationalSeaRequirement=" + nationalSeaRequirementParam;

                    HttpContext.Current.Response.Redirect(redirect);
                }
            }
            HttpContext.Current.Session["sortingType"] = sortingType;

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
        public static List<Kartverket.Register.Models.Register> SortingRegisters(Kartverket.Register.Models.Register Model, String sortingType)
        {
            string text = HttpContext.Current.Request.QueryString["text"] != null ? HttpContext.Current.Request.QueryString["text"].ToString() : "";
            string filterVertikalt = HttpContext.Current.Request.QueryString["filterVertikalt"] != null ? HttpContext.Current.Request.QueryString["filterVertikalt"].ToString() : "";
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

                    string redirect = HttpContext.Current.Request.Path + "?sorting=" + sortingType;

                    if (text != "")
                        redirect = redirect + "&text=" + text;

                    if (filterVertikalt != "")
                    {
                        if (filterVertikalt.Contains(","))
                            filterVertikalt = filterVertikalt.Replace(",false", "");
                        redirect = redirect + "&filterVertikalt=" + filterVertikalt;
                    }

                    if (filterHorisontalt != "")
                    {
                        if (filterHorisontalt.Contains(","))
                            filterHorisontalt = filterHorisontalt.Replace(",false", "");
                        redirect = redirect + "&filterHorisontalt=" + filterHorisontalt;
                    }

                    if (InspireRequirementParam != "")
                        redirect = redirect + "&inspireRequirement=" + InspireRequirementParam;

                    if (nationalRequirementParam != "")
                        redirect = redirect + "&nationalRequirement=" + nationalRequirementParam;

                    if (nationalSeaRequirementParam != "")
                        redirect = redirect + "&nationalSeaRequirement=" + nationalSeaRequirementParam;

                    HttpContext.Current.Response.Redirect(redirect);
                }
            }
            HttpContext.Current.Session["sortingType"] = sortingType;

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

        public static Kartverket.Register.Models.Register mainRegister(Kartverket.Register.Models.Register register)
        {
            Kartverket.Register.Models.Register parentRegister;
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

        private static Kartverket.Register.Models.Register getParentRegister(Kartverket.Register.Models.Register register)
        {
            Kartverket.Register.Models.Register parentRegister;
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


        //public static List<Kartverket.Register.Models.Register> parentRegisterList(Kartverket.Register.Models.Register model)
        //{
        //    List<Models.Register> parentsList = new List<Models.Register>();
        //    Models.Register register = model;
        //    Models.Register mainRegister;

        //    if (register.parentRegister != null)
        //    {
        //        parentsList.Add(model.parentRegister);
        //        register = model.parentRegister;

        //        register = hasParentRegister(model, parentsList, register);
        //    }
        //    else {
        //        mainRegister = register;
        //    }

        //}

        private static Models.Register hasParentRegister(Kartverket.Register.Models.Register model, List<Models.Register> parentsList, Models.Register register)
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
    }

}