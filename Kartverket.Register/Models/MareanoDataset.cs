using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kartverket.Register.Models.FAIR;

namespace Kartverket.Register.Models
{
    public class MareanoDataset : DatasetV2
    {
        //FAIR delivery status

        //Findable
        [ForeignKey("FindableStatus"), Required, Display(Name = "Findable-status:")]
        public Guid FindableStatusId { get; set; }
        public double FindableStatusPerCent { get; set; }
        public virtual FAIRDelivery FindableStatus { get; set; }

        //Accesible 
        [ForeignKey("AccesibleStatus"), Required, Display(Name = "Accesible-status:")]
        public Guid AccesibleStatusId { get; set; }
        public double AccesibleStatusPerCent { get; set; }
        public virtual FAIRDelivery AccesibleStatus { get; set; }

        //Interoperable 
        [ForeignKey("InteroperableStatus"), Required, Display(Name = "Interoperable-status:")]
        public Guid InteroperableStatusId { get; set; }
        public double InteroperableStatusPerCent { get; set; }
        public virtual FAIRDelivery InteroperableStatus { get; set; }

        //Re-useable 
        [ForeignKey("ReUseableStatus"), Required, Display(Name = "Re-useable-status:")]
        public Guid ReUseableStatusId { get; set; }
        public double ReUseableStatusPerCent { get; set; }
        public virtual FAIRDelivery ReUseableStatus { get; set; }

        //Total FAIR 
        [ForeignKey("FAIRStatus"), Required, Display(Name = "FAIR-status:")]
        public Guid FAIRStatusId { get; set; }
        public double FAIRStatusPerCent { get; set; }
        public virtual FAIRDelivery FAIRStatus { get; set; }


        #region Findable
        [NotMapped]
        public const string Findable_Label = "Findable";

        [NotMapped]
        public const string F1_Label = "F1: Metadata har uuid";

        [Display(Name = "Sjekker at metadata har uuid (gmd:fileIdentifier)")]
        public bool F1_a_Criteria { get; set; } = true;
        [NotMapped]
        public const int F1_a_Weight = 25;

        [NotMapped]
        public const string F2_Label = "F2: Det finnes rike søkemetadata";

        [Display(Name = "Minst tre tematiske nøkkelord skal være lagt inn")]
        public bool F2_a_Criteria { get; set; } = false;
        [NotMapped]
        public const int F2_a_Weight = 10;

        [Display(Name = "Tittel er på maks 100 tegn")]
        public bool F2_b_Criteria { get; set; } = false;
        [NotMapped]
        public const int F2_b_Weight = 5;

        [Display(Name = "Beskrivelse skal være på minst 200 tegn og maks 600 tegn")]
        public bool F2_c_Criteria { get; set; } = false;
        [NotMapped]
        public const int F2_c_Weight = 10;

        [NotMapped]
        public const string F3_Label = "F3: Metadata har datasett-id";

        [Display(Name = "Sjekker at metadata har utfylt dataset-id (MD_Identifier - Code og Namespace)")]
        public bool F3_a_Criteria { get; set; } = false;
        [NotMapped]
        public const int F3_a_Weight = 25;

        [NotMapped]
        public const string F4_Label = "F4: Søkbare";

        [Display(Name = "Metadata skal være søkbare gjennom åpne protokoller og apier")]
        public bool F4_a_Criteria { get; set; } = true;
        [NotMapped]
        public const int F4_a_Weight = 25;

        #endregion

        #region Accesible
        [NotMapped]
        public const string Accesible_Label = "Accesible";

        public const string A1_Label = "A1: Metadata og datasett er tilgjengelig gjennom standard web-protokoller";

        [Display(Name = "Datasettet er tilgjengelig som WFS/WCS")]
        public bool A1_a_Criteria { get; set; } = false;
        [NotMapped]
        public const int A1_a_Weight = 15;

        [Display(Name = "Datasettet er tilgjengelig som WMS")]
        public bool A1_b_Criteria { get; set; } = false;
        [NotMapped]
        public const int A1_b_Weight = 15;

