using System;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterItemV2ViewModel
    {
        public Guid SystemId { get; set; }

        [Required]
        [Display(Name = "Navn:")]
        public string Name { get; set; }

        public string Seoname { get; set; }

        [Display(Name = "Beskrivelse:")]
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

        public void UpdateRegisterItem(RegisterItemV2 item)
        {
            Name = item.Name;
            Seoname = item.Seoname;
            Submitter = item.Submitter;
            SubmitterId = item.SubmitterId;
            Owner = item.Owner;
            OwnerId = item.OwnerId;
            Description = item.Description;
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



        public string DetailPageUrl()
        {
            return Register.GetObjectUrl() + "/" + Owner.seoname + "/" + Seoname;
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
            Name = registerItem.name;
            Seoname = registerItem.seoname;
            Description = registerItem.description;
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
            if (this is InspireDatasetViewModel inspireDatasetViewModel)
            {
                return inspireDatasetViewModel.GetInspireDatasetEditUrl();
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
            if (this is InspireDatasetViewModel inspireDatasetViewModel)
            {
                return inspireDatasetViewModel.GetInspireDatasetEditUrl();
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
    }
}