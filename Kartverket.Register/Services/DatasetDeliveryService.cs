using Kartverket.Register.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;

namespace Kartverket.Register.Services
{
    public class DatasetDeliveryService : IDatasetDeliveryService
    {
        private readonly RegisterDbContext _dbContext;
        private XmlDocument _atomFeedDoc;
        private XmlNamespaceManager _nsmgr;
        private const string Useable = "useable";
        private const string Good = "good";
        public const string Notset = "notset";
        private const string Deficient = "deficient";


        public DatasetDeliveryService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Guid CreateDatasetDelivery(string deliveryStatusId, string note, bool autoupdate = true)
        {
            var datasetDelivery = new DatasetDelivery(deliveryStatusId, note, autoupdate);
            _dbContext.DatasetDeliveries.Add(datasetDelivery);
            _dbContext.SaveChanges();
            return datasetDelivery.DatasetDeliveryId;
        }

        public SelectList GetDokDeliveryStatusesAsSelectlist(string statusId)
        {
            return new SelectList(_dbContext.DokDeliveryStatuses, "value", "description", statusId);
        }

        public string GetMetadataStatus(string metadataUuid, bool autoUpdate, string currentStatus = Deficient)
        {
            var statusValue = currentStatus;
            if (!autoUpdate) return statusValue;
            try
            {
                if (string.IsNullOrEmpty(metadataUuid)) return Useable;

                var c = new WebClient { Encoding = System.Text.Encoding.UTF8 };
                var url = WebConfigurationManager.AppSettings["EditorUrl"] + "api/validatemetadata/" + metadataUuid;
                var data = c.DownloadString(url);
                var response = Newtonsoft.Json.Linq.JObject.Parse(data);
                var status = response["Status"];

                if (status == null) return Useable;
                var statusvalue = status.ToString();
                return statusvalue == "OK" ? Good : Useable;
            }
            catch (Exception)
            {
                return Deficient;
            }
        }

        public string GetDeliveryDistributionStatus(string distributionUrl, bool autoupdate, string currentStatus)
        {
            var status = currentStatus;
            try
            {
                var hasDistributionUrl = distributionUrl != null;
                if (autoupdate)
                {
                    status = hasDistributionUrl ? Good : Deficient;
                }
            }
            catch (Exception)
            {
                return Deficient;
            }

            return status;
        }