        [Display(Name = "Datasettet er tilgjengelig gjennom Geonorge-api")]
        public bool A1_c_Criteria { get; set; } = false;
        [NotMapped]
        public const int A1_c_Weight = 15;

        [Display(Name = "Datasettet er tilgjengelig somAtom Feed")]
        public bool A1_d_Criteria { get; set; } = false;
        [NotMapped]
        public const int A1_d_Weight = 5;

        [Display(Name = "Protokollene som støttes for tilgang til datasett er åpne, tilgjengelige og lesbare med standard IT-verktøy (Metadata som har nedlastnings-URL støtter dette kravet)")]
        public bool A1_e_Criteria { get; set; } = false;
        [NotMapped]
        public const int A1_e_Weight = 40;

        [Display(Name = "Protokoller støtter autentisering og autorisasjon")]
        public bool A1_f_Criteria { get; set; } = true;
        [NotMapped]
        public const int A1_f_Weight = 10;

        public const string A2_Label = "A2: Metadata er tilgjengelig selv om data ikke lenger er tilgjengelig";

        [Display(Name = "Ikke relevant")]
        public bool A2_a_Criteria { get; set; } = false;
        [NotMapped]
        public const int A2_a_Weight = 0;

        #endregion

        #region Interoperable
        public const string Interoperable_Label = "Interoperable";

        public const string I1_Label = "I1: Metadata er basert på internasjonale, godt dokumenterte og tilgjengelige standarder";

        [Display(Name = "Metadata i Geonorge baseres på ISO19115 og distibueres som OGC:CSW og DCAT-AP-NO og metadata i Geonorge tilfredsstiller dette kravet")]
        public bool I1_a_Criteria { get; set; } = true;
        [NotMapped]
        public const int I1_a_Weight = 20;

        [Display(Name = "Datasett er distribuert på internasjonalt kjente og dokumenterte åpne formater. Sjekk om datasett distribueres som GML")]
        public bool I1_b_Criteria { get; set; } = false;
        [NotMapped]
        public const int I1_b_Weight = 10;

        [Display(Name = "Datasett på GML-format validerer i henhold til UML-modell")]
        public bool I1_c_Criteria { get; set; } = false;
        [NotMapped]
        public const int I1_c_Weight = 20;

        public const string I2_Label = "I2: Begreper, terminologier, ontologier og kodeverk -  godt forvaltet og tilgjengelig";

        [Display(Name = "Tematisk hovedkategori må være utfylt")]
        public bool I2_a_Criteria { get; set; } = false;
        [NotMapped]
        public const int I2_a_Weight = 10;

        [Display(Name = "Nasjonal temakategori er utfylt")]
        public bool I2_b_Criteria { get; set; } = false;
        [NotMapped]
        public const int I2_b_Weight = 10;

        public const string I3_Label = "I3: Relasjoner mellom variabler er beskrevet – forståelig og presis tversgående sammenheng";

        [Display(Name = "Metadata skal refererer til begreper i objektkatalogens UML-modell (Minst ett nøkkelord hvor Thesaurus må være \"SOSI produktspesifikasjon\")")]
        public bool I3_a_Criteria { get; set; } = false;
        [NotMapped]
        public const int I3_a_Weight = 10;

        [Display(Name = "Metadata skal referere til UML-modell i Objektkatalogen (MD_ApplicationSchemaInformation) ")]
        public bool I3_b_Criteria { get; set; } = false;
        [NotMapped]
        public const int I3_b_Weight = 20;

        #endregion

        #region Re-useable
        public const string ReUseable_Label = "Re-useable";

        public const string R1_Label = "R1: Lisens er oppgitt";

        [Display(Name = "Lisens er oppgitt")]
        public bool R1_a_Criteria { get; set; } = false;
        [NotMapped]
        public const int R1_a_Weight = 40;

        public const string R2_Label = "R2: Metadata om kvalitet, dekning, bearbeiding, avledning, beregninger";

