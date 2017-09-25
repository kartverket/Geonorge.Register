using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Register.Models
{
    public abstract class RegisterItemV2
    {
        [Key]
        public Guid SystemId { get; set; }

        [Required]
        [Display(Name = "Navn:")]
        public string Name { get; set; }

        [Required]
        public string Seoname { get; set; }

        [Display(Name = "Beskrivelse:")]
        public string Description { get; set; }

        [ForeignKey("Submitter"), Required]
        public Guid SubmitterId { get; set; }
        [Display(Name = "Innsender:")]
        public virtual Organization Submitter { get; set; }

        [ForeignKey("Owner"), Required]
        public Guid OwnerId { get; set; }
        [Display(Name = "Eier:")]
        public virtual Organization Owner { get; set; }

        [Display(Name = "Dato innsendt:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Dato endret:")]
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

        public DateTime? DateAccepted { get; set; }
        public DateTime? DateNotAccepted { get; set; }
        public DateTime? DateSuperseded { get; set; }
        public DateTime? DateRetired { get; set; }
        public int VersionNumber { get; set; }
        public string VersionName { get; set; }
        [ForeignKey("Versioning")]
        public Guid VersioningId { get; set; }
        public virtual Version Versioning { get; set; }

        protected RegisterItemV2() {

        }

        public string DetailPageUrl()
        {
            return Register.GetObjectUrl() + "/" + Owner.seoname + "/" + Seoname;
        }

    }
}//end namespace Datamodell