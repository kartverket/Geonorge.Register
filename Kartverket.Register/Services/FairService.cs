using GeoNorgeAPI;
using Kartverket.Register.Models;
using Kartverket.Register.Models.FAIR;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Runtime.Remoting.MetadataServices;
using System.Web;
using System.Web.Configuration;

namespace Kartverket.Register.Services
{
    public interface IFairService
    {
        Fair GetFair(MetadataModel _metadata, DatasetDelivery wfsStatus, DatasetDelivery wmsStatus,
            DatasetDelivery atomFeedStatus);
    }
    public class FairService : IFairService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Fair GetFair(MetadataModel _metadata, DatasetDelivery wfsStatus, DatasetDelivery wmsStatus,
            DatasetDelivery atomFeedStatus)
        {
            Fair dataset = new Fair();

            //dataset.I1_c_Criteria = null; //Moved to I3_a_Criteria
            dataset.I3_a_Criteria = null;
            dataset.I3_b_Criteria = null;
            dataset.I3_c_Criteria = null;

            int findableWeight = 0;
            if (_metadata?.SimpleMetadata == null)
                return null;

            dataset.F2_a_Criteria = SimpleKeyword.Filter(_metadata.SimpleMetadata.Keywords.ToList(), SimpleKeyword.TYPE_THEME, null)?.Count() >= 3;
            dataset.F2_b_Criteria = _metadata.SimpleMetadata.Title.Count() <= 105;
            dataset.F2_c_Criteria = _metadata.SimpleMetadata.Abstract?.Count() >= 200;
            dataset.F2_d_Criteria = _metadata.SimpleMetadata.BoundingBox != null;
            dataset.F2_e_Criteria = _metadata.SimpleMetadata.DateCreated != null;
            dataset.F3_a_Criteria = _metadata.SimpleMetadata.ResourceReference != null ? _metadata.SimpleMetadata.ResourceReference?.Code != null && _metadata.SimpleMetadata.ResourceReference?.Codespace != null : false;

            if (dataset.F1_a_Criteria) findableWeight += 20;
            if (dataset.F2_a_Criteria) findableWeight += 10;
            if (dataset.F2_b_Criteria) findableWeight += 5;
            if (dataset.F2_c_Criteria) findableWeight += 10;
            if (dataset.F2_d_Criteria) findableWeight += 10;
            if (dataset.F2_e_Criteria) findableWeight += 5;
            if (dataset.F3_a_Criteria) findableWeight += 20;
            if (dataset.F4_a_Criteria) findableWeight += 20;

            dataset.FindableStatusPerCent = findableWeight;
            dataset.FindableStatus = CreateFairDelivery(findableWeight);

            int accesibleWeight = 0;

            dataset.A1_a_Criteria = CheckDownloadService(_metadata.SimpleMetadata.Uuid, wfsStatus);
            dataset.A1_b_Criteria = CheckViewService(_metadata.SimpleMetadata.Uuid, wmsStatus);
            dataset.A1_c_Criteria = _metadata.SimpleMetadata?.DistributionsFormats != null ? _metadata.SimpleMetadata.DistributionsFormats.Where(p => !string.IsNullOrEmpty(p.Protocol) && p.Protocol.Contains("GEONORGE:DOWNLOAD")).Any() : false;
            dataset.A1_d_Criteria = atomFeedStatus != null ? atomFeedStatus.IsGood() : false;
            dataset.A1_e_Criteria =  dataset.A1_c_Criteria || dataset.A1_d_Criteria || CheckDistributionUrl(_metadata.SimpleMetadata.Uuid, _metadata.SimpleMetadata.DistributionsFormats.Where(f => !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("GEONORGE:DOWNLOAD") || !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("WWW:DOWNLOAD") || !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("GEONORGE:FILEDOWNLOAD")));
            dataset.A1_f_Criteria = true;

            if (dataset.A1_a_Criteria) accesibleWeight += 20;
            if (dataset.A1_b_Criteria) accesibleWeight += 20;
            if (dataset.A1_c_Criteria) accesibleWeight += 5;
            if (dataset.A1_d_Criteria) accesibleWeight += 5;
            if (dataset.A1_e_Criteria) accesibleWeight += 50;
            if (dataset.A1_f_Criteria) accesibleWeight += 0;
            if (dataset.A2_a_Criteria) accesibleWeight += 0;

            dataset.AccesibleStatusPerCent = accesibleWeight;
            dataset.AccesibleStatus = CreateFairDelivery(accesibleWeight);
            int interoperableWeight = 0;

            var spatialRepresentation = _metadata.SimpleMetadata.SpatialRepresentation;
            if (spatialRepresentation == "vector")
            {
                dataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "GML" || p.FormatName == "GeoJSON" || p.FormatName == "JSON-FG" || p.FormatName == "JSON-LD" || p.FormatName == "GeoPackage" || p.FormatName == "COPC" || p.FormatName == "GeoParquet" || p.FormatName == "Shape").Any();
                if (!dataset.I1_b_Criteria)
                    dataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "NetCDF-CF").Any();
            }
            else if (spatialRepresentation == "grid")
            {
                dataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "GeoTIFF" || p.FormatName == "TIFF" || p.FormatName == "JPEG" || p.FormatName == "JPEG2000" || p.FormatName == "GeoPackage" || p.FormatName == "COG" || p.FormatName == "COPC").Any();
                if (!dataset.I1_b_Criteria)
                    dataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "NetCDF-CF").Any();
            }
    
            dataset.I2_a_Criteria = _metadata.SimpleMetadata.Keywords.Where(k => !string.IsNullOrEmpty(k.Thesaurus)).ToList().Count() >= 1;
            dataset.I2_b_Criteria = SimpleKeyword.Filter(_metadata.SimpleMetadata.Keywords, null, SimpleKeyword.THESAURUS_NATIONAL_THEME).ToList().Count() >= 1;

            dataset.I3_a_Criteria = _metadata.SimpleMetadata.QualitySpecifications != null && _metadata.SimpleMetadata.QualitySpecifications.Count > 0
                                            ? _metadata.SimpleMetadata.QualitySpecifications.Where(r => !string.IsNullOrEmpty(r.Explanation) && r.Explanation.Contains("er i henhold")).Any() : false;

            if (spatialRepresentation == "grid")
            {
                dataset.I3_b_Criteria = true;
                dataset.I3_c_Criteria = true;
            }
            else if (spatialRepresentation == "vector")
            { 
                dataset.I3_b_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.ApplicationSchema);
                dataset.I3_c_Criteria = _metadata.SimpleMetadata.QualitySpecifications != null && _metadata.SimpleMetadata.QualitySpecifications.Count > 0
                                            ? _metadata.SimpleMetadata.QualitySpecifications.Where(r => !string.IsNullOrEmpty(r.Explanation) && r.Explanation.Contains("er i henhold til applikasjonsskjema")).Any() : false;
            }

            if (dataset.I1_a_Criteria) interoperableWeight += 20;
            if (dataset.I1_b_Criteria) interoperableWeight += 40;
            if (dataset.I2_a_Criteria) interoperableWeight += 10;
            if (dataset.I2_b_Criteria) interoperableWeight += 10;
            if (!dataset.I3_a_Criteria.HasValue || (dataset.I3_a_Criteria.HasValue && dataset.I3_a_Criteria.Value)) interoperableWeight += 10;

            if (!dataset.I3_b_Criteria.HasValue || (dataset.I3_b_Criteria.HasValue && dataset.I3_b_Criteria.Value)) interoperableWeight += 5;
            if (!dataset.I3_c_Criteria.HasValue || (dataset.I3_c_Criteria.HasValue && dataset.I3_c_Criteria.Value)) interoperableWeight += 5;

            dataset.InteroperableStatusPerCent = interoperableWeight;
            dataset.InteroperableStatus = CreateFairDelivery(interoperableWeight);
            int reusableWeight = 0;

            dataset.R1_a_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.Constraints?.UseConstraintsLicenseLink);
            dataset.R1_b_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.Constraints?.AccessConstraintsLink);
            dataset.R2_a_Criteria = _metadata.SimpleMetadata?.ProcessHistory != null ? _metadata.SimpleMetadata?.ProcessHistory.Count() > 200 : false;
            dataset.R2_b_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.MaintenanceFrequency);
            dataset.R2_c_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ProductSpecificationUrl);
            dataset.R2_d_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ResolutionScale) || _metadata.SimpleMetadata?.ResolutionDistance != null;
            dataset.R2_e_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageUrl)
                                           || !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageGridUrl)
                                           || !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageCellUrl);

            dataset.R2_f_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.Purpose);

            dataset.R2_g_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ContactMetadata?.Organization);
            dataset.R2_h_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ContactPublisher?.Organization);
            dataset.R2_i_Criteria = _metadata.SimpleMetadata.QualitySpecifications != null && _metadata.SimpleMetadata.QualitySpecifications.Count > 0
                                            ? _metadata.SimpleMetadata.QualitySpecifications.Where(r => !string.IsNullOrEmpty(r.Explanation) && r.Title.Contains("dekning")).Any() : false;

            dataset.R3_b_Criteria = dataset.I1_b_Criteria;

            if (dataset.R1_a_Criteria) reusableWeight += 30;
            if (dataset.R1_b_Criteria) reusableWeight += 5;
            if (dataset.R2_a_Criteria) reusableWeight += 10;
            if (dataset.R2_b_Criteria) reusableWeight += 5;
            if (dataset.R2_c_Criteria) reusableWeight += 10;
            if (dataset.R2_d_Criteria) reusableWeight += 5;
            if (dataset.R2_e_Criteria) reusableWeight += 5;
            if (dataset.R2_f_Criteria) reusableWeight += 5;
            if (dataset.R2_g_Criteria) reusableWeight += 5;
            if (dataset.R2_h_Criteria) reusableWeight += 5;
            if (dataset.R2_i_Criteria) reusableWeight += 5;
            if (dataset.R3_a_Criteria) reusableWeight += 5;
            if (dataset.R3_b_Criteria) reusableWeight += 5;

            dataset.ReUseableStatusPerCent = reusableWeight;
            dataset.ReUseableStatus = CreateFairDelivery(reusableWeight);

            int fairWeight = (findableWeight + accesibleWeight + interoperableWeight + reusableWeight) / 4;
            dataset.FAIRStatusPerCent = fairWeight;
            dataset.FAIRStatus = CreateFairDelivery(fairWeight);

            return dataset;
        }

        public FAIRDelivery CreateFairDelivery(int weight, string fairStatusId = null, string note = "", bool autoupdate = true)
        {
            if (string.IsNullOrEmpty(fairStatusId))
            {
                if (weight > 90)
                    fairStatusId = FAIRDelivery.Good;
                else if (weight >= 75 && weight <= 90)
                    fairStatusId = FAIRDelivery.Satisfactory;
                else if (weight < 75 && weight >= 50)
                    fairStatusId = FAIRDelivery.Useable;
                else
                    fairStatusId = FAIRDelivery.Deficient;
            }

            var fairDelivery = new FAIRDelivery(fairStatusId, note, autoupdate);
            return fairDelivery;
        }

        private bool CheckViewService(string uuid, DatasetDelivery datasetDelivery)
        {
            if (datasetDelivery != null)
            {
                bool hasWms = datasetDelivery.IsGoodOrUseable();
                bool hasWMTS = false;
                var distros = GetDistributions(uuid);
                if (distros != null)
                {
                    foreach (var distro in distros)
                    {
                        string protocol = distro?.Protocol;
                        if (!string.IsNullOrEmpty(protocol) && protocol.Contains("WMTS"))
                            hasWMTS = true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Maps"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Tiles"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Styles"))
                            return true;
                    }

                    if (hasWms || hasWMTS)
                        return true;
                }
            }

            return false;
        }

        private bool CheckDownloadService(string uuid, DatasetDelivery datasetDelivery)
        {
            if (datasetDelivery != null)
            {
                bool hasWfs = datasetDelivery.IsGoodOrUseable();
                bool hasWcs = false;
                var distros = GetDistributions(uuid);
                if (distros != null)
                {
                    foreach (var distro in distros)
                    {
                        string protocol = distro?.Protocol;
                        if (!string.IsNullOrEmpty(protocol) && protocol.Contains("WCS"))
                            hasWcs = true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Features"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Coverages"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("OpenDAP"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("API-EDR"))
                            return true;
                    }

                    if (hasWfs || hasWcs)
                        return true;
                }
            }

            return false;
        }

        public static dynamic GetDistributions(string metadataUuid)
        {
            try
            {
                var metadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/distributions/" + metadataUuid;
                var c = new WebClient { Encoding = System.Text.Encoding.UTF8 };

                var json = c.DownloadString(metadataUrl);

                dynamic metadata = Newtonsoft.Json.Linq.JArray.Parse(json);
                return metadata;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool CheckDistributionUrl(string uuid, IEnumerable<SimpleDistribution> distributions)
        {
            if (distributions != null && distributions.Count() > 0)
            {
                var url = distributions.FirstOrDefault().URL;
                var protocol = distributions.FirstOrDefault().Protocol;

                if (!string.IsNullOrEmpty(url) && (url.StartsWith("https://")))
                {
                    try
                    {
                        if (protocol == "GEONORGE:DOWNLOAD")
                            url = url + uuid;

                        _httpClient.DefaultRequestHeaders.Accept.Clear();
                        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                        Log.Debug("Connecting to: " + url);

                        HttpResponseMessage response = _httpClient.GetAsync(new Uri(url)).Result;

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            Log.Error("Url svarer ikke: " + url + " , statuskode: " + response.StatusCode);
                        }
                        else
                            return true;

                    }
                    catch (Exception ex)
                    {
                        Log.Error("Url svarer ikke: " + url, ex);
                        return false;
                    }
                }
            }

            return false;
        }

    }
}