        [Display(Name = "Prosesshistorie er oppgitt  og har mer enn 200 tegn")]
        public bool R2_a_Criteria { get; set; } = false;
        [NotMapped]
        public const int R2_a_Weight = 10;

        [Display(Name = "Status er fylt ut med verdi  (gmd:maintenanceAndUpdateFrequency)")]
        public bool R2_b_Criteria { get; set; } = false;
        [NotMapped]
        public const int R2_b_Weight = 5;

        [Display(Name = "Lenke til produktspesifikasjon")]
        public bool R2_c_Criteria { get; set; } = false;
        [NotMapped]
        public const int R2_c_Weight = 10;

        [Display(Name = "Målestokksområde er oppgitt")]
        public bool R2_d_Criteria { get; set; } = false;
        [NotMapped]
        public const int R2_d_Weight = 5;

        [Display(Name = "Dekningskart er oppgitt (kommunevis eller rutenett)")]
        public bool R2_e_Criteria { get; set; } = false;
        [NotMapped]
        public const int R2_e_Weight = 5;

        [Display(Name = "Formål er utfylt")]
        public bool R2_f_Criteria { get; set; } = false;
        [NotMapped]
        public const int R2_f_Weight = 5;

        public const string R3_Label = "R3: Metadata og datasett følger internasjonale standarder";

        [Display(Name = "Metadata følger ISO19115 og distribueres også i henhold til DCAT-profilen")]
        public bool R3_a_Criteria { get; set; } = true;
        [NotMapped]
        public const int R3_a_Weight = 15;

        [Display(Name = "Datasett leveres på internasjonale åpne formater = GML")]
        public bool R3_b_Criteria { get; set; } = false;
        [NotMapped]
        public const int R3_b_Weight = 15;

        #endregion

        //Mareano delivery statuses

        //Metadata
        [ForeignKey("MetadataStatus"), Required, Display(Name = "Metadata:")]
        public Guid MetadataStatusId { get; set; }
        public virtual DatasetDelivery MetadataStatus { get; set; }

        //Produktspesifikasjon
        [ForeignKey("ProductSpesificationStatus"), Required, Display(Name = "Produktspesifikasjon:")]
        public Guid ProductSpesificationStatusId { get; set; }
        public virtual DatasetDelivery ProductSpesificationStatus { get; set; }

        //SOSI-data i hht nasjonal produkstspesifikasjon
        [ForeignKey("SosiDataStatus"), Required, Display(Name = "SOSI-data i hht nasjonal produkstspesifikasjon :")]
        public Guid SosiDataStatusId { get; set; }
        public virtual DatasetDelivery SosiDataStatus { get; set; }

        //GML-data i hht nasjonal produkstspesifikasjon
        [ForeignKey("GmlDataStatus"), Required, Display(Name = "GML-data i hht nasjonal produkstspesifikasjon :")]
        public Guid GmlDataStatusId { get; set; }
        public virtual DatasetDelivery GmlDataStatus { get; set; }

        //View service (Visningstjeneste)
        [ForeignKey("WmsStatus"), Required, Display(Name = "Visningstjeneste WMS/WMTS:")]
        public Guid WmsStatusId { get; set; }
        public virtual DatasetDelivery WmsStatus { get; set; }

        //WFS
        [ForeignKey("WfsStatus"), Required, Display(Name = "Nedlastingstjeneste WFS:")]
        public Guid WfsStatusId { get; set; }
        public virtual DatasetDelivery WfsStatus { get; set; }

        //Atom-feed
        [ForeignKey("AtomFeedStatus"), Required, Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid AtomFeedStatusId { get; set; }
        public virtual DatasetDelivery AtomFeedStatus { get; set; }

        //Nedlastingstjeneste - felles/kombi
        [ForeignKey("CommonStatus"), Required, Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid CommonStatusId { get; set; }
        public virtual DatasetDelivery CommonStatus { get; set; }


        public string DetailPageUrl()
        {
            return Register.GetObjectUrl() + "/" + Seoname + "/" + Uuid;
        }
    }
}