using Kartverket.Register.Models;
using Kartverket.Register.Resources;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using translation = Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Kartverket.Register.Models.ViewModels;
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

        public static bool accessRegisterItem(RegisterItem item)
        {
            return _accessControl.Access(item);
        }

        public static string lovligInnhold(string containedItemClass)
        {
            if (containedItemClass == "Document")
            {
                return Documents.Document;
            }
            else if (containedItemClass == "Dataset")
            {
                return DataSet.Dataset;
            }
            else if (containedItemClass == "EPSG")
            {
                return EPSGs.EpsgCode;
            }
            else if (containedItemClass == "Organization")
            {
                return Organizations.Organization;
            }
            else if (containedItemClass == "CodelistValue")
            {
                return CodelistValues.CodeValue;
            }
            else if (containedItemClass == "Register")
            {
                return "Register";
            }
            else if (containedItemClass == "NameSpace")
            {
                return Namespace.NamespaceName;
            }
            return "";
        }

        public static string Type(string containedItemClass)
        {
            if (containedItemClass == "Document")
            {
                return Documents.Document;
            }
            else if (containedItemClass == "Dataset")
            {
                return DataSet.Dataset;
            }
            else if (containedItemClass == "EPSG")
            {
                return EPSGs.EpsgCode;
            }
            else if (containedItemClass == "Organization")
            {
                return Organizations.Organization;
            }
            else if (containedItemClass == "CodelistValue")
            {
                return CodelistValues.CodeValue;
            }
            else if (containedItemClass == "Register")
            {
                return "Register";
            }
            else if (containedItemClass == "NameSpace")
            {
                return Namespace.NamespaceName;
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

            string forslag = translation.Registers.Proposal + ":&#013";
            string gyldig = translation.Registers.Valid + ":&#013";
            string historiske = translation.Registers.Historical + ":&#013";

            foreach (Status s in queryResults)
            {
                if (s.group == "suggested")
                {
                    forslag += "- " + s.DescriptionTranslated() + "&#013";
                }
                if (s.group == "historical")
                {
                    historiske += "- " + s.DescriptionTranslated() + "&#013";
                }
                if (s.group == "current")
                {
                    if (register.name != "Produktspesifikasjoner")
                    {
                        if (s.value != "Sosi-valid")
                        {
                            gyldig += "- " + s.DescriptionTranslated() + "&#013";
                        }
                    }
                    else
                    {
                        gyldig += "- " + s.DescriptionTranslated() + "&#013";
                    }
                }
            }

            string beskrivelse = translation.Registers.StatusAvailable + ":&#013&#013" + forslag + "&#013" + gyldig + "&#013" + historiske;
            return beskrivelse;
        }

        public static object DokStatusBeskrivelse()
        {
            return "Forslag&#013Kandidat&#013I Prosess&#013Godkjent&#013";

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
                return item.dokStatus.DescriptionTranslated();
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
                return item.dokStatus.DescriptionTranslated();
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
                    return Shared.Yes;
                }
                else
                {
                    return Shared.No;
                }
            }
            else return Shared.No;

        }

        public static string GetCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            CoverageDataset coverage = Coverage(item, selectedMunicipality);
            if (coverage != null)
            {
                if (coverage.Coverage)
                {
                    return Shared.Yes;
                }
                else
                {
                    return Shared.No;
                }
            }
            else return Shared.No;

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
            else if (sortingParam == "productsheet" || sortingParam == "productsheet_desc")
            {
                sortTitle = DataSet.DOK_Delivery_ProductSheet;
                statusIcon += "custom-icon-produktark";
            }
            else if (sortingParam == "presentationrules" || sortingParam == "presentationrules_desc")
            {
                sortTitle = DataSet.DOK_Delivery_PresentationRules;
                statusIcon += "glyphicon-picture";
            }
            else if (sortingParam == "productspecification" || sortingParam == "productspecification_desc")
            {
                sortTitle = DataSet.DOK_Delivery_ProductSpesification;
                statusIcon += "glyphicon-list-alt";
            }

            // *** INSPIRE SORTERING

            else if (sortingParam == "inspire_metadata_status" || sortingParam == "inspire_metadata_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Metadata;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspire_metadataservice_status" || sortingParam == "inspire_metadataservice_status_desc")
            {
                sortTitle = InspireDataSet.MetadataServiceStatus;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspire_distribution_status" || sortingParam == "inspire_distribution_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Distribution;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspire_wms_status" || sortingParam == "inspire_wms_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Wms;
                statusIcon += "custom-icon-wfs";
            }
            else if (sortingParam == "inspire_wfs_status" || sortingParam == "inspire_wfs_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Wfs;
                statusIcon += "custom-icon-wfs";
            }
            else if (sortingParam == "inspire_atom_status" || sortingParam == "inspire_atom_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_AtomFeed;
                statusIcon = "fa fa-rss-square";
            }
            else if (sortingParam == "inspire_wfsoratom_status" || sortingParam == "inspire_wfsoratom_status_desc")
            {
                sortTitle = InspireDataSet.WfsOrAtomStatus;
                statusIcon = "fa fa-rss-square";
            }
            else if (sortingParam == "inspire_harmonizeddata_status" || sortingParam == "inspire_harmonizeddata_status_desc")
            {
                sortTitle = InspireDataSet.HarmonizedDataStatus;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspire_spatialdataservice_status" || sortingParam == "inspire_spatialdataservice_status_desc")
            {
                sortTitle = InspireDataSet.SpatialDataServiceStatus;
                statusIcon += "custom-icon-info";
            }

            // *** GEODATALOV SORTERING

            else if (sortingParam == "geodatalov_metadata_status" || sortingParam == "geodatalov_metadata_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Metadata;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "geodatalov_productspecification_status" || sortingParam == "geodatalov_productspecification_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_ProductSpesification;
                statusIcon += "glyphicon-list-alt";
            }
            else if (sortingParam == "geodatalov_sosi_status" || sortingParam == "geodatalov_sosi_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_SosiRequirements;
                statusIcon += "custom-icon-sosi";
            }
            else if (sortingParam == "geodatalov_gml_status" || sortingParam == "geodatalov_gml_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_GmlRequirements;
                statusIcon += "custom-icon-gml";
            }
            else if (sortingParam == "geodatalov_wms_status" || sortingParam == "geodatalov_wms_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Wms;
                statusIcon += "custom-icon-wfs";
            }
            else if (sortingParam == "geodatalov_wfs_status" || sortingParam == "geodatalov_wfs_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Wfs;
                statusIcon += "custom-icon-wfs";
            }
            else if (sortingParam == "geodatalov_atom_status" || sortingParam == "geodatalov_atom_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_AtomFeed;
                statusIcon = "fa fa-rss-square";
            }
            else if (sortingParam == "geodatalov_common_status" || sortingParam == "geodatalov_common_status_desc")
            {
                sortTitle = DataSet.Delivery_Common;
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

        public static IEnumerable<Models.Register> ParentRegisters(RegisterV2ViewModel currentRegister)
        {
            var registerList = new List<Models.Register>();
            var parentRegister = ParentRegister(currentRegister.ParentRegister);
            if (parentRegister != null)
            {
                while (parentRegister != null)
                {
                    if (!registerList.Contains(parentRegister))
                        registerList.Add(parentRegister);
                    parentRegister = ParentRegister(parentRegister.parentRegister);
                }
            }
            registerList.Reverse();
            return registerList;

        }

        public static IEnumerable<Models.Register> ParentRegisters(Models.Register currentRegister)
        {
            var registerList = new List<Models.Register>();
            var parentRegister = ParentRegister(currentRegister.parentRegister);
            if (parentRegister != null)
            {
                while (parentRegister != null)
                {
                    if (!registerList.Contains(parentRegister))
                        registerList.Add(parentRegister);
                    parentRegister = ParentRegister(parentRegister.parentRegister);
                }
            }
            registerList.Reverse();
            return registerList;

        }


        public static Models.Register ParentRegister(Models.Register register)
        {
            return register;
        }
        

    }
}