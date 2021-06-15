using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class AlertAdd
    {
        /// <summary>
        /// Hva har skjedd? "Endret datainnhold", "Ny tjeneste" etc.
        /// </summary>
        [Required]
        public string AlertType { get; set; }

        /// <summary>
        /// Uuid til tjenesten
        /// </summary>
        [Required]
        public string ServiceUuid { get; set; }

        /// <summary>
        /// Beskrivelse av hva varselet gjelder
        /// </summary>
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Note { get; set; }

        /// <summary>
        /// Valgfri, dersom ikke oppgitt settes til dagens dato. Format: YYYY-MM-DD, f.eks. "2017-03-15".
        /// </summary>
        [Display(Name = "Varslingsdato")]
        public DateTime? AlertDate { get; set; }

        /// <summary>
        /// Valgfri, dersom ikke oppgitt settes til 3 måneder fram i tid. Format: YYYY-MM-DD, f.eks. "2018-07-21".
        /// </summary>
        [Display(Name = "Ikrafttredelsesdato")]
        public DateTime? EffectiveDate { get; set; }

    }
}