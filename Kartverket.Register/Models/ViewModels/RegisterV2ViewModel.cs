using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.StatusReports;
using Kartverket.Register.Models.Translations;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterV2ViewModel
    {
        private Register _register;

        public Register Register { get {return _register;} }

        public Guid SystemId { get; set; }

        [Display(Name = "Owner", ResourceType = typeof(Registers))]
        public Organization Owner { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Registers))]
        public string Name { get; set; }

        public string Seoname { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Registers))]
        public string Description { get; set; }

        public Status Status { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSubmitted { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Modified { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateAccepted { get; set; }

        [Display(Name = "ContainedItemClass", ResourceType = typeof(Registers))]
        public string ContainedItemClass { get; set; }

        public ICollection<RegisterItem> RegisterItems { get; set; }
        public ICollection<RegisterItemV2ViewModel> RegisterItemsV2 { get; set; }
        public ICollection<RegisterItemV2ViewModel> InspireDataService { get; set; }

        public Register ParentRegister { get; set; }

        public ICollection<Register> Subregisters { get; set; }

        public string TargetNamespace { get; set; }

        public Version Versioning { get; set; }

        public int VersionNumber { get; set; }

        public TranslationCollection<RegisterTranslation> Translations { get; set; }

        public string OrderBy { get; set; }
        public int AccessId { get; set; }
        public InspireReportViewModel InspireReport { get; set; }


        public string MunicipalityCode { get; set; }
        public Organization Municipality { get; set; }

        public AccessViewModel AccessRegister { get; set; }

        public SynchronizationViewModel SynchronizationJobs { get; set; }
        public StatusReportViewModel StatusReport { get; set; }

        public string SelectedInspireRegisteryType { get; set; }
        public string SelectedDokTab { get; set; }
        public string SelectedGeodatalovTab { get; set; }
        public string SelectedMareanoTab { get; set; }

        public RegisterV2ViewModel(Register register, FilterParameters filter, int? page = null, StatusReport statusReport = null, List<StatusReport> statusReports = null)
        {
            if (register != null)
            {
                _register = register;
                SystemId = register.systemId;
                Owner = register.owner;
                Name = register.NameTranslated();
                Description = register.DescriptionTranslated();
                Status = register.status;
                DateSubmitted = register.dateSubmitted;
                Modified = register.modified;
                DateAccepted = register.dateAccepted;
                ContainedItemClass = register.containedItemClass;
                RegisterItems = register.items;
                ParentRegister = register.parentRegister;
                Subregisters = register.subregisters;
                TargetNamespace = register.targetNamespace;
                Seoname = register.seoname;
                Versioning = register.versioning;
                VersionNumber = register.versionNumber;
                RegisterItemsV2 = GetRegisterItems(register.containedItemClass, register.RegisterItems, filter);
                SynchronizationJobs = new SynchronizationViewModel(register.Synchronizes, page);
                StatusReport = GetStatsReport(statusReport, statusReports, filter);


                if (register.accessId != null) AccessId = register.accessId.Value;

                if (register.IsAlertRegister())
                {
                    if (string.IsNullOrWhiteSpace(OrderBy))
                    {
                        OrderBy = "dateSubmitted_desc";
                    }
                }
            }
        }

        
        private StatusReportViewModel GetStatsReport(StatusReport statusReport, List<StatusReport> statusReports, FilterParameters filter)
        {
            if (statusReport != null)
            {
                if (statusReport.IsDokReport())
                {
                    return new DokStatusReportViewModel(statusReport, statusReports, filter.StatusType);
                }

                if (statusReport.IsInspireRegistryReport())
                {
                    return new InspireRegistryStatusReportViewModel(statusReport, statusReports, filter);
                }

                if (statusReport.IsGeodatalovDatasetReport())
                {
                    return new GeodatalovDatasetStatusReportViewModel(statusReport, statusReports, filter.StatusType);
                }

                if (statusReport.IsMareanoDatasetReport())
                {
                    return new MareanoDatasetStatusReportViewModel(statusReport, statusReports, filter.StatusType);
                }
            }
            return null;
        }


        private ICollection<RegisterItemV2ViewModel> GetRegisterItems(string containedItemClass, ICollection<RegisterItemV2> registerItems, FilterParameters filter)
        {
            var registerItemsViewModel = new Collection<RegisterItemV2ViewModel>();
            switch (containedItemClass)
            {
                case "InspireDataset":
                    
                    foreach (var inspireRegisterItem in registerItems)
                    {
                        if(Inspire.IncludeInFilter(inspireRegisterItem, filter))
                        { 
                            switch (inspireRegisterItem)
                            {
                                case InspireDataset inspireDataset:
                                    registerItemsViewModel.Add(new InspireDatasetViewModel(inspireDataset));
                                    break;
                                case InspireDataService inspireDataService:
                                    registerItemsViewModel.Add(new InspireDataServiceViewModel(inspireDataService));
                                    break;
                            }
                        }
                    }
                    break;
                case "GeodatalovDataset":
                    foreach (GeodatalovDataset geodatalovDataset in registerItems)
                    {
                        registerItemsViewModel.Add(new GeodatalovDatasetViewModel(geodatalovDataset));
                    }
                    break;
                case "MareanoDataset":
                    foreach (MareanoDataset mareanoDataset in registerItems)
                    {
                        registerItemsViewModel.Add(new MareanoDatasetViewModel(mareanoDataset));
                    }
                    break;
            }
            return registerItemsViewModel;
        }
 
        private ICollection<RegisterItemV2ViewModel> GetInspireDataService(string containedItemClass, ICollection<RegisterItemV2> registerItems)
        {
            var registerItemsViewModel = new Collection<RegisterItemV2ViewModel>();
            if (containedItemClass == "InspireDataset")
                foreach (var inspireRegisterItem in registerItems)
                {
                    if (inspireRegisterItem is InspireDataService inspireDataService)
                    {
                        registerItemsViewModel.Add(new InspireDataServiceViewModel(inspireDataService));
                    }
                }

            return registerItemsViewModel;
        }

        public bool IsDokMunicipal()
        {
            return SystemId == Guid.Parse(GlobalVariables.DokMunicipalRegistryId);
        }

        public bool IsAlertRegister()
        {
            return SystemId == Guid.Parse(GlobalVariables.AlertRegistryId);
        }

        public bool IsDokRegistry()
        {
            return SystemId == Guid.Parse(GlobalVariables.DokRegistryId);
        }

        public bool IsInspireStatusRegistry()
        {
            return SystemId == Guid.Parse(GlobalVariables.InspireRegistryId);
        }

        public bool IsGeodatalovStatusRegistry()
        {
            return SystemId == Guid.Parse(GlobalVariables.InspireRegistryId);
        }

        public bool ContainedItemClassIsOrganization()
        {
            return ContainedItemClass == "Organization";
        }

        public bool ContainedItemClassIsCodelistValue()
        {
            return ContainedItemClass == "CodelistValue";
        }

        public bool ContainedItemClassIsRegister()
        {
            return ContainedItemClass == "Register";
        }

        public bool ContainedItemClassIsDocument()
        {
            return ContainedItemClass == "Document";
        }

        public bool ContainedItemClassIsDataset()
        {
            return ContainedItemClass == "Dataset";
        }

        public bool ContainedItemClassIsEpsg()
        {
            return ContainedItemClass == "EPSG";
        }

        public bool ContainedItemClassIsNameSpace()
        {
            return ContainedItemClass == "NameSpace";
        }

        public bool ContainedItemClassIsAlert()
        {
            return ContainedItemClass == "Alert";
        }

        public bool ContainedItemClassIsInspireDataset()
        {
            return ContainedItemClass == "InspireDataset";
        }

        public bool ContainedItemClassIsGeodatalovDataset()
        {
            return ContainedItemClass == "GeodatalovDataset";
        }

        public bool ContainedItemClassIsMareanoDataset()
        {
            return ContainedItemClass == "MareanoDataset";
        }

        public string GetObjectCreateUrl()
        {
            var url = ParentRegister == null
                ? Seoname + "/ny"
                : ParentRegister.seoname + "/" + Owner.seoname + "/" + Seoname + "/ny";

            if (ContainedItemClassIsDocument()) return "/dokument/" + url;
            if (ContainedItemClassIsCodelistValue()) return "/kodeliste/" + url;
            if (ContainedItemClassIsRegister()) return "/subregister/" + url;
            if (ContainedItemClassIsOrganization()) return "/organisasjoner/" + url;
            if (ContainedItemClassIsEpsg()) return "/epsg/" + url;
            if (ContainedItemClassIsNameSpace()) return "/navnerom/" + url;
            if (ContainedItemClassIsAlert()) return "/varsler/" + url + $"?category={HttpContext.Current.Request.QueryString["category"]}";
            if (ContainedItemClassIsInspireDataset()) return "/inspire/" + url;
            if (ContainedItemClassIsGeodatalovDataset()) return "/geodatalov/" + url;
            if (ContainedItemClassIsDataset())
            {
                if (IsDokMunicipal()) return "/dataset/" + Seoname + "/" + MunicipalityCode + "/ny";
                return "/dataset/" + url;
            }
            return "#";
        }

        public string GetEditObjectUrl()
        {
            return ParentRegister == null
                ? "/rediger/" + Seoname
                : "/subregister/" + ParentRegister.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
        }

        public string GetDeleteObjectUrl()
        {
            return ParentRegister == null
                ? "/slett/" + Seoname
                : "/subregister/" + ParentRegister.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
        }


        public string GetEditListUrl()
        {
            return "/dok/kommunalt/" + MunicipalityCode + "/rediger";
        }

        public string StepBackUrl()
        {
            return ParentRegister == null ? "/" : ParentRegister.GetObjectUrl();
        }

        public bool SelectedInspireRegisteryTypeIsInspireReport()
        {
            return SelectedInspireRegisteryType == "inspirereport";
        }

        public bool SelectedInspireRegisteryTypeIsSynchronizations()
        {
            return SelectedInspireRegisteryType == "synchronizations";
        }

        public bool SelectedInspireRegisteryTypeIsReport()
        {
            return SelectedInspireRegisteryType == "report";
        }

        public bool SelectedDokTabIsReport()
        {
            return SelectedDokTab == "report";
        }

        public bool SelectedDokTabIsSuitability()
        {
            return SelectedDokTab == "suitability";
        }

        public bool SelectedInspireRegisteryTypeIsService()
        {
            return SelectedInspireRegisteryType == "service";
        }

        public bool SelectedGeodatalovTabIsReport()
        {
            return SelectedGeodatalovTab == "report";
        }

        public bool SelectedMareanoTabIsReport()
        {
            return SelectedMareanoTab == "report";
        }
    }
}
