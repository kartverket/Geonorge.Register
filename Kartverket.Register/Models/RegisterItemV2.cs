using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Resources;

namespace Kartverket.Register.Models
{
    public abstract class RegisterItemV2
    {
        [Key]
        public Guid SystemId { get; set; }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(Registers))]
        public string Name { get; set; }
        public string NameEnglish { get; set; }

        [Required]
        public string Seoname { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Registers))]
        public string Description { get; set; }
        public string DescriptionEnglish { get; set; }

        [ForeignKey("Submitter"), Required]
        public Guid SubmitterId { get; set; }
        [Display(Name = "Submitter", ResourceType = typeof(Registers))]
        public virtual Organization Submitter { get; set; }

        [ForeignKey("Owner"), Required]
        public Guid OwnerId { get; set; }
        [Display(Name = "Eier:")]
        public virtual Organization Owner { get; set; }

        [Display(Name = "DateSubmitted", ResourceType = typeof(Registers))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Modified", ResourceType = typeof(Registers))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Modified { get; set; }

        [ForeignKey("Status"), Required]
        public string StatusId { get; set; }
        [Display(Name = "Status:")]
        public virtual Status Status { get; set; }

        [ForeignKey("Register"), Required]
        public Guid RegisterId { get; set; }
        [Display(Name = "Register:")]
        public virtual Register Register { get; set; }

        [Display(Name = "DateAccepted", ResourceType = typeof(Registers))]
        public DateTime? DateAccepted { get; set; }

        [Display(Name = "DateNotAccepted", ResourceType = typeof(Registers))]
        public DateTime? DateNotAccepted { get; set; }

        [Display(Name = "DateSuperseded", ResourceType = typeof(Registers))]
        public DateTime? DateSuperseded { get; set; }

        [Display(Name = "DateRetired", ResourceType = typeof(Registers))]
        public DateTime? DateRetired { get; set; }

        [Display(Name = "VersionNumber", ResourceType = typeof(Registers))]
        public int VersionNumber { get; set; }

        [Display(Name = "VersionName", ResourceType = typeof(Registers))]
        public string VersionName { get; set; }

        [ForeignKey("Versioning")]
        public Guid VersioningId { get; set; }
        public virtual Version Versioning { get; set; }

        protected RegisterItemV2() {

        }

        public string DetailPageUrl()
        {
            if (this is GeodatalovDataset geodatalovDataset)
            {
                return geodatalovDataset.DetailPageUrl();
            }

            if (this is InspireDataService inspireDataService)
            {
                return inspireDataService.DetailPageUrl();
            }

            if (this is MareanoDataset mareanoDataset)
            {
                return mareanoDataset.DetailPageUrl();
            }
            return Register.GetObjectUrl() + "/" + Seoname + "/" + SystemId;
        }

    }
}//end namespace Datamodell