using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartverket.Register.Models
{
    public abstract class RegisterItemNy
    {
        [Key]
        public Guid SystemId { get; set; }

        [Display(Name = "Navn:")]
        public string Name { get; set; }

        public string Seoname { get; set; }

        [Display(Name = "Beskrivelse:")]
        public string Description { get; set; }

        [ForeignKey("Submitter")]
        [Display(Name = "Innsender:")]
        public Guid SubmitterId { get; set; }
        public virtual Organization Submitter { get; set; }

        [ForeignKey("Owner")]
        [Display(Name = "Eier:")]
        public Guid OwnerId { get; set; }
        public virtual Organization Owner { get; set; }

        [Display(Name = "Dato innsendt:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Dato endret:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Modified { get; set; }

        [ForeignKey("Status")]
        [Display(Name = "Status:")]
        public string statusId { get; set; }
        public virtual Status Status { get; set; }

        [Display(Name = "Register:")]
        public virtual Register Register { get; set; }
        [ForeignKey("Register")]
        public Guid RegisterId { get; set; }

    }
}//end namespace Datamodell