using System;
using System.ComponentModel.DataAnnotations;
using Kartverket.Register.Helpers;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterItemV2ViewModel
    {
        public Guid SystemId { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(Registers))]
        public string Name { get; set; }

        public string Seoname { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Registers))]
        public string Description { get; set; }

        public Guid SubmitterId { get; set; }
        public virtual Organization Submitter { get; set; }

        public Guid OwnerId { get; set; }
        public virtual Organization Owner { get; set; }

        public DateTime DateSubmitted { get; set; }

        public DateTime Modified { get; set; }

        [Display(Name = "DateAccepted", ResourceType = typeof(Registers))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateAccepted { get; set; }

        [Display(Name = "DateNotAccepted", ResourceType = typeof(Registers))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateNotAccepted { get; set; }

        [Display(Name = "DateSuperseded", ResourceType = typeof(Registers))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateSuperseded { get; set; }

        [Display(Name = "DateRetired", ResourceType = typeof(Registers))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateRetired { get; set; }

        public string StatusId { get; set; }
        public virtual Status Status { get; set; }

        public Guid RegisterId { get; set; }
        public virtual Register Register { get; set; }

        [Display(Name = "VersionNumber", ResourceType = typeof(Registers))]
        public int VersionNumber { get; set; }

        [Display(Name = "VersionName", ResourceType = typeof(Registers))]
        public string VersionName { get; set; }

        //TODO gjøre på samme måte som RegisteritemV2
        public RegisterItem RegisterItem { get; set; }

        public bool AccessRegisterItem { get; set; }


        public void UpdateRegisterItem(RegisterItemV2 item)
        {
            var cultureName = CultureHelper.GetCurrentCulture();

            string name = !CultureHelper.IsNorwegian(cultureName) && !string.IsNullOrEmpty(item.NameEnglish)
                          ? item.NameEnglish : item.Name;

            string description = !CultureHelper.IsNorwegian(cultureName) && !string.IsNullOrEmpty(item.DescriptionEnglish)
              ? item.DescriptionEnglish : item.Description;

            SystemId = item.SystemId;
            Name = name;
            Seoname = item.Seoname;
            Submitter = item.Submitter;
            SubmitterId = item.SubmitterId;
            Owner = item.Owner;
            OwnerId = item.OwnerId;
            Description = description;
            DateSubmitted = item.DateSubmitted;
            Modified = item.Modified;
            Status = item.Status;
            StatusId = item.StatusId;
            Register = item.Register;
            RegisterId = item.RegisterId;
            DateAccepted = item.DateAccepted;
            DateNotAccepted = item.DateNotAccepted;
            DateSuperseded = item.DateSuperseded;
            DateRetired = item.DateRetired;
            VersionNumber = item.VersionNumber;
            VersionName = item.VersionName;
        }

        public void UpdateRegisterItem(RegisterItem item)
        {
            SystemId = item.systemId;
            Name = item.name;
            Seoname = item.seoname;
            Submitter = item.submitter;
            SubmitterId = item.submitterId;
            //Owner = item.Owner;
            //OwnerId = item.OwnerId;
            Description = item.description;
            DateSubmitted = item.dateSubmitted;
            Modified = item.modified;
            Status = item.status;
            StatusId = item.statusId;
            Register = item.register;
            RegisterId = item.registerId;
            DateAccepted = item.dateAccepted;
            DateNotAccepted = item.dateNotAccepted;
            DateSuperseded = item.dateSuperseded;
            DateRetired = item.DateRetired;
            VersionNumber = item.versionNumber;
            VersionName = item.versionName;

            if (item is Document document)
            {
                Owner = document.documentowner;
                OwnerId = document.documentownerId;
            }
            if (item is Dataset dataset)
            {
                Owner = dataset.datasetowner;
                OwnerId = dataset.datasetownerId;
            }
        }

        public string NameTranslated(RegisterItem registerItem)
        {
            return registerItem != null ? registerItem.NameTranslated() : Name;
        }

        public string DescriptionTranslated(RegisterItem registerItem)
        {
            return registerItem != null ? registerItem.DescriptionTranslated() : Description;
        }

        public string DetailPageUrl()
        {
            var path = Register.GetObjectUrl() + "/" + Seoname + "/";
            switch (this)
            {
                case InspireDatasetViewModel inspireDatasetViewModel:
                    path += inspireDatasetViewModel.Uuid;
                    break;
                case InspireDataServiceViewModel inspireDataServiceViewModel:
                    path += inspireDataServiceViewModel.Uuid;
                    break;
                case GeodatalovDatasetViewModel geodatalovDatasetViewModel:
                    path = geodatalovDatasetViewModel.Uuid;
                    break;
                case DokDatasetViewModel dokDatasetViewModel:
                    path = dokDatasetViewModel.Uuid;
                    break;
                default:
                    path += SystemId;
                    break;
            }

            return path;
        }

        public string ItemsByOwnerUrl()
        {
            return Register.GetObjectUrl() + "/" + Owner.seoname;
        }

        public string GetDescriptionAsSubstring()
        {
            if (!string.IsNullOrWhiteSpace(Description))
            {
                if (Description.Length < 80)
                {
                    return Description.Substring(0, Description.Length);
                }
                return Description.Substring(0, 80) + "...";
            }
            return "";
        }

        public RegisterItemV2ViewModel(RegisterItem registerItem)
        {
            SystemId = registerItem.systemId;
            Name = NameTranslated(registerItem);
            Seoname = registerItem.seoname;
            Description = DescriptionTranslated(registerItem);
            SubmitterId = registerItem.submitterId;
            Submitter = registerItem.submitter;
            Owner = GetOwner(registerItem);
            DateSubmitted = registerItem.dateSubmitted;
            Modified = registerItem.modified;
            StatusId = registerItem.statusId;
            Status = registerItem.status;
            RegisterId = registerItem.registerId;
            Register = registerItem.register;
            RegisterItem = registerItem;
            DateAccepted = registerItem.dateAccepted;
            DateNotAccepted = registerItem.dateNotAccepted;
            DateSuperseded = registerItem.dateSuperseded;
            DateRetired = registerItem.DateRetired;
            VersionNumber = registerItem.versionNumber;
            VersionName = registerItem.versionName;
            RegisterItem = registerItem;
        }

        public RegisterItemV2ViewModel()
        {
        }

        private Organization GetOwner(RegisterItem registerItem)
        {
            return registerItem.GetOwner();
        }

        public virtual string GetObjectEditUrl()
        {
            switch (this)
            {
                case InspireDatasetViewModel inspireDatasetViewModel:
                    return inspireDatasetViewModel.GetInspireDatasetEditUrl();
                case InspireDataServiceViewModel inspireDataServiceViewModel:
                    return inspireDataServiceViewModel.GetInspireDataServiceEditUrl();
                case GeodatalovDatasetViewModel geodatalovDatasetViewModel:
                    return geodatalovDatasetViewModel.GetGeodatalovDatasetEditUrl();
                case DocumentViewModel documentViewModel:
                    return documentViewModel.EditVersionOfDocumentUrl();
                case DokDatasetViewModel dokDatasetViewModel:
                    return dokDatasetViewModel.GetDatasetEditUrl();

            }
            switch (RegisterItem)
            {
                case Document _:
                    var document = (Document)RegisterItem;
                    return document.GetDocumentEditUrl();
                case Dataset _:
                    var dataset = (Dataset)RegisterItem;
                    return dataset.GetDatasetEditUrl();
                case EPSG _:
                    var epsg = (EPSG)RegisterItem;
                    return epsg.GetEPSGEditUrl();
                case CodelistValue _:
                    var codelistValue = (CodelistValue)RegisterItem;
                    return codelistValue.GetCodelistValueEditUrl();
                case NameSpace _:
                    var nameSpace = (NameSpace)RegisterItem;
                    return nameSpace.GetNameSpaceEditUrl();
                case Organization _:
                    var organization = (Organization)RegisterItem;
                    return organization.GetOrganizationEditUrl();
                case ServiceAlert _:
                    var serviceAlert = (ServiceAlert)RegisterItem;
                    return serviceAlert.GetServiceAlertEditUrl();
            }
            return "#";
        }

        public string GetObjectDeleteUrl()
        {
            switch (this)
            {
                case InspireDatasetViewModel inspireDatasetViewModel:
                    return inspireDatasetViewModel.GetInspireDatasetDeleteUrl();
                case InspireDataServiceViewModel inspireDataServiceViewModel:
                    return inspireDataServiceViewModel.GetInspireDataServiceDeleteUrl();
                case GeodatalovDatasetViewModel geodatalovDatasetViewModel:
                    return geodatalovDatasetViewModel.GetGeodatalovDatasetDeleteUrl();
                case DocumentViewModel documentViewModel:
                    return documentViewModel.DeleteVersionOfDocumentUrl();
                case DokDatasetViewModel dokDatasetViewModel:
                    return dokDatasetViewModel.GetDatasetDeleteUrl();
            }
            switch (RegisterItem)
            {
                case Document _:
                    var document = (Document)RegisterItem;
                    return document.GetDocumentDeleteUrl();
                case Dataset _:
                    var dataset = (Dataset)RegisterItem;
                    return dataset.GetDatasetDeleteUrl();
                case EPSG _:
                    var epsg = (EPSG)RegisterItem;
                    return epsg.GetEPSGDeleteUrl();
                case CodelistValue _:
                    var codelistValue = (CodelistValue)RegisterItem;
                    return codelistValue.GetCodelistValueDeleteUrl();
                case NameSpace _:
                    var nameSpace = (NameSpace)RegisterItem;
                    return nameSpace.GetNameSpaceDeleteUrl();
                case Organization _:
                    var organization = (Organization)RegisterItem;
                    return organization.GetOrganizationDeleteUrl();
                case ServiceAlert _:
                    var serviceAlert = (ServiceAlert)RegisterItem;
                    return serviceAlert.GetServiceAlertDeleteUrl();
            }
            return "#";
        }

        public string EditVersionRegisterItemUrl()
        {
            switch (this)
            {
                case DocumentViewModel _:
                    var document = (DocumentViewModel)this;
                    return document.EditVersionOfDocumentUrl();
            }
            return "";
        }

        public string DeleteVersionRegisterItemUrl()
        {
            switch (this)
            {
                case DocumentViewModel _:
                    var document = (DocumentViewModel)this;
                    return document.DeleteVersionOfDocumentUrl();
            }
            return "";
        }

        public string DeleteVersionOfDocumentUrl()
        {
            return "/dokument/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett?vnr=" + VersionNumber;
        }

        public bool RedirectToNewPath(string requestPath)
        {
            var newPath = DetailPageUrl();
            return requestPath != newPath;


        }
    }
}