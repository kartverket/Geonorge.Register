using Kartverket.Register.Models;
using Kartverket.Register.Resources;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using translation = Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Kartverket.Register.Models.ViewModels;
using Resources;

namespace Kartverket.Register.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static readonly RegisterDbContext Db = new RegisterDbContext();
        private static readonly IRegisterItemService RegisteritemService = new RegisterItemService(Db);
        private static readonly IAccessControlService AccessControl = new AccessControlService(Db);
        private static readonly IRegisterService RegisterService = new RegisterService(Db);



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
            return AccessControl.Access(model);
        }

        public static bool IsAdmin()
        {
            return AccessControl.IsAdmin();
        }

        public static bool AccessRegisterItem(RegisterItem item)
        {
            return AccessControl.Access(item);
        }

        public static bool AccessEditDokMunicipalBySelectedMunicipality(string selectedMunicipalityCode)
        {
            return AccessControl.AccessEditOrCreateDOKMunicipalBySelectedMunicipality(selectedMunicipalityCode);
        }


        public static string Type(string containedItemClass)
        {
            if (containedItemClass == "Document")
            {
                return Documents.Document;
            }
            if (containedItemClass == "Dataset")
            {
                return DataSet.Dataset;
            }
            if (containedItemClass == "EPSG")
            {
                return EPSGs.EpsgCode;
            }
            if (containedItemClass == "Organization")
            {
                return Organizations.Organization;
            }
            if (containedItemClass == "CodelistValue")
            {
                return CodelistValues.CodeValue;
            }
            if (containedItemClass == "Register")
            {
                return "Register";
            }
            if (containedItemClass == "NameSpace")
            {
                return Namespace.NamespaceName;
            }
            return "";
        }



        public static List<Models.Register> Registers()
        {
            var registersList = RegisterService.GetRegisters();
            if (registersList.Any())
            {
                registersList.OrderBy(r => r.NameTranslated());
            }
            return registersList;
        }

        public static List<Models.Register> CodelistRegister()
        {
            var registersList = RegisterService.GetCodelistRegisters();
            return registersList;
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
                return AccessControl.GetMunicipality();
            }
            return RegisteritemService.GetMunicipalityByNr(selectedMunicipalityCode.ToString());
        }

        public static string GetDokStatusFromCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            if (item.register.IsDokMunicipal())
            {
                return item.dokStatus.DescriptionTranslated();
            }
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

        public static string GetNationalDokStatus(Dataset item)
        {
            if (item.register.IsDokMunicipal())
            {
                return " ";
            }
            return item.dokStatus.DescriptionTranslated();
        }

        private static CoverageDataset Coverage(Dataset item, CodelistValue selectedMunicipality)
        {
            var municipality = RegisterService.GetOrganizationByMunicipalityCode(selectedMunicipality.value);
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
            var coverage = Coverage(item, selectedMunicipality);
            if (coverage != null)
            {
                if (coverage.ConfirmedDok)
                {
                    return Shared.Yes;
                }
                return Shared.No;
            }
            else return Shared.No;

        }

        public static string GetCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            var coverage = Coverage(item, selectedMunicipality);
            if (coverage != null)
            {
                if (coverage.Coverage)
                {
                    return Shared.Yes;
                }
                return Shared.No;
            }
            else return Shared.No;

        }

        public static string GetNoteFromCoverage(Dataset item, CodelistValue selectedMunicipality)
        {
            if (item.register.IsDokMunicipal())
            {
                return item.Notes;
            }

            var coverage = Coverage(item, selectedMunicipality);
            return coverage?.Note;
        }

        public static bool SosiIsChecked(string statusId)
        {
            if (statusId == "Sosi-valid")
            {
                return true;
            }
            return false;
        }

        public static string GetSelectedMunicipalityName(CodelistValue selectedMunicipal)
        {
            if (selectedMunicipal != null)
            {
                return selectedMunicipal.name;
            }
            return DataSet.DOK_Nasjonalt_SelectMunicipality;
        }

        public static string GetDOKMunicipalConfirmationText(Organization municipality)
        {
            if (municipality != null)
            {
                string confirmed = DataSet.NotConfirmedMunicipalDOK;
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
                    return "<label class='label-" + status + " label auto-width'>" + translation.DataSet.MunicipalDOKStatusDraft + " " + DateTime.Now.Year + lastDateConfirmedText + "</label>";
                }
                else if (municipality.StatusConfirmationMunicipalDOK == "valid")
                {
                    status = "success";
                    confirmed = "";
                    lastDateConfirmedText = GetlastDayConfirmed(municipality);
                }

                return "<label class='label-" + status + " label auto-width'>" + Resource.MunicipalDOKConfirmedInfo(confirmed) + " " + DateTime.Now.Year + lastDateConfirmedText + "</label>";
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

        public static IHtmlString OrderByLink(string sortingSelected, string searchParam, string tittel, string defaultSort, string municipality = null, string inspireRegisteryType = null)
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
            else if (sortingParam == "owner" || sortingParam == "owner_desc" || sortingParam == "datasetOwner" || sortingParam == "datasetOwner_desc")
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
            else if (sortingParam == "dokstatus" || sortingParam == "dokstatus_desc")
            {
                sortTitle = "DOK-Status";
                ; statusIcon = "statusIcon-Accepted";
            }

            // *** DOK STATUS SORTERING
            else if (sortingParam == "dokDeliveryMetadataStatus" || sortingParam == "dokDeliveryMetadataStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Metadata;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "dokDeliveryProductSheetStatus" || sortingParam == "dokDeliveryProductSheetStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_ProductSheet;
                statusIcon += "custom-icon-produktark";
            }
            else if (sortingParam == "dokDeliveryPresentationRulesStatus" || sortingParam == "dokDeliveryPresentationRulesStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_PresentationRules;
                statusIcon = "glyphicon glyphicon-picture";
            }
            else if (sortingParam == "dokDeliveryProductSpecificationStatus" || sortingParam == "dokDeliveryProductSpecificationStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_ProductSpesification;
                statusIcon = "glyphicon glyphicon-list-alt";
            }
            else if (sortingParam == "dokDeliveryWmsStatus" || sortingParam == "dokDeliveryWmsStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Wms;
                statusIcon = "glyphicon glyphicon-globe";
            }
            else if (sortingParam == "dokDeliveryWfsStatus" || sortingParam == "dokDeliveryWfsStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Wfs;
                statusIcon += "custom-icon-wfs";
            }
            else if (sortingParam == "dokDeliverySosiRequirementsStatus" || sortingParam == "dokDeliverySosiRequirementsStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_SosiRequirements;
                statusIcon += "custom-icon-sosi";
            }
            else if (sortingParam == "dokDeliveryGmlRequirementsStatus" || sortingParam == "dokDeliveryGmlRequirementsStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_GmlRequirements;
                statusIcon += "custom-icon-gml";
            }
            else if (sortingParam == "dokDeliveryAtomFeedStatus" || sortingParam == "dokDeliveryAtomFeedStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_AtomFeed;
                statusIcon = "fa fa-rss-square";
            }
            else if (sortingParam == "dokDeliveryDistributionStatus" || sortingParam == "dokDeliveryDistributionStatus_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Distribution;
                statusIcon += "custom-icon-lastned";
            }

            // *** INSPIRE SORTERING

            else if (sortingParam == "inspire_metadata_status" || sortingParam == "inspire_metadata_status_desc")
            {
                sortTitle = InspireDataSet.Metadata;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspire_metadataservice_status" || sortingParam == "inspire_metadataservice_status_desc")
            {
                sortTitle = InspireDataSet.MetadataServiceStatus;
                statusIcon += "custom-icon-metadatatjeneste";
            }
            else if (sortingParam == "inspire_distribution_status" || sortingParam == "inspire_distribution_status_desc")
            {
                sortTitle = InspireDataSet.Distribution;
                statusIcon += "custom-icon-datadeling";
            }
            else if (sortingParam == "inspire_wms_status" || sortingParam == "inspire_wms_status_desc")
            {
                sortTitle = InspireDataSet.WmsStatus;
                statusIcon += "glyphicon glyphicon-globe";
            }
            else if (sortingParam == "inspire_wfs_status" || sortingParam == "inspire_wfs_status_desc")
            {
                sortTitle = InspireDataSet.WfsStatus;
                statusIcon += "custom-icon-wfs";
            }
            else if (sortingParam == "inspire_atom_status" || sortingParam == "inspire_atom_status_desc")
            {
                sortTitle = InspireDataSet.AtomStatus;
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
                statusIcon += "custom-icon-gml";
            }
            else if (sortingParam == "inspire_spatialdataservice_status" || sortingParam == "inspire_spatialdataservice_status_desc")
            {
                sortTitle = InspireDataSet.SpatialDataServiceStatus;
                statusIcon += "custom-icon-info";
            }


            // ** INSPIRE DATA SERVICE

            else if (sortingParam == "inspire_theme_status" || sortingParam == "inspire_theme_status_desc")
            {
                sortTitle = "Inspire tema";
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspire_serviceType" || sortingParam == "inspire_serviceType_desc")
            {
                sortTitle = "Type";
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "request" || sortingParam == "request_desc")
            {
                sortTitle = "Request";
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "sds" || sortingParam == "sds_desc")
            {
                sortTitle = "Sds";
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "networkService" || sortingParam == "networkService_desc")
            {
                sortTitle = "Nettverkstjeneste";
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspireService_metadata_status" || sortingParam == "inspireService_metadata_status_desc")
            {
                sortTitle = "Metadata";
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspire_metadataSearchService_status" || sortingParam == "inspire_metadataSearchService_status_desc")
            {
                sortTitle = "Metadata i søketjeneste";
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "inspire_service_status" || sortingParam == "inspire_service_status_desc")
            {
                sortTitle = "Tjenestestatus";
                statusIcon += "custom-icon-info";
            }

            // *** GEODATALOV SORTERING

            else if (sortingParam == "inspire" || sortingParam == "inspire_desc")
            {
                sortTitle = GeodatalovDataSet.InspireTheme;
                statusIcon += "custom-icon-inspire";
            }
            else if (sortingParam == "dok" || sortingParam == "dok_desc")
            {
                sortTitle = GeodatalovDataSet.Dok;
                statusIcon += "custom-icon-dok";
            }
            else if (sortingParam == "nationalt_dataset" || sortingParam == "nationalt_dataset_desc")
            {
                sortTitle = GeodatalovDataSet.NationalDataset;
                statusIcon += "custom-icon-norge-digitalt";
            }
            else if (sortingParam == "plan" || sortingParam == "plan_desc")
            {
                sortTitle = GeodatalovDataSet.Plan;
                statusIcon += "custom-icon-arealplan";
            }
            else if (sortingParam == "geodatalov" || sortingParam == "geodatalov_desc")
            {
                sortTitle = GeodatalovDataSet.Geodatalov;
                statusIcon += "custom-icon-seksjonstegn";
            }
            else if (sortingParam == "geodatalov_metadata_status" || sortingParam == "geodatalov_metadata_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_Metadata;
                statusIcon += "custom-icon-info";
            }
            else if (sortingParam == "geodatalov_productspecification_status" || sortingParam == "geodatalov_productspecification_status_desc")
            {
                sortTitle = DataSet.DOK_Delivery_ProductSpesification;
                statusIcon += "glyphicon glyphicon-list-alt";
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
                statusIcon += "glyphicon glyphicon-globe";
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
                sortTitle = GeodatalovDataSet.Common;
                statusIcon += "custom-icon-info";
            }

            var linkSort = "<a data-toggle='tooltip' class='show-loading-animation' data-loading-message='Sorterer innhold' data-placement = 'bottom' title='" + sortTitle + "' class='" + sortingClass + "' href='?sorting=" + sortingParam;

            if (municipality != null)
            {
                linkSort += "&municipality=" + municipality;
            }

            if (inspireRegisteryType != null)
            {
                linkSort += "&inspireRegisteryType=" + inspireRegisteryType;
            }

            if (text != null)
                linkSort += "&text=" + text;
            if (string.IsNullOrWhiteSpace(tittel))
                tittel = "<span class='" + statusIcon + "'></span>";

            linkSort = linkSort + "'>" + tittel + "</a>";

            return new HtmlString(linkSort);
        }

        public static IHtmlString GetDokDeliveryStatusSymbol(string status, bool? restricted, string label, string type = null)
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

            if (!string.IsNullOrEmpty(label))
                title = label + ": " + title;

            var html = "";
            if (status == "notset")
                html = "<span class='" + symbolNotSet + "'></span>";
            else
                html = "<span data-toggle='tooltip' data-placement = 'bottom' title='" + title + "'><span class='" + statusSymbol + "'></span></span>";

            return new HtmlString(html);
        }

        

        public static IEnumerable<Models.Register> ParentRegisters(Object model)
        {
            var parentRegister = GetRegister(model);
            var registerList = new List<Models.Register>();
            if (parentRegister != null)
            {
                while (parentRegister != null)
                {
                    if (!registerList.Contains(parentRegister))
                        registerList.Add(parentRegister);
                    parentRegister = parentRegister.parentRegister;
                }
            }
            registerList.Reverse();
            return registerList;

        }

        private static Models.Register GetRegister(object model)
        {
            Models.Register parentRegister = null;
            switch (model)
            {
                case RegisterV2ViewModel registerViewModel:
                    parentRegister = registerViewModel.ParentRegister;
                    break;
                case Models.Register register:
                    parentRegister = register.parentRegister;
                    break;
                case RegisterItem registerItem:
                    parentRegister = registerItem.register;
                    break;
                case RegisterItemV2 registerItemV2:
                    parentRegister = registerItemV2.Register;
                    break;
                case RegisterItemV2ViewModel registerItemV2:
                    parentRegister = registerItemV2.Register;
                    break;
            }

            return parentRegister;
        }


        public static HtmlString Checked(bool isChecked, string type)
        {
            var className = "custom-icon";
            if (isChecked) className = "fa fa-check";
            return new HtmlString("<span data-toggle='tooltip' data-placement = 'bottom' title='" + type + "'><span class='" + className + "'></span></span>");
        }

        public static string GetDistrbutionType(string codeValue)
        {
            return RegisteritemService.GetDistributionType(codeValue);
        }

        public static string TranslateBool(bool value)
        {
            return value ? "Ja" : "Nei";
        }

        public static string GetThumbnail(string thumbnailSrc)
        {
            return thumbnailSrc ?? "/Content/pdf.jpg";
        }
    }
}