using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class GeoLett
    {
        public string KontekstType { get; set; }        
        public string ID { get; set; }
        public string Tittel { get; set; }
        public string ForklarendeTekst { get; set; }
        public Lenke Lenke1 { get; set; }
        public Lenke Lenke2 { get; set; }
        public Lenke Lenke3 { get; set; }
        public Lenke VarselTekst { get; set; }
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
        public Lenke Tek17 { get; set; }
        public Lenke AnnenLov { get; set; }
        public Lenke RundskrivFraDep { get; set; }
    }

    public class Datasett
    {
        public string Tittel { get; set; }
        public string UrlMetadata { get; set; }
        public ObjectType TypeReferanse { get; set; }
    }
    public class ObjectType
    {
        public string Objekttype { get; set; }
        public string Attributt { get; set; }
        public string Kodeverdi { get; set; }

    }
    public class Lenke
    {
        public string Href { get; set; }
        public string Tittel { get; set; }
    }
}