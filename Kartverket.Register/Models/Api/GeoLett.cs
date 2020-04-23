using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    /// <summary>
    /// Geolett
    /// </summary>
    public class GeoLett
    {
        /// <summary>
        /// kodeliste - åpen
        /// </summary>
        public string KontekstType { get; set; }
        /// <summary>
        /// Identifikasjonsnummer for tekstene
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Overskrift
        /// </summary>
        public string Tittel { get; set; }
        /// <summary>
        /// En begrunnelse for hvorfor søker gjøres oppmerksom på forholdet
        /// </summary>
        public string ForklarendeTekst { get; set; }
        /// <summary>
        /// Lenker om det er ønskelig å lenke til mer utfyllende informasjon
        /// </summary>
        public List<Lenke> Lenker { get; set; }
        public string Dialogtekst { get; set; }
        /// <summary>
        /// Denne teksten skal fungere som en hjelp til søkeren til å komme videre i prosessen
        /// </summary>
        public string MuligeTiltak { get; set; }
        /// <summary>
        /// Veiledende tekst om ett eller flere av de mulige tiltakene
        /// </summary>
        public string Veiledning { get; set; }       
        public Datasett Datasett { get; set; }
        public Referanse Referanse { get; set; }
        public string TekniskKommentar { get; set; }
        public string AnnenKommentar { get; set; }
        public string Tegn1 { get; set; }
        public string Tegn2 { get; set; }
        public string Tegn3 { get; set; }
        public string Tegn4 { get; set; }
        public string Tegn5 { get; set; }
        public string Tegn6 { get; set; }

    }

    public class Referanse
    {
        public string Tittel { get; set; }
        /// <summary>
        /// Byggesaksforskrifter, lenke
        /// </summary>
        public Lenke Tek17 { get; set; }
        public Lenke AnnenLov { get; set; }
        public Lenke RundskrivFraDep { get; set; }
    }

    public class Datasett
    {
        /// <summary>
        /// Hvilket datasett tekstene er knyttet til
        /// </summary>
        public string Tittel { get; set; }
        /// <summary>
        /// Url til metadata om datasettet
        /// </summary>
        public string UrlMetadata { get; set; }
        /// <summary>
        /// Buffer avstand i meter
        /// </summary>
        public int? BufferAvstand { get; set; }
        /// <summary>
        /// Url til gml-skjema
        /// </summary>
        public string GmlSkjema { get; set; }
        /// <summary>
        /// Objekttype, attributt og datasett skal sammen med type tiltak...
        /// </summary>
        public ObjectType TypeReferanse { get; set; }
    }
    public class ObjectType
    {
        /// <summary>
        /// Flomsone
        /// </summary>
        public string Objekttype { get; set; }
        /// <summary>
        /// Sannsynlighet
        /// </summary>
        public string Attributt { get; set; }
        /// <summary>
        /// 200
        /// </summary>
        public string Kodeverdi { get; set; }

    }
    public class Lenke
    {
        public string Href { get; set; }
        public string Tittel { get; set; }
    }
}