using Kartverket.Register.Models;
using Kartverket.Register.Resources;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using translation = Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Resources;

namespace Kartverket.Register.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static readonly RegisterDbContext db = new RegisterDbContext();
        private static IRegisterItemService _registeritemService = new RegisterItemService(db);
        private static IAccessControlService _accessControl = new AccessControlService();
        private static IRegisterService _registerService = new RegisterService(db);

        public static string EnvironmentName(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["EnvironmentName"];
        }

        public static string WebmasterEmail(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["WebmasterEmail"];
        }

        public static string ApplicationVersionNumber(this HtmlHelper helper)
        {
            string versionNumber = WebConfigurationManager.AppSettings["BuildVersionNumber"];
            return versionNumber;
        }

        public static bool SupportsMultiCulture(this HtmlHelper helper)
        {
            return Boolean.Parse(WebConfigurationManager.AppSettings["SupportsMultiCulture"]); ;
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

        public static bool accessRegisterItem(RegisterItem item)
        {
            return _accessControl.Access(item);
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

            // ***** RegisterItems

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

            // ***** Documents 

            else if (sortingType == "documentOwner")
            {
                var sorting = register.items.OfType<Document>().OrderBy(o => o.documentowner.name);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "documentOwner_desc")
            {
                var sorting = register.items.OfType<Document>().OrderByDescending(o => o.documentowner.name);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }

            // ***** Dataset
            else if (sortingType == "datasetOwner")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.datasetowner.name);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "datasetOwner_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.datasetowner.name);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokStatus.description);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokStatus.description);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "distributionFormat")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.DistributionFormat);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "distributionFormat_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.DistributionFormat);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "wmsUrl")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.WmsUrl);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "wmsUrl_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.WmsUrl);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "theme")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.theme == null ? "" : o.theme.value);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "theme_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.theme == null ? "" : o.theme.value);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "type")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.DatasetType);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "type_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.DatasetType);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }

            else if (sortingType == "description")
            {
                sortedList = register.items.OrderBy(o => o.description).ToList();
            }
            else if (sortingType == "description_desc")
            {
                sortedList = register.items.OrderByDescending(o => o.description).ToList();
            }

            //DOK delivery status
            else if (sortingType == "dokDeliveryMetadataStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryMetadataStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryMetadataStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryMetadataStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }

            else if (sortingType == "dokDeliveryProductSheetStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryProductSheetStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryProductSheetStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryProductSheetStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryPresentationRulesStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryPresentationRulesStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryPresentationRulesStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryPresentationRulesStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryProductSpecificationStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryProductSpecificationStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryProductSpecificationStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryProductSpecificationStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryWmsStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryWmsStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryWmsStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryWmsStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryWfsStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryWfsStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryWfsStatusId_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryWfsStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliverySosiRequirementsStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliverySosiRequirementsStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliverySosiRequirementsStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliverySosiRequirementsStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryDistributionStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryDistributionStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryDistributionStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryDistributionStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryGmlRequirementsStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryGmlRequirementsStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryGmlRequirementsStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryGmlRequirementsStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryAtomFeedStatus")
            {
                var sorting = register.items.OfType<Dataset>().OrderBy(o => o.dokDeliveryAtomFeedStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dokDeliveryAtomFeedStatus_desc")
            {
                var sorting = register.items.OfType<Dataset>().OrderByDescending(o => o.dokDeliveryAtomFeedStatusId);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }


            // ***** CodelistValue

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

            // ***** Organization

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

            // ***** EPSG
            else if (sortingType == "verticalReferenceSystem")
            {
                var sorting = register.items.OfType<EPSG>().OrderBy(o => o.verticalReferenceSystem);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "verticalReferenceSystem_desc")
            {
                var sorting = register.items.OfType<EPSG>().OrderByDescending(o => o.verticalReferenceSystem);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "horizontalReferenceSystem")
            {
                var sorting = register.items.OfType<EPSG>().OrderBy(o => o.horizontalReferenceSystem);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "horizontalReferenceSystem_desc")
            {
                var sorting = register.items.OfType<EPSG>().OrderByDescending(o => o.horizontalReferenceSystem);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dimension")
            {
                var sorting = register.items.OfType<EPSG>().OrderBy(o => o.dimension == null ? "" : o.dimension.description);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "dimension_desc")
            {
                var sorting = register.items.OfType<EPSG>().OrderByDescending(o => o.dimension == null ? "" : o.dimension.description);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "epsg")
            {
                var sorting = register.items.OfType<EPSG>().OrderBy(o => o.epsgcode);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "epsg_desc")
            {
                var sorting = register.items.OfType<EPSG>().OrderByDescending(o => o.epsgcode);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "sosiReferencesystem")
            {
                var sorting = register.items.OfType<EPSG>().OrderBy(o => o.sosiReferencesystem);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "sosiReferencesystem_desc")
            {
                var sorting = register.items.OfType<EPSG>().OrderByDescending(o => o.sosiReferencesystem);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "inspireRequirement")
            {
                var sorting = register.items.OfType<EPSG>().OrderBy(o => o.inspireRequirement.sortOrder);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "inspireRequirement_desc")
            {
                var sorting = register.items.OfType<EPSG>().OrderByDescending(o => o.inspireRequirement.sortOrder);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "nationalRequirement")
            {
                var sorting = register.items.OfType<EPSG>().OrderBy(o => o.nationalRequirement.sortOrder);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "nationalRequirement_desc")
            {
                var sorting = register.items.OfType<EPSG>().OrderByDescending(o => o.nationalRequirement.sortOrder);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "nationalSeasRequirement")
            {
                var sorting = register.items.OfType<EPSG>().OrderBy(o => o.nationalSeasRequirement.sortOrder);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "nationalSeasRequirement_desc")
            {
                var sorting = register.items.OfType<EPSG>().OrderByDescending(o => o.nationalSeasRequirement.sortOrder);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }

            // ***** ServiceAlert
            else if (sortingType == "alertdate")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderBy(o => o.AlertDate);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "alertdate_desc")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderByDescending(o => o.AlertDate);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "effektivedate")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderBy(o => o.EffectiveDate);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "effektivedate_desc")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderByDescending(o => o.EffectiveDate);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "owner")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderBy(o => o.Owner);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "owner_desc")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderByDescending(o => o.Owner);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "servicetype")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderBy(o => o.ServiceType);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "servicetype_desc")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderByDescending(o => o.ServiceType);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "servicealert")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderBy(o => o.AlertType);
                sortedList = sorting.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "servicealert_desc")
            {
                var sorting = register.items.OfType<ServiceAlert>().OrderByDescending(o => o.AlertType);
                sortedList = sorting.Cast<RegisterItem>().ToList();
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

        public static List<Models.Register> registers(Models.Register current)
        {
            List<Models.Register> registerList = new List<Models.Register>();
            Models.Register main = mainRegister(current);
            Models.Register parentRegister = current.parentRegister;
            if (parentRegister != null)
            {
                while (main != null && parentRegister != main)
                {
                    if (!registerList.Contains(parentRegister))
                        registerList.Add(parentRegister);
                    parentRegister = parentRegister.parentRegister;
                }
            }
            registerList.Reverse();

            if (!registerList.Contains(main) && main != current)
                registerList.Insert(0, main);

            //registerList.Add(current);

            return registerList;

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

        public static string StatusBeskrivelse(Models.Register register)
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
                    if (register.name != "Produktspesifikasjoner")
                    {
                        if (s.value != "Sosi-valid")
                        {
                            gyldig += "- " + s.description + "&#013";
                        }
                    }
                    else
                    {
                        gyldig += "- " + s.description + "&#013";
                    }
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
            return translation.Registers.ErrorMessageValidationName;
        }

        public static string ErrorMessageValidationDataset()
        {
            return translation.DataSet.ErrorMessageValidationDataset;
        }

        public static CodelistValue GetSelectedMunicipality(string selectedMunicipalityCode)
        {
            if (selectedMunicipalityCode == null)
            {
                return _accessControl.GetMunicipality();
            }
            else
            {
                return _registeritemService.GetMunicipalityByNr(selectedMunicipalityCode.ToString());
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

        public static string GetNationalDokStatus(Dataset item)
        {
            if (item.register.IsDokMunicipal())
            {
                return " ";
            }
            else
                return item.dokStatus.description;
        }

        private static CoverageDataset Coverage(Dataset item, CodelistValue selectedMunicipality)
        {
            Organization municipality = _registerService.GetOrganizationByMunicipalityCode(selectedMunicipality.value);
            if (municipality != null)
            {
                foreach (CoverageDataset coverage in item.Coverage)
                {
                    if (municipality.systemId == coverage.MunicipalityId)
                    {
                        return coverage;
                    }
                }
            }
            return null;
        }

        public static string GetConfirmedFromCoverage(Dataset item, CodelistValue selectedMunicipality)
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

        public static string GetCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            CoverageDataset coverage = Coverage(item, selectedMunicipality);
            if (coverage != null)
            {
                if (coverage.Coverage)
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

        public static string GetNoteFromCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            if (item.register.IsDokMunicipal())
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

        public static bool SosiIsChecked(string statusId)
        {
            if (statusId == "Sosi-valid")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AccessEditDOKMunicipalBySelectedMunicipality(string selectedMunicipalityCode)
        {
            return _accessControl.AccessEditOrCreateDOKMunicipalBySelectedMunicipality(selectedMunicipalityCode);
        }

        public static string GetSelectedMunicipalityName(CodelistValue selectedMunicipal)
        {
            if (selectedMunicipal != null)
            {
                return selectedMunicipal.name;
            }
            return translation.DataSet.DOK_Nasjonalt_SelectMunicipality;
        }


        public static string GetDOKMunicipalConfirmationText(Organization municipality)
        {
            if (municipality != null)
            {
                string confirmed = translation.DataSet.NotConfirmedMunicipalDOK;
                string lastDateConfirmedText = "";
                string status = "danger";
                if (lastDateConfirmedIsNotFromThisYear(municipality.DateConfirmedMunicipalDOK))
                {
                    lastDateConfirmedText = GetlastDayConfirmed(municipality);
                }
                else if (municipality.StatusConfirmationMunicipalDOK == "draft")
                {
                    status = "warning";
                    lastDateConfirmedText = GetlastDayConfirmed(municipality);
                    return "<label class='label-" + status + " label auto-width'>"+ translation.DataSet.MunicipalDOKStatusDraft + " " + DateTime.Now.Year + lastDateConfirmedText + "</label>";
                }
                else if (municipality.StatusConfirmationMunicipalDOK == "valid")
                {
                    status = "success";
                    confirmed = "";
                    lastDateConfirmedText = GetlastDayConfirmed(municipality);
                }

                return "<label class='label-" + status + " label auto-width'>" + Resource.MunicipalDOKConfirmedInfo(confirmed) + " "  + DateTime.Now.Year + lastDateConfirmedText + "</label>";
            }
            return "";
        }

        private static bool lastDateConfirmedIsNotFromThisYear(DateTime? dateConfirmedMunicipalDOK)
        {
            if (dateConfirmedMunicipalDOK != null)
            {
                return dateConfirmedMunicipalDOK.Value.Year != DateTime.Now.Year;
            }
            return false;
        }

        private static string GetlastDayConfirmed(Organization municipality)
        {
            if (municipality.DateConfirmedMunicipalDOK != null)
            {
                return " (" + municipality.DateConfirmedMunicipalDOK.Value.ToString("dd.MM.yyyy") + ")";
            }

            return null;
        }

        public static int cbChecked(bool checkboxChecked)
        {
            if (checkboxChecked)
            {
                return 1;
            }
            return 0;
        }
        public static IHtmlString OrderByLink(string sortingSelected, string searchParam, string tittel, string defaultSort)
        {

            var sortingClass = "";
            var sortTitle = "";
            var sortingParam = "";
            var statusIcon = "custom-icon ";

            if (sortingSelected == null)
                sortingSelected = "name";

            if (sortingSelected == defaultSort)
            {
                sortingClass = "sorted-asc";
                sortTitle = DataSet.DOK_Delivery_Title;
                sortingParam = defaultSort + "_desc";
            }
            else if (sortingSelected.IndexOf("_desc") > -1 && sortingSelected == defaultSort + "_desc")
            {
                sortingClass = "sorted-desc";
                sortTitle = DataSet.DOK_Delivery_Title;
                sortingParam = defaultSort;
            }
            else
            {
                sortingClass = "";
                sortTitle = "";
                sortingParam = defaultSort;
            }

            if (sortingParam.IndexOf("Requirement") > -1)
            {
                sortTitle = "Sortert etter logisk rekkefølge";
            }
            var text = searchParam;

            if (sortingParam == "title" || sortingParam == "title_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Title;
            }
            else if (sortingParam == "owner" || sortingParam == "owner_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Owner;
            }
            else if (sortingParam == "theme" || sortingParam == "theme_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Theme;
            }
            else if (sortingParam == "metadata" || sortingParam == "metadata_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Metadata;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "metadataservice" || sortingParam == "metadataservice_desc")
            {
                sortTitle = InspireDataSet.MetadataServiceStatus;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "productSheet" || sortingParam == "metadataservice_desc")
            {
                sortTitle = DataSet.DOK_Delivery_ProductSheet;
                statusIcon += "";
            }
            else if (sortingParam == "presentationRules" || sortingParam == "presentationRules_desc")
            {
                sortTitle = DataSet.DOK_Delivery_PresentationRules;
                statusIcon += "";
            }
            else if (sortingParam == "productSpecification" || sortingParam == "productSpecification_desc")
            {
                sortTitle = DataSet.DOK_Delivery_ProductSpesification;
                statusIcon += "";
            }
            else if (sortingParam == "wms" || sortingParam == "wms_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Wms;
                statusIcon += "custom-icon-wfs";
            }
            else if (sortingParam == "wfs" || sortingParam == "wfs_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Wfs;
                statusIcon += "custom-icon-wfs";
            }
            else if (sortingParam == "sosi" || sortingParam == "sosi_desc")
            {
                sortTitle = DataSet.DOK_Delivery_SosiRequirements;
                statusIcon += "";
            }
            else if (sortingParam == "distribution" || sortingParam == "distribution_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Distribution;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "gml" || sortingParam == "gml_desc")
            {
                sortTitle = DataSet.DOK_Delivery_GmlRequirements;
                statusIcon += "";
            }
            else if (sortingParam == "atom" || sortingParam == "atom_desc")
            {
                sortTitle = DataSet.DOK_Delivery_AtomFeed;
                statusIcon = "fa fa-rss-square";
            }
            else if (sortingParam == "wfsoratom" || sortingParam == "wfsoratom_desc")
            {
                sortTitle = InspireDataSet.WfsOrAtomStatus;
                statusIcon = "fa fa-rss-square";
            }
            else if (sortingParam == "harmonizeddata" || sortingParam == "harmonizeddata_desc")
            {
                sortTitle = InspireDataSet.HarmonizedDataStatus;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "spatialdataservice" || sortingParam == "spatialdataservice_desc")
            {
                sortTitle = InspireDataSet.SpatialDataServiceStatus;
                statusIcon += "custom-icon-info";
            }

            var linkSort = "<a data-toggle='tooltip' class='show-loading-animation' data-loading-message='Sorterer innhold' data-placement = 'bottom' title='" + sortTitle + "' class='" + sortingClass + "' href='?sorting=" + sortingParam;

            if (text != null)
                linkSort = linkSort + "&text=" + text;
            if (string.IsNullOrWhiteSpace(tittel))
                tittel = "<span class='" + statusIcon + "'></span>";
            
            linkSort = linkSort + "'>" + tittel + "</a>";

            return new HtmlString(linkSort);
        }

        public static IHtmlString GetDokDeliveryStatusSymbol(string status, bool? restricted, string type = null)
        {

            var symbolDeficient = "custom-icon custom-icon-smile-red";
            var symbolUseable = "custom-icon custom-icon-smile-yellow";
            var symbolGood = "custom-icon custom-icon-smile-green";
            var symbolNotSet = "custom-icon";

            var statusSymbol = symbolUseable;
            var title = "";

            if (restricted.HasValue && restricted.Value && type != "metadata")
            {
                statusSymbol = "custom-icon custom-icon-hengelaas-closed-red";
                title = DataSet.DOK_Delivery_Restricted;
            }
            else if (!string.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "notset":
                        statusSymbol = symbolNotSet;
                        title = DataSet.DOK_Delivery_Status_NotSet;
                        break;
                    case "deficient":
                        statusSymbol = symbolDeficient;
                        title = DataSet.DOK_Delivery_Status_Deficient;
                        break;
                    case "useable":
                        statusSymbol = symbolUseable;
                        title = DataSet.DOK_Delivery_Status_Useable;
                        break;
                    case "good":
                        statusSymbol = symbolGood;
                        title = DataSet.DOK_Delivery_Status_Good;
                        break;
                }
            }

            var label = "";

            switch (type)
            {
                case "metadata":
                    label = DataSet.DOK_Delivery_Metadata;
                    break;
                case "metadataservice":
                    label = InspireDataSet.MetadataServiceStatus;
                    break;
                case "ProductSheet":
                    label = DataSet.DOK_Delivery_ProductSheet;
                    break;
                case "presentationRules":
                    label = DataSet.DOK_Delivery_PresentationRules;
                    break;
                case "productSpecification":
                    label = DataSet.DOK_Delivery_ProductSpesification;
                    break;
                case "wms":
                    label = DataSet.DOK_Delivery_Wms;
                    break;
                case "wfs":
                    label = DataSet.DOK_Delivery_Wfs;
                    break;
                case "sosi":
                    label = DataSet.DOK_Delivery_SosiRequirements;
                    break;
                case "distribution":
                    label = DataSet.DOK_Delivery_Distribution;
                    break;
                case "gml":
                    label = DataSet.DOK_Delivery_GmlRequirements;
                    break;
                case "atom":
                    label = DataSet.DOK_Delivery_AtomFeed;
                    break;
                case "harmonizeddata":
                    label = InspireDataSet.HarmonizedDataStatus;
                    break;
                case "spatialdataservice":
                    label = InspireDataSet.SpatialDataServiceStatus;
                    break;
                case "wfsoratom":
                    label = InspireDataSet.WfsOrAtomStatus;
                    break;
            }


            if (!string.IsNullOrEmpty(label))
                title = label + ": " + title;

            var html = "";
            if (status == "notset")
                html = "<span class='" + symbolNotSet + "'></span>";
            else
                html = "<span data-toggle='tooltip' data-placement = 'bottom' title='" + title + "'><span class='" + statusSymbol + "'></span></span>";

            return new HtmlString(html);
        }

    }
}