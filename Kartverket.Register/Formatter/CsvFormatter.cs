using Kartverket.Register.Models.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Geonorge.AuthLib.Common;
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
                type == typeof(InspireRegistryStatusReport) ||
                type == typeof(List<Models.Api.Register>) ||
                type == typeof(IEnumerable<Registeritem>) ||
                type == typeof(List<InspireRegistryStatusReport>) ||
                type == typeof(DokStatusReport) ||
                type == typeof(MareanoDatasetStatusReport) ||
                type == typeof(List<MareanoDatasetStatusReport>) ||
                type == typeof(GeodatalovDatasetStatusReport) ||
                type == typeof(List<GeodatalovDatasetStatusReport>) ||
                type == typeof(List<DokStatusReport>) ||
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
                type == typeof(InspireRegistryStatusReport) ||
                type == typeof(DokStatusReport) ||
                type == typeof(List<InspireRegistryStatusReport>) ||
                type == typeof(List<DokStatusReport>) ||
                type == typeof(GeodatalovDatasetStatusReport) ||
                type == typeof(List<GeodatalovDatasetStatusReport>) ||
                type == typeof(MareanoDatasetStatusReport) ||
                type == typeof(List<MareanoDatasetStatusReport>) ||
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

            if (models is DokStatusReport dokStatusReport)
            {
                streamWriter.WriteLine(SingelStatusReportHeading());
                WriteDokStatusesInTable(streamWriter, dokStatusReport);
            }

            if (models is List<DokStatusReport> dokStatusReports)
            {
                streamWriter.WriteLine(DokStatusReportHeading());
                streamWriter.WriteLine(StatusReportHeadingStatusValues());

                foreach (var report in dokStatusReports)
                {
                    streamWriter.WriteLine(WriteDokStatusReport(report));
                }
            }

            if (models is InspireRegistryStatusReport inspireRegisteryStatusReport)
            {
                WriteInspireStatusesInTable(streamWriter, inspireRegisteryStatusReport);
            }

            if (models is List<InspireRegistryStatusReport> inspireStatusReports)
            {
                WriteInspireStatusesInList(streamWriter, inspireStatusReports);
            }

            if (models is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
            {
                streamWriter.WriteLine(SingelStatusReportHeading());
                WriteGeodatalovStatusesInTable(streamWriter, geodatalovDatasetStatusReport);
            }

            if (models is MareanoDatasetStatusReport mareanoDatasetStatusReport)
            {
                streamWriter.WriteLine(SingelStatusReportHeading("", "mareano"));
                WriteMareanoStatusesInTable(streamWriter, mareanoDatasetStatusReport);
            }

            if (models is List<GeodatalovDatasetStatusReport> geodatalovDatasetStatusReports)
            {
                streamWriter.WriteLine(GeodatalovStatusReportHeading());
                streamWriter.WriteLine(StatusReportHeadingStatusValuesGeodatalov());

                foreach (var report in geodatalovDatasetStatusReports)
                {
                    streamWriter.WriteLine(WriteGeodatalovStatusReport(report));
                }
            }

            if (models is List<MareanoDatasetStatusReport> mareanoDatasetStatusReports)
            {
                streamWriter.WriteLine(MareanoStatusReportHeading());
                streamWriter.WriteLine(StatusReportHeadingStatusValuesMareano());

                foreach (var report in mareanoDatasetStatusReports)
                {
                    streamWriter.WriteLine(WriteMareanoStatusReport(report));
                }
            }

            streamWriter.Close();
        }

        private string StatusReportHeadingStatusValuesMareano()
        {
            return ";" +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano() +
               HeadingStatusesMareano();
        }

        private string HeadingStatusesMareano()
        {
            return DataSet.DOK_Delivery_Status_Good + ";" + MareanoDataSet.Delivery_Status_Useable + ";" + MareanoDataSet.Delivery_Status_Deficient + ";" + DataSet.DOK_Delivery_Status_NotSet + ";" + MareanoDataSet.Delivery_Status_Satisfactory + ";";
        }

        private string WriteMareanoStatusReport(MareanoDatasetStatusReport report)
        {
            return report.Date + ";" +
                   report.NumberOfItemsWithMetadata.Good + ";" +
                   report.NumberOfItemsWithMetadata.Useable + ";" +
                   report.NumberOfItemsWithMetadata.Deficient + ";" +
                   report.NumberOfItemsWithMetadata.Notset + ";" +
                   report.NumberOfItemsWithMetadata.Satisfactory + ";" +

                   report.NumberOfItemsWithProductSpecification.Good + ";" +
                   report.NumberOfItemsWithProductSpecification.Useable + ";" +
                   report.NumberOfItemsWithProductSpecification.Deficient + ";" +
                   report.NumberOfItemsWithProductSpecification.Notset + ";" +
                   report.NumberOfItemsWithProductSpecification.Satisfactory + ";" +

                   report.NumberOfItemsWithProductSheet.Good + ";" +
                   report.NumberOfItemsWithProductSheet.Useable + ";" +
                   report.NumberOfItemsWithProductSheet.Deficient + ";" +
                   report.NumberOfItemsWithProductSheet.Notset + ";" +
                   report.NumberOfItemsWithProductSheet.Satisfactory + ";" +

                   report.NumberOfItemsWithPresentationRules.Good + ";" +
                   report.NumberOfItemsWithPresentationRules.Useable + ";" +
                   report.NumberOfItemsWithPresentationRules.Deficient + ";" +
                   report.NumberOfItemsWithPresentationRules.Notset + ";" +
                   report.NumberOfItemsWithPresentationRules.Satisfactory + ";" +

                   report.NumberOfItemsWithSosiRequirements.Good + ";" +
                   report.NumberOfItemsWithSosiRequirements.Useable + ";" +
                   report.NumberOfItemsWithSosiRequirements.Deficient + ";" +
                   report.NumberOfItemsWithSosiRequirements.Notset + ";" +
                   report.NumberOfItemsWithSosiRequirements.Satisfactory + ";" +

                   report.NumberOfItemsWithGmlRequirements.Good + ";" +
                   report.NumberOfItemsWithGmlRequirements.Useable + ";" +
                   report.NumberOfItemsWithGmlRequirements.Deficient + ";" +
                   report.NumberOfItemsWithGmlRequirements.Notset + ";" +
                   report.NumberOfItemsWithGmlRequirements.Satisfactory + ";" +

                   report.NumberOfItemsWithWms.Good + ";" +
                   report.NumberOfItemsWithWms.Useable + ";" +
                   report.NumberOfItemsWithWms.Deficient + ";" +
                   report.NumberOfItemsWithWms.Notset + ";" +
                   report.NumberOfItemsWithWms.Satisfactory + ";" +

                   report.NumberOfItemsWithWfs.Good + ";" +
                   report.NumberOfItemsWithWfs.Useable + ";" +
                   report.NumberOfItemsWithWfs.Deficient + ";" +
                   report.NumberOfItemsWithWfs.Notset + ";" +
                   report.NumberOfItemsWithWfs.Satisfactory + ";" +

                   report.NumberOfItemsWithAtomFeed.Good + ";" +
                   report.NumberOfItemsWithAtomFeed.Useable + ";" +
                   report.NumberOfItemsWithAtomFeed.Deficient + ";" +
                   report.NumberOfItemsWithAtomFeed.Notset + ";" +
                   report.NumberOfItemsWithAtomFeed.Satisfactory + ";" +

                   report.NumberOfItemsWithCommon.Good + ";" +
                   report.NumberOfItemsWithCommon.Useable + ";" +
                   report.NumberOfItemsWithCommon.Deficient + ";" +
                   report.NumberOfItemsWithCommon.Notset + ";" +
                   report.NumberOfItemsWithCommon.Satisfactory + ";" +

                   report.NumberOfItemsWithFindable.Good + ";" +
                   report.NumberOfItemsWithFindable.Useable + ";" +
                   report.NumberOfItemsWithFindable.Deficient + ";" +
                   report.NumberOfItemsWithFindable.Notset + ";" +
                   report.NumberOfItemsWithFindable.Satisfactory + ";" +

                   report.NumberOfItemsWithAccesible.Good + ";" +
                   report.NumberOfItemsWithAccesible.Useable + ";" +
                   report.NumberOfItemsWithAccesible.Deficient + ";" +
                   report.NumberOfItemsWithAccesible.Notset + ";" +
                   report.NumberOfItemsWithAccesible.Satisfactory + ";" +

                   report.NumberOfItemsWithInteroperable.Good + ";" +
                   report.NumberOfItemsWithInteroperable.Useable + ";" +
                   report.NumberOfItemsWithInteroperable.Deficient + ";" +
                   report.NumberOfItemsWithInteroperable.Notset + ";" +
                   report.NumberOfItemsWithInteroperable.Satisfactory + ";" +

                   report.NumberOfItemsWithReUseable.Good + ";" +
                   report.NumberOfItemsWithReUseable.Useable + ";" +
                   report.NumberOfItemsWithReUseable.Deficient + ";" +
                   report.NumberOfItemsWithReUseable.Notset + ";" +
                   report.NumberOfItemsWithReUseable.Satisfactory + ";";

        }

        private string MareanoStatusReportHeading()
        {
            return Registers.Date + ";" +
               DataSet.DOK_Delivery_Metadata + ";" + ";" + ";" + ";" + ";" +
               DataSet.DOK_Delivery_ProductSpesification + ";" + ";" + ";" + ";" + ";" +
               DataSet.DOK_Delivery_ProductSheet + ";" + ";" + ";" + ";" + ";" +
               DataSet.DOK_Delivery_PresentationRules + ";" + ";" + ";" + ";" + ";" +
               DataSet.DOK_Delivery_SosiRequirements + ";" + ";" + ";" + ";" + ";" +
               DataSet.DOK_Delivery_GmlRequirements + ";" + ";" + ";" + ";" + ";" +
               DataSet.DOK_Delivery_Wms + ";" + ";" + ";" + ";" + ";" +
               DataSet.DOK_Delivery_Wfs + ";" + ";" + ";" + ";" + ";" +
               DataSet.DOK_Delivery_AtomFeed + ";" + ";" + ";" + ";" + ";" +
               GeodatalovDataSet.Common + ";" + ";" + ";" + ";" + ";" +
               MareanoDataSet.Findable_Label + ";" + ";" + ";" + ";" + ";" +
               MareanoDataSet.Accesible_Label + ";" + ";" + ";" + ";" + ";" +
               MareanoDataSet.Interoperable_Label + ";" + ";" + ";" + ";" +";" +
               MareanoDataSet.ReUseable_Label + ";" + ";" + ";" + ";" + ";";
        }

        private void WriteMareanoStatusesInTable(StreamWriter streamWriter, MareanoDatasetStatusReport mareanoDatasetStatusReport)
        {
            streamWriter.WriteLine(StatusByType(MareanoDataSet.Findable_Label,
                mareanoDatasetStatusReport.NumberOfItemsWithFindable.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithFindable.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithFindable.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithFindable.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithFindable.Satisfactory
                ));

            streamWriter.WriteLine(StatusByType(MareanoDataSet.Accesible_Label,
                mareanoDatasetStatusReport.NumberOfItemsWithAccesible.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithAccesible.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithAccesible.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithAccesible.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithAccesible.Satisfactory
                ));

            streamWriter.WriteLine(StatusByType(MareanoDataSet.Interoperable_Label,
                mareanoDatasetStatusReport.NumberOfItemsWithInteroperable.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithInteroperable.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithInteroperable.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithInteroperable.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithInteroperable.Satisfactory
                ));

            streamWriter.WriteLine(StatusByType(MareanoDataSet.ReUseable_Label,
                mareanoDatasetStatusReport.NumberOfItemsWithReUseable.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithReUseable.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithReUseable.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithReUseable.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithReUseable.Satisfactory
                ));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Metadata,
                mareanoDatasetStatusReport.NumberOfItemsWithMetadata.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithMetadata.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithMetadata.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithMetadata.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithMetadata.Satisfactory
                ));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_ProductSpesification,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSpecification.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSpecification.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSpecification.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSpecification.Notset, 
                mareanoDatasetStatusReport.NumberOfItemsWithMetadata.Satisfactory));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_ProductSheet,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSheet.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSheet.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSheet.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSheet.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithProductSheet.Satisfactory));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_PresentationRules,
                mareanoDatasetStatusReport.NumberOfItemsWithPresentationRules.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithPresentationRules.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithPresentationRules.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithPresentationRules.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithPresentationRules.Satisfactory));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_GmlRequirements,
                mareanoDatasetStatusReport.NumberOfItemsWithGmlRequirements.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithGmlRequirements.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithGmlRequirements.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithGmlRequirements.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithGmlRequirements.Satisfactory));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_SosiRequirements,
                mareanoDatasetStatusReport.NumberOfItemsWithSosiRequirements.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithSosiRequirements.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithSosiRequirements.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithSosiRequirements.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithSosiRequirements.Satisfactory));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Wms,
                mareanoDatasetStatusReport.NumberOfItemsWithWms.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithWms.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithWms.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithWms.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithWms.Satisfactory));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Wfs,
                mareanoDatasetStatusReport.NumberOfItemsWithWfs.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithWfs.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithWfs.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithWfs.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithWfs.Satisfactory));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_AtomFeed,
                mareanoDatasetStatusReport.NumberOfItemsWithAtomFeed.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithAtomFeed.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithAtomFeed.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithAtomFeed.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithAtomFeed.Satisfactory));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Distribution,
                mareanoDatasetStatusReport.NumberOfItemsWithCommon.Good,
                mareanoDatasetStatusReport.NumberOfItemsWithCommon.Useable,
                mareanoDatasetStatusReport.NumberOfItemsWithCommon.Deficient,
                mareanoDatasetStatusReport.NumberOfItemsWithCommon.Notset,
                mareanoDatasetStatusReport.NumberOfItemsWithCommon.Satisfactory));

            streamWriter.WriteLine(SingelStatusReportHeadingOK());
        }

        private void WriteInspireStatusesInList(StreamWriter streamWriter, List<InspireRegistryStatusReport> inspireRegisteryStatusReports)
        {
            WriteInspireDatasetStatusToList(streamWriter, inspireRegisteryStatusReports);
            streamWriter.WriteLine();
            WriteInspireServiceStatusToList(streamWriter, inspireRegisteryStatusReports);
        }

        private void WriteInspireServiceStatusToList(StreamWriter streamWriter, List<InspireRegistryStatusReport> inspireRegisteryStatusReports)
        {
            streamWriter.WriteLine(InspireDataSet.Service);
            streamWriter.WriteLine(InspireServiceReportHeading());
            streamWriter.WriteLine(InspireServiceStatusReportHeadingStatusValues());

            foreach (var report in inspireRegisteryStatusReports)
            {
                streamWriter.WriteLine(WriteInspireServiceStatusReport(report));
            }
        }

        private void WriteInspireDatasetStatusToList(StreamWriter streamWriter, List<InspireRegistryStatusReport> inspireRegisteryStatusReports)
        {
            streamWriter.WriteLine(DataSet.Dataset);
            streamWriter.WriteLine(InspireDatasetReportHeading());
            streamWriter.WriteLine(StatusReportHeadingStatusValues());

            foreach (var report in inspireRegisteryStatusReports)
            {
                streamWriter.WriteLine(WriteInspireDatasetStatusReport(report));
            }
        }

        private string InspireServiceReportHeading()
        {
            return Registers.Date + ";" +
                   InspireDataSet.Metadata + ";" + ";" + ";" + ";" +
                   InspireDataSet.MetadataInSearchService + ";" + ";" + ";" + ";" +
                   InspireDataSet.ServiceStatus + ";" + ";" + ";" + ";" +
                   InspireDataSet.Sds + ";" + ";" +
                   InspireDataSet.NetworkService + ";" + ";";
        }

        private string WriteInspireDatasetStatusReport(InspireRegistryStatusReport inspreStatusReport)
        {
            return inspreStatusReport.InspireDatasetStatusReport.Date + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadata.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadata.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadata.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadata.Notset + ";" +

                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadataService.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadataService.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadataService.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadataService.Notset + ";" +

                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithDistribution.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithDistribution.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithDistribution.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithDistribution.Notset + ";" +

                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWms.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWms.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWms.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWms.Notset + ";" +

                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfs.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfs.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfs.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfs.Notset + ";" +

                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithAtomFeed.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithAtomFeed.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithAtomFeed.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithAtomFeed.Notset + ";" +

                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfsOrAtom.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfsOrAtom.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfsOrAtom.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfsOrAtom.Notset + ";" +

                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithHarmonizedData.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithHarmonizedData.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithHarmonizedData.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithHarmonizedData.Notset + ";" +

                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithSpatialDataService.Good + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithSpatialDataService.Useable + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithSpatialDataService.Deficient + ";" +
                   inspreStatusReport.InspireDatasetStatusReport.NumberOfItemsWithSpatialDataService.Notset;

        }

        private string WriteInspireServiceStatusReport(InspireRegistryStatusReport inspreStatusReport)
        {
            return inspreStatusReport.InspireServiceStatusReport.Date + ";" +

                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadata.Good + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadata.Useable + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadata.Deficient + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadata.Notset + ";" +

                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadataInSearchService.Good + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadataInSearchService.Useable + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadataInSearchService.Deficient + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadataInSearchService.Notset + ";" +

                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithServiceStatus.Good + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithServiceStatus.Useable + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithServiceStatus.Deficient + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithServiceStatus.Notset + ";" +

                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithSds + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithoutSds + ";" +

                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithNetworkService + ";" +
                   inspreStatusReport.InspireServiceStatusReport.NumberOfItemsWithoutNetworkService + ";";
        }

        private void WriteDokStatusesInTable(StreamWriter streamWriter, DokStatusReport dokStatusReport)
        {
            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Metadata,
                dokStatusReport.NumberOfItemsWithMetadata.Good,
                dokStatusReport.NumberOfItemsWithMetadata.Useable,
                dokStatusReport.NumberOfItemsWithMetadata.Deficient,
                dokStatusReport.NumberOfItemsWithMetadata.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_ProductSheet,
                dokStatusReport.NumberOfItemsWithProductsheet.Good,
                dokStatusReport.NumberOfItemsWithProductsheet.Useable,
                dokStatusReport.NumberOfItemsWithProductsheet.Deficient,
                dokStatusReport.NumberOfItemsWithProductsheet.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_PresentationRules,
                dokStatusReport.NumberOfItemsWithPresentationRules.Good,
                dokStatusReport.NumberOfItemsWithPresentationRules.Useable,
                dokStatusReport.NumberOfItemsWithPresentationRules.Deficient,
                dokStatusReport.NumberOfItemsWithPresentationRules.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_ProductSpesification,
                dokStatusReport.NumberOfItemsWithProductSpecification.Good,
                dokStatusReport.NumberOfItemsWithProductSpecification.Useable,
                dokStatusReport.NumberOfItemsWithProductSpecification.Deficient,
                dokStatusReport.NumberOfItemsWithProductSpecification.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Wms,
                dokStatusReport.NumberOfItemsWithWms.Good,
                dokStatusReport.NumberOfItemsWithWms.Useable,
                dokStatusReport.NumberOfItemsWithWms.Deficient,
                dokStatusReport.NumberOfItemsWithWms.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Wfs,
                dokStatusReport.NumberOfItemsWithWfs.Good,
                dokStatusReport.NumberOfItemsWithWfs.Useable,
                dokStatusReport.NumberOfItemsWithWfs.Deficient,
                dokStatusReport.NumberOfItemsWithWfs.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_GmlRequirements,
                dokStatusReport.NumberOfItemsWithGmlRequirements.Good,
                dokStatusReport.NumberOfItemsWithGmlRequirements.Useable,
                dokStatusReport.NumberOfItemsWithGmlRequirements.Deficient,
                dokStatusReport.NumberOfItemsWithGmlRequirements.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_SosiRequirements,
                dokStatusReport.NumberOfItemsWithSosiRequirements.Good,
                dokStatusReport.NumberOfItemsWithSosiRequirements.Useable,
                dokStatusReport.NumberOfItemsWithSosiRequirements.Deficient,
                dokStatusReport.NumberOfItemsWithSosiRequirements.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_AtomFeed,
                dokStatusReport.NumberOfItemsWithAtomFeed.Good,
                dokStatusReport.NumberOfItemsWithAtomFeed.Useable,
                dokStatusReport.NumberOfItemsWithAtomFeed.Deficient,
                dokStatusReport.NumberOfItemsWithAtomFeed.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Distribution,
                dokStatusReport.NumberOfItemsWithDistribution.Good,
                dokStatusReport.NumberOfItemsWithDistribution.Useable,
                dokStatusReport.NumberOfItemsWithDistribution.Deficient,
                dokStatusReport.NumberOfItemsWithDistribution.Notset));
        }

        private void WriteGeodatalovStatusesInTable(StreamWriter streamWriter, GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
        {
            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Metadata,
                geodatalovDatasetStatusReport.NumberOfItemsWithMetadata.Good,
                geodatalovDatasetStatusReport.NumberOfItemsWithMetadata.Useable,
                geodatalovDatasetStatusReport.NumberOfItemsWithMetadata.Deficient,
                geodatalovDatasetStatusReport.NumberOfItemsWithMetadata.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_ProductSpesification,
                geodatalovDatasetStatusReport.NumberOfItemsWithProductSpecification.Good,
                geodatalovDatasetStatusReport.NumberOfItemsWithProductSpecification.Useable,
                geodatalovDatasetStatusReport.NumberOfItemsWithProductSpecification.Deficient,
                geodatalovDatasetStatusReport.NumberOfItemsWithProductSpecification.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_GmlRequirements,
                geodatalovDatasetStatusReport.NumberOfItemsWithGmlRequirements.Good,
                geodatalovDatasetStatusReport.NumberOfItemsWithGmlRequirements.Useable,
                geodatalovDatasetStatusReport.NumberOfItemsWithGmlRequirements.Deficient,
                geodatalovDatasetStatusReport.NumberOfItemsWithGmlRequirements.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_SosiRequirements,
                geodatalovDatasetStatusReport.NumberOfItemsWithSosiRequirements.Good,
                geodatalovDatasetStatusReport.NumberOfItemsWithSosiRequirements.Useable,
                geodatalovDatasetStatusReport.NumberOfItemsWithSosiRequirements.Deficient,
                geodatalovDatasetStatusReport.NumberOfItemsWithSosiRequirements.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Wms,
                geodatalovDatasetStatusReport.NumberOfItemsWithWms.Good,
                geodatalovDatasetStatusReport.NumberOfItemsWithWms.Useable,
                geodatalovDatasetStatusReport.NumberOfItemsWithWms.Deficient,
                geodatalovDatasetStatusReport.NumberOfItemsWithWms.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Wfs,
                geodatalovDatasetStatusReport.NumberOfItemsWithWfs.Good,
                geodatalovDatasetStatusReport.NumberOfItemsWithWfs.Useable,
                geodatalovDatasetStatusReport.NumberOfItemsWithWfs.Deficient,
                geodatalovDatasetStatusReport.NumberOfItemsWithWfs.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_AtomFeed,
                geodatalovDatasetStatusReport.NumberOfItemsWithAtomFeed.Good,
                geodatalovDatasetStatusReport.NumberOfItemsWithAtomFeed.Useable,
                geodatalovDatasetStatusReport.NumberOfItemsWithAtomFeed.Deficient,
                geodatalovDatasetStatusReport.NumberOfItemsWithAtomFeed.Notset));

            streamWriter.WriteLine(StatusByType(DataSet.DOK_Delivery_Distribution,
                geodatalovDatasetStatusReport.NumberOfItemsWithCommon.Good,
                geodatalovDatasetStatusReport.NumberOfItemsWithCommon.Useable,
                geodatalovDatasetStatusReport.NumberOfItemsWithCommon.Deficient,
                geodatalovDatasetStatusReport.NumberOfItemsWithCommon.Notset));

            streamWriter.WriteLine(SingelStatusReportHeadingOK());

            streamWriter.WriteLine(StatusOk(GeodatalovDataSet.InspireTheme,
                geodatalovDatasetStatusReport.NumberOfItemsWithInspireTheme,
                geodatalovDatasetStatusReport.NumberOfItemsWithoutInspireTheme
            ));

            streamWriter.WriteLine(StatusOk(GeodatalovDataSet.Dok,
                geodatalovDatasetStatusReport.NumberOfItemsWithDok,
                geodatalovDatasetStatusReport.NumberOfItemsWithoutDok
            ));

            streamWriter.WriteLine(StatusOk(GeodatalovDataSet.NationalDataset,
                geodatalovDatasetStatusReport.NumberOfItemsWithNationalDatasets,
                geodatalovDatasetStatusReport.NumberOfItemsWithoutNationalDatasets
            ));

            streamWriter.WriteLine(StatusOk(GeodatalovDataSet.Plan,
                geodatalovDatasetStatusReport.NumberOfItemsWithPlan,
                geodatalovDatasetStatusReport.NumberOfItemsWithoutPlan
            ));

            streamWriter.WriteLine(StatusOk(GeodatalovDataSet.Geodatalov,
                geodatalovDatasetStatusReport.NumberOfItemsWithGeodatalov,
                geodatalovDatasetStatusReport.NumberOfItemsWithoutGeodatalov
            ));
        }

        private void WriteInspireStatusesInTable(StreamWriter streamWriter,
            InspireRegistryStatusReport inspireStatusReport)
        {
            WriteInspireDatasetStatusesInTable(streamWriter, inspireStatusReport);
            streamWriter.WriteLine();
            WriteInspireServiceStatusesInTable(streamWriter, inspireStatusReport);
        }

        private void WriteInspireDatasetStatusesInTable(StreamWriter streamWriter, InspireRegistryStatusReport inspireStatusReport)
        {
            streamWriter.WriteLine(SingelStatusReportHeading(DataSet.Dataset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.Metadata,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadata.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadata.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadata.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadata.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.MetadataServiceStatus,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadataService.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadataService.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadataService.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithMetadataService.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.Distribution,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithDistribution.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithDistribution.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithDistribution.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithDistribution.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.WmsStatus,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWms.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWms.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWms.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWms.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.WfsOrAtomStatus,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfs.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfs.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfs.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfs.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.AtomFeedStatus,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithAtomFeed.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithAtomFeed.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithAtomFeed.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithAtomFeed.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.WfsOrAtomStatus,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfsOrAtom.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfsOrAtom.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfsOrAtom.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithWfsOrAtom.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.HarmonizedDataStatus,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithHarmonizedData.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithHarmonizedData.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithHarmonizedData.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithHarmonizedData.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.SpatialDataServiceStatus,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithSpatialDataService.Good,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithSpatialDataService.Useable,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithSpatialDataService.Deficient,
                inspireStatusReport.InspireDatasetStatusReport.NumberOfItemsWithSpatialDataService.Notset));
        }

        private void WriteInspireServiceStatusesInTable(StreamWriter streamWriter, InspireRegistryStatusReport inspireStatusReport)
        {
            streamWriter.WriteLine(SingelStatusReportHeading(InspireDataSet.Service));

            streamWriter.WriteLine(StatusByType(InspireDataSet.Metadata,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadata.Good,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadata.Useable,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadata.Deficient,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadata.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.Metadata,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadataInSearchService.Good,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadataInSearchService.Useable,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadataInSearchService.Deficient,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithMetadataInSearchService.Notset));

            streamWriter.WriteLine(StatusByType(InspireDataSet.Distribution,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithServiceStatus.Good,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithServiceStatus.Useable,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithServiceStatus.Deficient,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithServiceStatus.Notset));

            streamWriter.WriteLine(SingelStatusReportHeadingOK());

            streamWriter.WriteLine(StatusOk(InspireDataSet.Sds,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithSds,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithoutSds
                ));

            streamWriter.WriteLine(StatusOk(InspireDataSet.NetworkService,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithNetworkService,
                inspireStatusReport.InspireServiceStatusReport.NumberOfItemsWithoutNetworkService
                ));
        }

        private string StatusByType(string statusType, int good, int useable, int deficient, int notSet, int satisfactory = 0)
        {
            return statusType + ";" + good + ";" + useable + ";" + deficient + ";" + notSet + ";" + satisfactory;
        }

        private string StatusOk(string statusType, int ok, int notOk)
        {
            return statusType + ";" + ok + ";" + notOk;
        }

        private string SingelStatusReportHeading(string label = "", string type ="")
        {
            var delivery_Status_Useable = DataSet.DOK_Delivery_Status_Useable;
            var delivery_Status_Deficient = DataSet.DOK_Delivery_Status_Deficient;

            if (type == "mareano")
            {
                delivery_Status_Useable = MareanoDataSet.Delivery_Status_Useable;
                delivery_Status_Deficient = MareanoDataSet.Delivery_Status_Deficient;
            }

                string heading = label + ";" + DataSet.DOK_Delivery_Status_Good + ";" +
                   delivery_Status_Useable + ";" +
                   delivery_Status_Deficient + ";" +
                   DataSet.DOK_Delivery_Status_NotSet;

            if(type == "mareano")
            {
               heading = heading + ";" + MareanoDataSet.Delivery_Status_Satisfactory;
            }

            return heading;
        }

        private string SingelStatusReportHeadingOK()
        {
            return "";
            //return ";" + Shared.Yes + ";" +
            //       Shared.No;
        }

        private string WriteDokStatusReport(DokStatusReport dokStatusReport)
        {
            return dokStatusReport.Date + ";" +
                   dokStatusReport.NumberOfItemsWithMetadata.Good + ";" +
                   dokStatusReport.NumberOfItemsWithMetadata.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithMetadata.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithMetadata.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithProductsheet.Good + ";" +
                   dokStatusReport.NumberOfItemsWithProductsheet.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithProductsheet.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithProductsheet.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithPresentationRules.Good + ";" +
                   dokStatusReport.NumberOfItemsWithPresentationRules.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithPresentationRules.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithPresentationRules.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithProductSpecification.Good + ";" +
                   dokStatusReport.NumberOfItemsWithProductSpecification.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithProductSpecification.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithProductSpecification.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithWms.Good + ";" +
                   dokStatusReport.NumberOfItemsWithWms.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithWms.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithWms.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithWfs.Good + ";" +
                   dokStatusReport.NumberOfItemsWithWfs.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithWfs.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithWfs.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithSosiRequirements.Good + ";" +
                   dokStatusReport.NumberOfItemsWithSosiRequirements.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithSosiRequirements.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithSosiRequirements.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithGmlRequirements.Good + ";" +
                   dokStatusReport.NumberOfItemsWithGmlRequirements.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithGmlRequirements.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithGmlRequirements.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithAtomFeed.Good + ";" +
                   dokStatusReport.NumberOfItemsWithAtomFeed.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithAtomFeed.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithAtomFeed.Notset + ";" +

                   dokStatusReport.NumberOfItemsWithDistribution.Good + ";" +
                   dokStatusReport.NumberOfItemsWithDistribution.Useable + ";" +
                   dokStatusReport.NumberOfItemsWithDistribution.Deficient + ";" +
                   dokStatusReport.NumberOfItemsWithDistribution.Notset;
        }


        private string WriteGeodatalovStatusReport(GeodatalovDatasetStatusReport geodatalovStatusReport)
        {
            return geodatalovStatusReport.Date + ";" +
                   geodatalovStatusReport.NumberOfItemsWithMetadata.Good + ";" +
                   geodatalovStatusReport.NumberOfItemsWithMetadata.Useable + ";" +
                   geodatalovStatusReport.NumberOfItemsWithMetadata.Deficient + ";" +
                   geodatalovStatusReport.NumberOfItemsWithMetadata.Notset + ";" +

                   geodatalovStatusReport.NumberOfItemsWithProductSpecification.Good + ";" +
                   geodatalovStatusReport.NumberOfItemsWithProductSpecification.Useable + ";" +
                   geodatalovStatusReport.NumberOfItemsWithProductSpecification.Deficient + ";" +
                   geodatalovStatusReport.NumberOfItemsWithProductSpecification.Notset + ";" +

                   geodatalovStatusReport.NumberOfItemsWithSosiRequirements.Good + ";" +
                   geodatalovStatusReport.NumberOfItemsWithSosiRequirements.Useable + ";" +
                   geodatalovStatusReport.NumberOfItemsWithSosiRequirements.Deficient + ";" +
                   geodatalovStatusReport.NumberOfItemsWithSosiRequirements.Notset + ";" +

                   geodatalovStatusReport.NumberOfItemsWithGmlRequirements.Good + ";" +
                   geodatalovStatusReport.NumberOfItemsWithGmlRequirements.Useable + ";" +
                   geodatalovStatusReport.NumberOfItemsWithGmlRequirements.Deficient + ";" +
                   geodatalovStatusReport.NumberOfItemsWithGmlRequirements.Notset + ";" +

                   geodatalovStatusReport.NumberOfItemsWithWms.Good + ";" +
                   geodatalovStatusReport.NumberOfItemsWithWms.Useable + ";" +
                   geodatalovStatusReport.NumberOfItemsWithWms.Deficient + ";" +
                   geodatalovStatusReport.NumberOfItemsWithWms.Notset + ";" +

                   geodatalovStatusReport.NumberOfItemsWithWfs.Good + ";" +
                   geodatalovStatusReport.NumberOfItemsWithWfs.Useable + ";" +
                   geodatalovStatusReport.NumberOfItemsWithWfs.Deficient + ";" +
                   geodatalovStatusReport.NumberOfItemsWithWfs.Notset + ";" +

                   geodatalovStatusReport.NumberOfItemsWithAtomFeed.Good + ";" +
                   geodatalovStatusReport.NumberOfItemsWithAtomFeed.Useable + ";" +
                   geodatalovStatusReport.NumberOfItemsWithAtomFeed.Deficient + ";" +
                   geodatalovStatusReport.NumberOfItemsWithAtomFeed.Notset + ";" +

                   geodatalovStatusReport.NumberOfItemsWithCommon.Good + ";" +
                   geodatalovStatusReport.NumberOfItemsWithCommon.Useable + ";" +
                   geodatalovStatusReport.NumberOfItemsWithCommon.Deficient + ";" +
                   geodatalovStatusReport.NumberOfItemsWithCommon.Notset + ";" +

                   geodatalovStatusReport.NumberOfItemsWithInspireTheme + ";" +
                   geodatalovStatusReport.NumberOfItemsWithoutInspireTheme + ";" +

                   geodatalovStatusReport.NumberOfItemsWithDok + ";" +
                   geodatalovStatusReport.NumberOfItemsWithoutDok + ";" +

                   geodatalovStatusReport.NumberOfItemsWithNationalDatasets + ";" +
                   geodatalovStatusReport.NumberOfItemsWithoutNationalDatasets + ";" +

                   geodatalovStatusReport.NumberOfItemsWithPlan + ";" +
                   geodatalovStatusReport.NumberOfItemsWithoutPlan + ";" +

                   geodatalovStatusReport.NumberOfItemsWithGeodatalov + ";" +
                   geodatalovStatusReport.NumberOfItemsWithoutGeodatalov + ";";


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
            bool isAdmin = ClaimsPrincipal.Current.IsInRole(GeonorgeRoles.MetadataAdmin);

            item.description = RemoveBreaksAndSemicolon(item.description);
            item.label = RemoveBreaksAndSemicolon(item.label);
            string text = null;
            if (item.itemclass == "Document")
            {
                text = item.label + ";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.documentreference + ";" + item.id;
            }
            else if (item.itemclass == "CodelistValue")
            {
                text = item.label + ";" + item.codevalue + ";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.ValidFrom + ";" + item.ValidTo + ";" + item.id + ";" + item.broader;
            }
            else if (item.itemclass == "EPSG")
            {
                text = item.label + ";" + item.epsgcode + ";" + item.sosiReferencesystem + ";" + item.verticalReferenceSystem + ";" + item.horizontalReferenceSystem + ";" + item.dimension + ";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.id;
            }
            else if (item.itemclass == "Dataset")
            {
                text = item.theme + ";" + item.label + ";" + item.owner + ";" + item.dokStatus + ";" + (item.dokStatusDateAccepted.HasValue ? item.dokStatusDateAccepted.Value.ToString("dd/MM/yyyy") : "") + ";" + (item.Kandidatdato.HasValue ? item.Kandidatdato.Value.ToString("dd/MM/yyyy") : "") + (isAdmin ? ";" + item.lastUpdated.ToString("dd/MM/yyyy") : "") + ";" + item.versionNumber + ";" + item.description + ";" + item.id + ";" + GetDOKDeliveryStatus(item) + (isAdmin ? ";" + item.uuid : "") + ";" + item.MetadataUrl;
            }
            else if (item.itemclass == "InspireDataset" || item.itemclass == "InspireDataService")
            {
                text = item.InspireTheme + ";" +
                        item.label + ";" +
                        item.owner + ";" +
                        item.dokStatus + ";" +
                        (item.dokStatusDateAccepted.HasValue ? item.dokStatusDateAccepted.Value.ToString("dd/MM/yyyy") : "") + ";" +
                        (item.Kandidatdato.HasValue ? item.Kandidatdato.Value.ToString("dd/MM/yyyy") : "") +
                        (isAdmin ? ";" + item.lastUpdated.ToString("dd/MM/yyyy") : "") + ";" +
                        item.versionNumber + ";" +
                        item.description + ";" +
                        item.id + ";" +
                        GetInspireDeliveryStatus(item) + ";" +
                        item.InspireDataType + ";" +
                        (isAdmin ? item.uuid.ToString() : "") + ";" +
                        item.MetadataUrl;
            }
            else if (item.itemclass == "GeodatalovDataset")
            {
                text = item.theme + ";" +
                       item.label + ";" +
                       item.owner + ";" +
                       item.dokStatus + ";" +
                       (item.dokStatusDateAccepted.HasValue ? item.dokStatusDateAccepted.Value.ToString("dd/MM/yyyy") : "") + ";" +
                       (item.Kandidatdato.HasValue ? item.Kandidatdato.Value.ToString("dd/MM/yyyy") : "") +
                       (isAdmin ? ";" + item.lastUpdated.ToString("dd/MM/yyyy") : "") + ";" +
                       item.versionNumber + ";" +
                       item.description + ";" +
                       item.id + ";" +
                       GetGeodatalovDeliveryStatus(item) +
                       (isAdmin ? ";" + item.uuid : "") + ";" +
                       item.MetadataUrl;
            }
            else if (item.itemclass == "MareanoDataset")
            {
                text = item.label + ";" +
                       item.owner + ";" +
                       item.lastUpdated.ToString("dd/MM/yyyy") + ";" +
                       item.versionNumber + ";" +
                       item.description + ";" +
                       item.id + ";" +
                       GetMareanoDeliveryStatus(item) +
                       (isAdmin ? ";" + item.uuid : "") + ";" +
                       item.MetadataUrl;
            }
            else if (item.itemclass == "Organization")
            {
                text = $"{item.label};{item.number};{item.MunicipalityCode};{item.GeographicCenterX};{item.GeographicCenterY};{item.BoundingBoxNorth};{item.BoundingBoxWest};{item.BoundingBoxSouth};{item.BoundingBoxEast};{HtmlHelperExtensions.TranslateBool(item.NorgeDigitaltMember)}";
            }
            else if (item.itemclass == "NameSpace")
            {
                text = item.label + ";" + item.owner + ";" + item.description + ";" + item.serviceUrl + ";";
            }
            else if (item.itemclass == "Alert")
            {
                text = item.AlertDate.ToString("dd/MM/yyyy") + ";" + item.EffectiveDate.ToString("dd/MM/yyyy") + ";" + item.label + ";" + item.ServiceType + ";" + item.AlertType + ";" + item.owner + ";" + RemoveBreaksAndSemicolon(item.Note) + ";" + item.MetadataUrl;
            }

            streamWriter.WriteLine(text);
        }

        private static string GetDOKDeliveryStatus(Registeritem item)
        {

            return item.dokDeliveryMetadataStatus + ";" + item.dokDeliveryProductSheetStatus + ";" + item.dokDeliveryPresentationRulesStatus +
                ";" + item.dokDeliveryProductSpecificationStatus + ";" + item.dokDeliveryWmsStatus + ";" + item.dokDeliveryWfsStatus +
                ";" + item.dokDeliverySosiRequirementsStatus + ";" + item.dokDeliveryDistributionStatus + ";" + item.dokDeliveryGmlRequirementsStatus + ";" + item.dokDeliveryAtomFeedStatus;

        }

        private static string GetInspireDeliveryStatus(Registeritem item)
        {
            if (item.itemclass == "InspireDataset")
            {
                return item.MetadataStatus + ";" +
                   item.MetadataServiceStatus + ";" +
                   item.DistributionStatus + ";" +
                   item.WmsStatus + ";" +
                   item.WfsStatus + ";" +
                   item.AtomFeedStatus + ";" +
                   item.WfsOrAtomStatus + ";" +
                   item.HarmonizedDataStatus + ";" +
                   item.SpatialDataServiceStatus + ";;;;;";
            }
            else if (item.itemclass == "InspireDataService")
            {
                return item.MetadataStatus + ";" +
                   ";;;;;;;;" +
                   item.MetadataInSearchServiceStatus + ";" +
                   item.ServiceStatus + ";" +
                   item.Requests + ";" +
                   item.NetworkService + ";" +
                   item.Sds;
            }
            else return "";
        }

        private static string GetGeodatalovDeliveryStatus(Registeritem item)
        {
            return item.MetadataStatus + ";" +
                   item.ProductspesificationStatus + ";" +
                   item.SosiStatus + ";" +
                   item.GmlStatus + ";" +
                   item.WmsStatus + ";" +
                   item.WfsStatus + ";" +
                   item.AtomFeedStatus + ";" +
                   item.CommonStatus;
        }

        private static string GetMareanoDeliveryStatus(Registeritem item)
        {
            return item.FindableStatus + ";" +
                    item.AccesibleStatus + ";" +
                    item.InteroperableStatus + ";" +
                    item.ReUsableStatus + ";" +
                    item.MetadataStatus + ";" +
                   item.ProductspesificationStatus + ";" +
                   item.ProductSheetStatus + ";" +
                   item.PresentationRulesStatus + ";" +
                   item.SosiStatus + ";" +
                   item.GmlStatus + ";" +
                   item.WmsStatus + ";" +
                   item.WfsStatus + ";" +
                   item.AtomFeedStatus + ";" +
                   item.CommonStatus;
        }

        private static void ConvertRegisterItemDokMunicipalToCSV(StreamWriter streamWriter, Registeritem item)
        {
            item.description = RemoveBreaksAndSemicolon(item.description);
            string text = null;
            text = item.theme + ";" + item.label + ";" + item.owner + ";" + item.dokStatus + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.DatasetType + ";" + item.ConfirmedDok + ";" + item.Coverage + ";" + item.Measure + ";" + item.NoteMunicipal + ";" + item.MetadataUrl;
            streamWriter.WriteLine(text);
        }

        private static void ConvertRegisterToCSV(StreamWriter streamWriter, Models.Api.Register register)
        {
            register.contentsummary = RemoveBreaksAndSemicolon(register.contentsummary);
            string text = register.id + ";" + register.label + ";" + register.contentsummary + ";" + register.owner + ";" + register.lastUpdated.ToString("dd/MM/yyyy");
            streamWriter.WriteLine(text);
        }

        private static string RemoveBreaksAndSemicolon(string text)
        {
            string replaceWith = " ";
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith).Replace(";", ".");
            }
            else
            {
                text = Registers.NotSet;
            }

            return text;
        }

        private string RegisterItemHeading(string containedItemClass)
        {
            bool isAdmin = ClaimsPrincipal.Current.IsInRole(GeonorgeRoles.MetadataAdmin);

            if (containedItemClass == "Document")
            {
                return Registers.Name + ";" + Documents.DocumentOwner + ";Status;" + Registers.Updated + ";" + Registers.VersionNumber + ";" + Registers.Description + ";" + Registers.DocumentUrl + ";ID";
            }
            if (containedItemClass == "CodelistValue")
            {
                return Registers.Name + ";" + CodelistValues.CodeValue + ";" + Registers.Owner + ";Status;" + Registers.Updated + ";" + Registers.VersionNumber + ";" + Registers.Description + ";" + CodelistValues.ValidFromDate + ";" + CodelistValues.ValidToDate + ";ID;" + CodelistValues.BroaderItem;
            }
            if (containedItemClass == "EPSG")
            {
                return Registers.Name + "; EPSG; SOSI;" + EPSGs.Vertical + ";" + EPSGs.Horizontal + ";" + EPSGs.Dimension + ";" + Registers.Owner + ";Status;" + Registers.Updated + ";" + Registers.VersionNumber + ";" + Registers.Description + ";ID";
            }
            if (containedItemClass == "Dataset")
            {
                return DataSet.DOK_Delivery_Theme + ";" + Registers.Name + ";" + Registers.Owner + ";DOK-status;" + DataSet.DOK_StatusDateAccepted + ";" + DataSet.DOK_Kandidatdato + (isAdmin ? ";" + Registers.Updated : "") + ";" + Registers.VersionNumber + ";" + Registers.Description + ";ID" + ";" + DataSet.DOK_Delivery_Metadata + ";"
                    + DataSet.DOK_Delivery_ProductSheet + ";" + DataSet.DOK_Delivery_PresentationRules + ";" + DataSet.DOK_Delivery_ProductSpesification + ";"
                    + DataSet.DOK_Delivery_Wms + ";" + DataSet.DOK_Delivery_Wfs + ";" + DataSet.DOK_Delivery_SosiRequirements + ";"
                    + DataSet.DOK_Delivery_Distribution + ";" + DataSet.DOK_Delivery_GmlRequirements + ";" + DataSet.DOK_Delivery_AtomFeed + (isAdmin ? ";Uuid" : "") + ";" + DataSet.DisplayKartkatalogen;
            }
            if (containedItemClass == "InspireDataset")
            {
                return "InspireTema;" +
                        Registers.Name + ";" +
                        Registers.Owner + ";DOK-status;" +
                        DataSet.DOK_StatusDateAccepted + ";" +
                        DataSet.DOK_Kandidatdato + (isAdmin ? ";" + Registers.Updated : "") + ";" +
                        Registers.VersionNumber + ";" +
                        Registers.Description + ";ID" + ";" +
                        DataSet.DOK_Delivery_Metadata + ";" +
                        InspireDataSet.MetadataServiceStatus + ";" +
                        InspireDataSet.DistributionStatus + ";" +
                        DataSet.DOK_Delivery_Wms + ";" +
                        DataSet.DOK_Delivery_Wfs + ";" +
                        DataSet.DOK_Delivery_AtomFeed + ";" +
                        InspireDataSet.WfsOrAtomStatus + ";" +
                        InspireDataSet.HarmonizedDataStatus + ";" +
                        InspireDataSet.SpatialDataServiceStatus + ";" +
                        "Metadata i søketjenesten;" +
                        "Tjenestestatus;" +
                        "Requests;" +
                        "Nettverkstjeneste;" +
                        "Sds;" +
                        "Datatype;" +
                        (isAdmin ? "Uuid" : "") + ";" +
                        DataSet.DisplayKartkatalogen;
            }
            if (containedItemClass == "GeodatalovDataset")
            {
                return DataSet.DOK_Delivery_Theme + ";" +
                       Registers.Name + ";" +
                       Registers.Owner + "; DOK-status;" +
                       DataSet.DOK_StatusDateAccepted + ";" +
                       DataSet.DOK_Kandidatdato + (isAdmin ? ";" + Registers.Updated : "") + ";" +
                       Registers.VersionNumber + ";" +
                       Registers.Description + "; ID" + ";" +
                       DataSet.DOK_Delivery_Metadata + ";" +
                       DataSet.DOK_Delivery_ProductSpesification + ";" +
                       DataSet.DOK_Delivery_SosiRequirements + ";" +
                       DataSet.DOK_Delivery_GmlRequirements + ";" +
                       DataSet.DOK_Delivery_Wms + ";" +
                       DataSet.DOK_Delivery_Wfs + ";" +
                       DataSet.DOK_Delivery_AtomFeed + ";" +
                       DataSet.Delivery_Common + ";" + (isAdmin ? ";Uuid" : "") + ";" +
                       DataSet.DisplayKartkatalogen;
            }
            if (containedItemClass == "MareanoDataset")
            {
                return Registers.Name + ";" +
                       Registers.Owner + ";" +
                       Registers.Updated + ";" +
                       Registers.VersionNumber + ";" +
                       Registers.Description + "; ID" + ";" +
                       MareanoDataSet.Findable_Label + ";" +
                       MareanoDataSet.Accesible_Label + ";" +
                       MareanoDataSet.Interoperable_Label + ";" +
                       MareanoDataSet.ReUseable_Label + ";" +
                       DataSet.DOK_Delivery_Metadata + ";" +
                       DataSet.DOK_Delivery_ProductSpesification + ";" +
                       DataSet.DOK_Delivery_ProductSheet + ";" +
                       DataSet.DOK_Delivery_PresentationRules + ";" +
                       DataSet.DOK_Delivery_SosiRequirements + ";" +
                       DataSet.DOK_Delivery_GmlRequirements + ";" +
                       DataSet.DOK_Delivery_Wms + ";" +
                       DataSet.DOK_Delivery_Wfs + ";" +
                       DataSet.DOK_Delivery_AtomFeed + ";" +
                       DataSet.Delivery_Common + (isAdmin ? ";Uuid" : "") + ";" +
                       DataSet.DisplayKartkatalogen;
            }
            if (containedItemClass == "Organization")
            {
                return Organizations.Organization_Name + ";" + Organizations.Organization_Number + ";" + Organizations.MunicipalityCode + ";" + Organizations.GeographicCenterX + ";" + Organizations.GeographicCenterY + ";" + Organizations.BoundingBoxNorth + ";" + Organizations.BoundingBoxWest + ";" + Organizations.BoundingBoxSouth + ";" + Organizations.BoundingBoxEast + ";" + Organizations.Member;
            }
            if (containedItemClass == "NameSpace")
            {
                return Registers.Name + ";" + Registers.Etat + ";" + Namespace.Content + ";" + Namespace.Service;
            }
            if (containedItemClass == "Alert")
            {
                return Alerts.LastAlert + ";" + Alerts.EffectiveDate + ";" + Alerts.Service + ";" + Alerts.Type + ";" + Alerts.AlertType + ";" + Registers.Owner + ";" + Alerts.Note + ";" + Alerts.Description;
            }

            return null;
        }

        private string RegisterHeading()
        {
            return "Id;" + Registers.Name + ";" + Registers.Description + ";" + Registers.Owner + ";" + Registers.Updated;
        }

        private string DokStatusReportHeading()
        {
            return Registers.Date + ";" +
                   DataSet.DOK_Delivery_Metadata + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_ProductSheet + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_PresentationRules + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_ProductSpesification + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_Wms + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_Wfs + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_SosiRequirements + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_GmlRequirements + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_AtomFeed + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_Distribution;
        }
        private string GeodatalovStatusReportHeading()
        {
            return Registers.Date + ";" +
                   DataSet.DOK_Delivery_Metadata + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_ProductSpesification + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_SosiRequirements + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_GmlRequirements + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_Wms + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_Wfs + ";" + ";" + ";" + ";" +
                   DataSet.DOK_Delivery_AtomFeed + ";" + ";" + ";" + ";" +
                   GeodatalovDataSet.Common + ";" + ";" + ";" + ";" +
                   GeodatalovDataSet.InspireTheme + ";" + ";" +
                   GeodatalovDataSet.Dok + ";" + ";" +
                   GeodatalovDataSet.NationalDataset + ";" + ";" +
                   GeodatalovDataSet.Plan + ";" + ";" +
                   GeodatalovDataSet.Geodatalov + ";" + ";";
        }


        private string InspireDatasetReportHeading()
        {
            return Registers.Date + ";" +
                   InspireDataSet.Metadata + ";" + ";" + ";" + ";" +
                   InspireDataSet.MetadataServiceStatus + ";" + ";" + ";" + ";" +
                   InspireDataSet.Distribution + ";" + ";" + ";" + ";" +
                   InspireDataSet.WmsStatus + ";" + ";" + ";" + ";" +
                   InspireDataSet.WfsStatus + ";" + ";" + ";" + ";" +
                   InspireDataSet.AtomFeedStatus + ";" + ";" + ";" + ";" +
                   InspireDataSet.WfsOrAtomStatus + ";" + ";" + ";" + ";" +
                   InspireDataSet.HarmonizedDataStatus + ";" + ";" + ";" + ";" +
                   InspireDataSet.SpatialDataServiceStatus + ";" + ";" + ";" + ";";
        }

        private string StatusReportHeadingStatusValues()
        {
            return ";" +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses();
        }

        private string StatusReportHeadingStatusValuesGeodatalov()
        {
            return ";" +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatusesOk() +
                   HeadingStatusesOk() +
                   HeadingStatusesOk() +
                   HeadingStatusesOk() +
                   HeadingStatusesOk();
        }

        private string InspireServiceStatusReportHeadingStatusValues()
        {
            return ";" +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatuses() +
                   HeadingStatusesOk() +
                   HeadingStatusesOk();

        }

        private static string HeadingStatuses()
        {
            return DataSet.DOK_Delivery_Status_Good + ";" + DataSet.DOK_Delivery_Status_Useable + ";" + DataSet.DOK_Delivery_Status_Deficient + ";" + DataSet.DOK_Delivery_Status_NotSet + ";";
        }

        private static string HeadingStatusesOk()
        {
            return Shared.Yes + ";" + Shared.No + ";";
        }

        private string RegisterItemDokMunicipalHeading(Models.Api.Register register)
        {
            string heading = "Det offentlige kartgrunnlaget - " + register.SelectedDOKMunicipality + ", " + DateTime.Today.ToString("d") + "\r\n";
            heading = heading + DataSet.DOK_Delivery_Theme + ";" + Registers.Name + ";" + Registers.Owner + ";DOK-status;" + Registers.Updated + ";" + Registers.VersionNumber + ";" + Registers.Description + ";" + DataSet.RegionType + ";" + DataSet.DOK_Confirmed + ";" + DataSet.DOK_Coverage + ";" + DataSet.DOK_Coverage_Measure + ";" + DataSet.DOK_Note + ";" + DataSet.DisplayKartkatalogen;
            return heading;
        }

        private void SetDokMunicipalDataset(string registerName)
        {
            IsRegisterDokMunicipal = (registerName == "Det offentlige kartgrunnlaget - Kommunalt");
        }

        bool IsRegisterDokMunicipal = false;
    }
}