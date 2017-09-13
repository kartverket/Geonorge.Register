using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterItemV2ViewModel
    {
        private ICollection<RegisterItemV2> registerItems;

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

        public string StatusId { get; set; }
        public virtual Status Status { get; set; }

        public Guid RegisterId { get; set; }
        public virtual Register Register { get; set; }

        public void UpdateRegisterItem(RegisterItemV2 item)
        {
            Name = item.Name;
            Seoname = item.Seoname;
            Submitter = item.Submitter;
            SubmitterId = item.SubmitterId;
            Owner = item.Owner;
            OwnerId = item.OwnerId;
            Description = item.Description;
            OwnerId = item.OwnerId;
            Register = item.Register;
            RegisterId = item.RegisterId;

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

     
        public RegisterItemV2ViewModel(RegisterItemV2 registerItem)
        {
            Name = registerItem.Name;
            Seoname = registerItem.Seoname;
            Description = registerItem.Description;
            SubmitterId = registerItem.SubmitterId;
            OwnerId = registerItem.OwnerId;
            DateSubmitted = registerItem.DateSubmitted;
            Modified = registerItem.Modified;
            StatusId = registerItem.StatusId;
            RegisterId = registerItem.RegisterId;

        }

        public RegisterItemV2ViewModel()
        {
        }
    }
}