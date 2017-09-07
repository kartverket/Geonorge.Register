using System;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models.ViewModels
{
    public class RegisterItemV2VeiwModel
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

        public string StatusId { get; set; }
        public virtual Status Status { get; set; }

        public Guid RegisterId { get; set; }
        public virtual Register Register { get; set; }

        public void UpdateRegisterItem(RegisterItemV2 item)
        {
            Name = item.Name;
            Description = item.Description;
            OwnerId = item.OwnerId;
        }
    }
}