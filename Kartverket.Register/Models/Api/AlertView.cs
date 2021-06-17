using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class AlertView
    {
        public string SystemId { get; set; }

        public string Label { get; set; }

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

        /// <summary>
        /// Hva har skjedd? "Endret datainnhold", "Ny tjeneste" etc.
        /// </summary>
        public string AlertType { get; set; }

        /// <summary>
        /// Service type ex OGC:WMS
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Tjenestevarsel, Datasettvarsel, Driftsmelding
        /// </summary>
        public string AlertCategory { get; set; }

        public string Owner { get; set; }

        public string UrlExternal { get; set; }

        /// <summary>
        /// Uuid til tjenesten
        /// </summary>
        public string UuidExternal { get; set; }

        /// <summary>
        /// Beskrivelse av hva varselet gjelder
        /// </summary>
        public string Note { get; set; }

        public List<string> Tags { get; set; }

        public string Department { get; set; }

        public string StationName { get; set; }
        public string StationType { get; set; }

        public string Summary { get; set; }
        public string Link { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image1Thumbnail { get; set; }
        public string Image2Thumbnail { get; set; }


        public DateTime? DateResolved { get; set; }

    }
}