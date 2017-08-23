using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Register.Models
{
    public abstract class RegisterItemV2
    {
        [Key]
        public Guid SystemId { get; set; }

        [Display(Name = "Navn:")]
        public string Name { get; set; }

        public string Seoname { get; set; }

        [Display(Name = "Beskrivelse:")]
        public string Description { get; set; }

        [ForeignKey("Submitter")]
        public Guid SubmitterId { get; set; }
        [Display(Name = "Innsender:")]
        public virtual Organization Submitter { get; set; }

        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }
        [Display(Name = "Eier:")]
        public virtual Organization Owner { get; set; }

        [Display(Name = "Dato innsendt:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Dato endret:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Modified { get; set; }

        [ForeignKey("Status")]
        public string StatusId { get; set; }
        [Display(Name = "Status:")]
        public virtual Status Status { get; set; }

        [ForeignKey("Register")]
        public Guid RegisterId { get; set; }
        [Display(Name = "Register:")]
        public virtual Register Register { get; set; }

    }
}//end namespace Datamodell