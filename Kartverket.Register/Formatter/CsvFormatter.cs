using Kartverket.Register.Models.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Resources;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Formatter
{
    public class CsvFormatter : BufferedMediaTypeFormatter
    {
        private readonly string csv = "text/csv";

        public CsvFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(csv));
            MediaTypeMappings.Add(new UriPathExtensionMapping("csv", csv));
        }

        Func<Type, bool> SupportedTypeCSV = (type) =>
        {
            if (type == typeof(Models.Api.Register) ||
                type == typeof(Registeritem) ||
                type == typeof(IEnumerable<Models.Api.Register>) ||
                type == typeof(List<Models.Api.Register>) ||
                type == typeof(IEnumerable<Registeritem>) ||
                type == typeof(List<Registeritem>))

                return true;
            else
                return false;
        };

        public override bool CanWriteType(System.Type type)
        {
            return SupportedTypeCSV(type);
        }
        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            if (type == typeof(Models.Api.Register) ||
                type == typeof(Registeritem) ||
                type == typeof(IEnumerable<Models.Api.Register>) ||
                type == typeof(List<Models.Api.Register>) ||
                type == typeof(IEnumerable<Registeritem>) ||
                type == typeof(List<Registeritem>))
                BuildCSV(value, writeStream, content.Headers.ContentType.MediaType);
        }

        private void BuildCSV(object models, Stream stream, string contenttype)
        {
            StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
            if (models is Models.Api.Register)
            {
                Models.Api.Register register = (Models.Api.Register)models;
                ConvertRegisters(streamWriter, register);
            }
            if (models is Registeritem)
            {
                Registeritem registerItem = (Registeritem)models;
                streamWriter.WriteLine(RegisterItemHeading(registerItem.itemclass));
                ConvertRegisterItemToCSV(streamWriter, registerItem);
                foreach (Registeritem item in registerItem.versions.OrderBy(v => v.versionNumber))
                {
                    ConvertRegisterItemToCSV(streamWriter, item);
                }
            }
            if (models is List<Models.Api.Register>)
            {
                streamWriter.WriteLine(RegisterHeading());
                List<Models.Api.Register> registers = (List<Models.Api.Register>)models;
                foreach (Models.Api.Register reg in registers.OrderBy(r => r.label))
                {
                    ConvertRegisterToCSV(streamWriter, reg);
                }
            }
            if (models is List<Registeritem>)
            {
                List<Registeritem> registerItems = (List<Registeritem>)models;
                if (registerItems.Count() > 0)
                {
                    streamWriter.WriteLine(RegisterItemHeading(registerItems[0].itemclass));
                    foreach (Registeritem item in registerItems.OrderBy(r => r.label))
                    {
                        ConvertRegisterItemToCSV(streamWriter, item);
                    }
                }
            }
            streamWriter.Close();
        }

        private void ConvertRegisters(StreamWriter streamWriter, Models.Api.Register register)
        {
            if (register.containeditems != null && register.containeditems.Count > 0)
            {
                SetDokMunicipalDataset(register.label);

                if (IsRegisterDokMunicipal)
                    streamWriter.WriteLine(RegisterItemDokMunicipalHeading(register));
                else
                    streamWriter.WriteLine(RegisterItemHeading(register.containedItemClass));

                foreach (Registeritem item in register.containeditems.ToList().OrderBy(i => i.label))
                {
                    if (IsRegisterDokMunicipal)
                        ConvertRegisterItemDokMunicipalToCSV(streamWriter, item);
                    else
                        ConvertRegisterItemToCSV(streamWriter, item);
                }
            }
            else if (register.containedSubRegisters != null)
            {
                streamWriter.WriteLine(RegisterHeading());
                foreach (Models.Api.Register item in register.containedSubRegisters.ToList())
                {
                    ConvertRegisterToCSV(streamWriter, item);
                }
            }
        }

        private static void ConvertRegisterItemToCSV(StreamWriter streamWriter, Registeritem item)
        {
            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            bool isAdmin = role == "nd.metadata_admin";

            item.description = RemoveBreaksFromDescription(item.description);
            string text = null;
            if (item.itemclass == "Document")
            {
                text = item.label + ";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.documentreference + ";" + item.id;
            }
            else if (item.itemclass == "CodelistValue")
            {
                text = item.label + ";" + item.codevalue + ";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.id;
            }
            else if (item.itemclass == "EPSG")
            {
                text = item.label + ";" + item.epsgcode + ";" + item.sosiReferencesystem + ";" + item.verticalReferenceSystem + ";" + item.horizontalReferenceSystem + ";" + item.dimension + ";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.id;
            }
            else if (item.itemclass == "Dataset")
            {
                text = item.theme + ";" + item.label + ";" + item.owner + ";" + item.dokStatus + (isAdmin ? ";" + (item.dokStatusDateAccepted.HasValue ? item.dokStatusDateAccepted.Value.ToString("dd/MM/yyyy") :"" ) : "") + ";" + (item.Kandidatdato.HasValue ? item.Kandidatdato.Value.ToString("dd/MM/yyyy") : "") + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.id + ";" + GetDOKDeliveryStatus(item);
            }
            else if (item.itemclass == "Organization")
            {
                text = $"{item.label};{item.number};{item.MunicipalityCode};{item.GeographicCenterX};{item.GeographicCenterY};{item.BoundingBoxNorth};{item.BoundingBoxWest};{item.BoundingBoxSouth};{item.BoundingBoxEast}";
            }
            else if (item.itemclass == "NameSpace")
            {
                text = item.label + ";" + item.owner + ";" + item.description + ";" + item.serviceUrl + ";";
            }
            else if (item.itemclass == "ServiceAlert")
            {
                text = item.AlertDate.ToString("dd/MM/yyyy") + ";" + item.EffectiveDate.ToString("dd/MM/yyyy") + ";" + item.label + ";" + item.ServiceType + ";" + item.AlertType + ";" + item.owner + ";" + item.Note + ";" + item.MetadataUrl + ";";
            }

            streamWriter.WriteLine(text);
        }

        private static string GetDOKDeliveryStatus(Registeritem item)
        {

            return item.dokDeliveryMetadataStatus + ";" + item.dokDeliveryProductSheetStatus + ";" + item.dokDeliveryPresentationRulesStatus +
                ";" + item.dokDeliveryProductSpecificationStatus + ";" + item.dokDeliveryWmsStatus + ";" + item.dokDeliveryWfsStatus +
                ";" + item.dokDeliverySosiRequirementsStatus + ";" + item.dokDeliveryDistributionStatus + ";" + item.dokDeliveryGmlRequirementsStatus + ";" + item.dokDeliveryAtomFeedStatus;

        }

        private static void ConvertRegisterItemDokMunicipalToCSV(StreamWriter streamWriter, Registeritem item)
        {
            item.description = RemoveBreaksFromDescription(item.description);
            string text = null;
            text = item.theme + ";" + item.label + ";" + item.owner + ";" + item.dokStatus + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.DatasetType + ";" + item.ConfirmedDok + ";" + item.Coverage + ";" + item.NoteMunicipal + ";" + item.MetadataUrl;
            streamWriter.WriteLine(text);
        }

        private static void ConvertRegisterToCSV(StreamWriter streamWriter, Models.Api.Register register)
        {
            register.contentsummary = RemoveBreaksFromDescription(register.contentsummary);
            string text = register.id + ";" + register.label + ";" + register.contentsummary + ";" + register.owner + ";" + register.lastUpdated.ToString("dd/MM/yyyy");
            streamWriter.WriteLine(text);
        }

        private static string RemoveBreaksFromDescription(string description)
        {
            string replaceWith = " ";
            if (!string.IsNullOrWhiteSpace(description))
            {
                description = description.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
            }
            else
            {
                description = "ikke angitt";
            }

            return description;
        }

        private string RegisterItemHeading(string containedItemClass)
        {
            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            bool isAdmin = role == "nd.metadata_admin";

            if (containedItemClass == "Document")
            {
                return "Navn; Eier; Status; Oppdatert; Versjons Id; Beskrivelse; Dokumentreferanse; ID";
            }
            else if (containedItemClass == "CodelistValue")
            {
                return "Navn; Kode; Eier; Status; Oppdatert; Versjons Id; Beskrivelse; ID";
            }
            else if (containedItemClass == "EPSG")
            {
                return "Tittel; EPSG; SOSI; Vertikalt; Horisontalt; Dimensjon; Eier; Status; Oppdatert; Versjons Id; Beskrivelse; ID";
            }
            else if (containedItemClass == "Dataset")
            {
                return "Temagruppe; Navn; Eier; DOK-status; DOK-status dato godkjent; Kandidatdato" + (isAdmin ? "; Oppdatert " : "") +"; Versjons Id; Beskrivelse; ID" + ";" + UI.DOK_Delivery_Metadata + ";"
                    + UI.DOK_Delivery_ProductSheet + ";" + UI.DOK_Delivery_PresentationRules + ";" + UI.DOK_Delivery_ProductSpesification + ";"
                    + UI.DOK_Delivery_Wms + ";" + UI.DOK_Delivery_Wfs + ";" + UI.DOK_Delivery_SosiRequirements + ";"
                    + UI.DOK_Delivery_Distribution + ";" + UI.DOK_Delivery_GmlRequirements + ";" + UI.DOK_Delivery_AtomFeed;
            }
            else if (containedItemClass == "Organization")
            {
                return "Navn; Nummer; Kommunenummer; GeografiskSenterpunktX; GeografiskSenterpunktY; BoundingBoxNord; BoundingBoxVest; BoundingBoxSør; BoundingBoxØst";
            }
            else if (containedItemClass == "NameSpace")
            {
                return "Navn; Etat; Innhold; Tjeneste";
            }
            else if (containedItemClass == "ServiceAlert")
            {
                return "Siste varsel; Ikrafttredelse; Tjeneste; Tjenestetype; Tjenestevarsel; Eier; Varselet gjelder; Url til tjeneste detaljside ";
            }

            return null;
        }

        private string RegisterHeading()
        {
            return "Id ;Navn; Beskrivelse; Eier; Oppdatert";
        }

        private string RegisterItemDokMunicipalHeading(Models.Api.Register register)
        {
            string heading = "Det offentlige kartgrunnlaget - " + register.SelectedDOKMunicipality + ", " + DateTime.Today.ToString("d") + "\r\n";
            heading = heading + "Temagruppe;Tittel;Eier;DOK-status;Oppdatert;Versjons Id;Beskrivelse;Regions-type;Bekreftet;Dekning;Merknad;Url til kartkatalog";
            return heading;
        }

        private void SetDokMunicipalDataset(string registerName)
        {
            IsRegisterDokMunicipal = (registerName == "Det offentlige kartgrunnlaget - Kommunalt");
        }

        bool IsRegisterDokMunicipal = false;
    }
}