        public string GetDokDeliveryServiceStatus(string metadataUuid, bool autoupdate, string currentStatus, string serviceUuid)
        {
            var hasServiceUrl = false;
            var status = Deficient;

            if (!autoupdate) return status;
            try
            {
                dynamic metadata = GetMetadataFromKartkatalogen(metadataUuid);

                if (metadata != null && metadata.Related != null)
                    foreach (var service in metadata.Related)
                    {
                        if (service.DistributionDetails != null &&
                            service.DistributionDetails.Protocol.Value == "OGC:WMS") hasServiceUrl = true;
                    }
            }
            catch (Exception)
            {

            }

            var statusUrl = WebConfigurationManager.AppSettings["StatusApiUrl"] + "monitorApi/serviceDetail?uuid=";
            statusUrl = statusUrl + serviceUuid;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                try
                {
                    var response = client.GetAsync(new Uri(statusUrl)).Result;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var text = response.Content.ReadAsStringAsync().Result;
                        dynamic data = Newtonsoft.Json.Linq.JObject.Parse(text);

                        var responseTime = (double)data.svartid;
                        var resposeGetCapabilities = false;
                        var supportCors = false;
                        var epsgSupport = false;
                        var featuresSupport = false;
                        var hasLegend = false;
                        var hasCoverage = false;
                        var connectSoso = false;

                        if (data.connect.vurdering == "yes")
                            resposeGetCapabilities = true;

                        if (data.connect.vurdering == "soso")
                            connectSoso = true;

                        if (data.cors.vurdering == "yes")
                            supportCors = true;

                        if (data.epsgSupported.vurdering == "yes")
                            epsgSupport = true; //Todo check epsg code

                        if (data.hasGFI.vurdering == "yes")
                            featuresSupport = true;

                        if (data.hasLegend.vurdering == "yes")
                            hasLegend = true;

                        if (data.bbox.vurdering == "yes")
                            hasCoverage = true;

                        if (!hasServiceUrl)
                            status = Deficient;
                        //Grønn på WMS:
                        //Respons fra GetCapabilities
                        //Svartid innen 4 sekunder
                        //Støtter CORS
                        //EPSG: 25832, 25833, 25835
                        //Støtter egenskapsspørringer
                        //Støtter tegnforklaring
                        //Oppgir dekningsområde
                        else if ((resposeGetCapabilities && responseTime <= 4
                                  && supportCors && epsgSupport && featuresSupport
                                  && hasLegend && hasCoverage) || connectSoso)
                            status = Good;
                        //Gul:
                        //Respons fra GetCapabilities
                        //Svartid innen 10 sekunder
                        //Støtter CORS
                        //EPSG: 25833, 25835 eller 32633
                        //Støtter tegnforklaring
                        //Oppgir dekningsområde
                        else if ((resposeGetCapabilities && responseTime <= 10
                                  && epsgSupport && hasLegend && hasCoverage))
                            status = Useable;
                        //Rød:
                        //Feiler på en av testene til gul
                        else
                            status = Deficient;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return status;
        }

        public string GetWfsStatus(string metadataUuid, bool autoupdate, string currentStatus)
        {
            var statusValue = currentStatus;

            if (!autoupdate) return statusValue;
            try
            {
                var metadata = GetMetadataFromKartkatalogen(metadataUuid);

                if (metadata != null && metadata.Related != null)
                {
                    foreach (var service in metadata.Related)
                    {
                        if (service.DistributionDetails != null && service.DistributionDetails.Protocol == "OGC:WFS"
                        ) return Useable;
                    }
                }
                else
                {
                    return Deficient;
                }
            }

            catch (Exception)
            {
                return Notset;
            }

            return statusValue;
        }

        public string GetAtomFeedStatus(string metadataUuid, bool autoUpdate, string currentStatus)
        {
            var statusValue = currentStatus;
            if (!autoUpdate) return statusValue;
            try
            {
                var atomfeed = AtomFeed(metadataUuid);
                statusValue = !string.IsNullOrEmpty(atomfeed) ? Good : Deficient;
            }
            catch (Exception)
            {
                return Notset;
            }

            return statusValue;
        }

        public string GetHarmonizedStatus(string metadataUuid, bool autoUpdate, string currentStatus)
        {
            string status;
            try
            {
                var metadata = GetMetadataFromKartkatalogen(metadataUuid);
                if (metadata != null)
                {
                    if (metadata.QualitySpecifications != null)
                    {
                        status = metadata.QualitySpecifications[0].Result == "true" ? Good : Deficient;
                    }
                    else
                    {
                        status = Deficient;
                    }
                }
                else
                {
                    status = Deficient;
                }
            }

            catch (Exception)
            {
                return Deficient;
            }
            return status;
        }

        public string GetSpatialDataStatus(string metadataUuid, bool autoUpdate, string currentStatus)
        {
            string status;
            try
            {
                var metadata = GetMetadataFromKartkatalogen(metadataUuid);

                if (metadata != null)
                {
                    if (metadata.DistributionDetails != null && metadata.DistributionDetails.Protocol == "GEONORGE:DOWNLOAD")
                    {
                        if (metadata.ServiceUuid != null)
                        {
                            var relatedService = GetMetadataFromKartkatalogen(metadata.ServiceUuid.ToString());
                            status = relatedService.DistributionDetails != null &&
                                     (relatedService.DistributionDetails.Protocol == "W3C:REST"
                                      || relatedService.DistributionDetails.Protocol == "W3C:WS"
                                      || relatedService.DistributionDetails.Protocol == "OGC:WPS")
                                ? Good
                                : Deficient;
                        }
                        else
                        {
                            status = Deficient;
                        }
                    }
                    else
                    {
                        status = Deficient;
                    }

                }
                else
                {
                    status = Deficient;
                }
            }

            catch (Exception)
            {
                return Deficient;
            }
            if (!string.IsNullOrWhiteSpace(status)) return status;
            status = Deficient;
            return status;
        }

        public string GetDownloadRequirementsStatus(string wfsStatus, string atomFeedStatus)
        {
            if (wfsStatus == Good || atomFeedStatus == Good)
            {
                return Good;
            }
            if (wfsStatus == Useable || atomFeedStatus == Useable)
            {
                return Useable;
            }
            if (wfsStatus == Deficient || atomFeedStatus == Deficient)
            {
                return Deficient;
            }
            if (wfsStatus == Notset || atomFeedStatus == Notset)
            {
                return Notset;
            }
            return Notset;
        }

        private static dynamic GetMetadataFromKartkatalogen(string metadataUuid)
        {
            var metadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + metadataUuid;
            var c = new WebClient { Encoding = System.Text.Encoding.UTF8 };

            var json = c.DownloadString(metadataUrl);

            dynamic metadata = Newtonsoft.Json.Linq.JObject.Parse(json);
            return metadata;
        }

        private string AtomFeed(string uuid)
        {
            var memoryCache = MemoryCache.Default;
            _atomFeedDoc = memoryCache.Get("AtomFeedDoc") as XmlDocument;
            if (_atomFeedDoc == null)
                SetAtomFeed();

            var atomFeed = GetAtomFeed(uuid);
            return atomFeed;
        }

        private string GetAtomFeed(string uuid)
        {
            _nsmgr = new XmlNamespaceManager(_atomFeedDoc.NameTable);
            _nsmgr.AddNamespace("ns", "http://www.w3.org/2005/Atom");
            _nsmgr.AddNamespace("georss", "http://www.georss.org/georss");
            _nsmgr.AddNamespace("inspire_dls", "http://inspire.ec.europa.eu/schemas/inspire_dls/1.0");

            var feed = "";
            var entry = _atomFeedDoc.SelectSingleNode("//ns:feed/ns:entry[inspire_dls:spatial_dataset_identifier_code='" + uuid + "']/ns:link", _nsmgr);
            if (entry == null) return feed;
            feed = entry.InnerText;
            return feed;
        }

        private void SetAtomFeed()
        {
            _atomFeedDoc = new XmlDocument();
            _atomFeedDoc.Load("https://nedlasting.geonorge.no/geonorge/Tjenestefeed.xml");

            MemoryCache memoryCache = MemoryCache.Default;
            memoryCache.Add("AtomFeedDoc", _atomFeedDoc, new DateTimeOffset(DateTime.Now.AddDays(1)));
        }
    }
}