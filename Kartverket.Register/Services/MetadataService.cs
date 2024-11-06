using System.Linq;
using GeoNorgeAPI;
using Kartverket.Register.Models;
using www.opengis.net;
using System.Web.Configuration;
using Kartverket.Register.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Models.Translations;

namespace Kartverket.DOK.Service
{
    public class MetadataService
    {
        private readonly RegisterDbContext _dbContext;
        public MetadataService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Dataset UpdateDatasetWithMetadata(Dataset dataset, string uuid, bool dontUpdateDescription)
        {
            Dataset metadata = FetchDatasetFromKartkatalogen(uuid);
            if (metadata != null)
            {
                dataset.Uuid = uuid;
                dataset.name = metadata.name;
                dataset.seoname = RegisterUrls.MakeSeoFriendlyString(dataset.name);
                dataset.description = metadata.description;
                dataset.MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + uuid;
                dataset.PresentationRulesUrl = metadata.PresentationRulesUrl;
                dataset.ProductSheetUrl = metadata.ProductSheetUrl;
                dataset.ProductSpecificationUrl = metadata.ProductSpecificationUrl;
                dataset.datasetthumbnail = metadata.datasetthumbnail;

                dataset.datasetownerId = metadata.datasetownerId;
                dataset.ThemeGroupId = metadata.ThemeGroupId;
                dataset.UuidService = metadata.UuidService;
                dataset.WmsUrl = metadata.WmsUrl;
                dataset.SpecificUsage = metadata.SpecificUsage;

                dataset.restricted = metadata.restricted;

                dataset.DistributionUrl = metadata.DistributionUrl;
                dataset.DistributionArea = metadata.DistributionArea;

                if (metadata.DistributionFormat != null)
                {
                    dataset.DistributionFormat = metadata.DistributionFormat;
                }
               
                dataset.Translations.Where(t => t.RegisterItemId != Guid.Empty).ToList().ForEach(x => _dbContext.Entry(x).State = EntityState.Deleted);
                dataset.Translations = metadata.Translations;
            }

            return dataset;
        }

        private string AddTheme(string theme)
        {

            var queryResultsRegisterItem = from o in _dbContext.DOKThemes
                                           where o.value == theme
                                           select o.value;

            if (queryResultsRegisterItem.ToList().Count == 0)
            {
                _dbContext.DOKThemes.Add(new DOKTheme { description = theme, value = theme });
                _dbContext.SaveChanges();
            }

            return theme;
        }

        private static string FetchThumbnailUrl(SimpleMetadata metadata)
        {
            string thumbnailUrl = null;
            if (metadata.Thumbnails != null)
            {
                foreach (var thumbnail in metadata.Thumbnails)
                {
                    if (thumbnail.Type == "large_thumbnail")
                    {
                        thumbnailUrl = thumbnail.URL;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(thumbnailUrl))
                {
                    var thumbnail = metadata.Thumbnails.FirstOrDefault();
                    if (thumbnail != null)
                    {
                        thumbnailUrl = thumbnail.URL;
                    }
                }

                if (thumbnailUrl != null && !thumbnailUrl.StartsWith("http"))
                {
                    thumbnailUrl = "https://www.geonorge.no/geonetwork/srv/nor/resources.get?uuid=" + metadata.Uuid + "&access=public&fname=" + thumbnailUrl;
                }
            }
            return thumbnailUrl;
        }

        public static SimpleMetadata FetchMetadata(string uuid)
        {
            try
            {
                GeoNorge g = new GeoNorge("", "", WebConfigurationManager.AppSettings["GeoNetworkUrl"]);
                MD_Metadata_Type metadata = g.GetRecordByUuid(uuid);
                return metadata != null ? new SimpleMetadata(metadata) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Dataset FetchDatasetFromKartkatalogen(string uuid)
        {
            Dataset metadata = new Dataset();

            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            try
            {
                var json = c.DownloadString(url);

                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    metadata.name = data.Title;
                    metadata.description = data.Abstract;
                    metadata.PresentationRulesUrl = data.LegendDescriptionUrl;
                    metadata.ProductSheetUrl = data.ProductSheetUrl;
                    metadata.ProductSpecificationUrl = data.ProductSpecificationUrl;
                    metadata.SpecificUsage = data.SpecificUsage;
                    var thumbnails = data.Thumbnails;
                    if (thumbnails != null && thumbnails.Count > 0)
                    {
                        metadata.datasetthumbnail = thumbnails[0].URL.Value;
                    }

                    metadata.datasetownerId = mapOrganizationNameToId(data.ContactOwner != null && data.ContactOwner.Organization != null ? data.ContactOwner.Organization.Value : "Kartverket");
                    metadata.ThemeGroupId = AddTheme(data.KeywordsNationalTheme != null && data.KeywordsNationalTheme.Count > 0 ? data.KeywordsNationalTheme[0].KeywordValue.Value : "Annen");

                    if (data.ServiceUuid != null)
                        metadata.UuidService = data.ServiceUuid;

                    if (data.ServiceDistributionUrlForDataset != null)
                        metadata.WmsUrl = data.ServiceDistributionUrlForDataset;

                }

                if (data.DistributionDetails != null)
                    metadata.DistributionUrl = data.DistributionDetails.URL;

                if (data.UnitsOfDistribution != null)
                    metadata.DistributionArea = data.UnitsOfDistribution.Value;

                var distributionFormat = data.DistributionFormat;
                if (distributionFormat != null)
                {
                    if (distributionFormat.Name != null)
                        metadata.DistributionFormat = distributionFormat.Name.Value;
                }

                metadata.restricted = false;

                var constraints = data.Constraints;

                if (constraints != null)
                {
                    string accessConstraint = constraints.AccessConstraints.Value;
                    if (!string.IsNullOrEmpty(accessConstraint) && accessConstraint == "Beskyttet")
                    {
                        metadata.restricted = true;
                    }
                }

                var englishTitle = data.EnglishTitle;
                var englishAbstract = data.EnglishAbstract;
                string keywordsNationalThemeEnglish = "";
                if (data.KeywordsNationalTheme.Count > 0)
                    keywordsNationalThemeEnglish = data.KeywordsNationalTheme[0].EnglishKeyword;
                metadata.Translations.Add(new DatasetTranslation
                {
                    CultureName = Culture.EnglishCode,
                    Name = englishTitle,
                    Description = englishAbstract,
                    ThemeGroupId = keywordsNationalThemeEnglish
                });

                metadata.AddMissingTranslations();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }

            return metadata;
        }

        public InspireDataset FetchInspireDatasetFromKartkatalogen(string uuid)
        {
            var inspireDataset = new InspireDataset();
            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
            var c = new System.Net.WebClient { Encoding = System.Text.Encoding.UTF8 };
            try
            {
                var json = c.DownloadString(url);

                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    inspireDataset.Name = data.Title;
                    inspireDataset.Description = data.Abstract;
                    inspireDataset.PresentationRulesUrl = data.LegendDescriptionUrl;
                    inspireDataset.ProductSheetUrl = data.ProductSheetUrl;
                    inspireDataset.ProductSpecificationUrl = data.ProductSpecificationUrl;
                    inspireDataset.SpecificUsage = data.SpecificUsage;
                    inspireDataset.Uuid = data.Uuid;
                    inspireDataset.MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + inspireDataset.Uuid;
                    var thumbnails = data.Thumbnails;
                    if (thumbnails != null && thumbnails.Count > 0)
                    {
                        inspireDataset.DatasetThumbnail = thumbnails[0].URL.Value;
                    }

                    inspireDataset.OwnerId = mapOrganizationNameToId(
                        data.ContactOwner != null && data.ContactOwner.Organization != null
                            ? data.ContactOwner.Organization.Value
                            : "Kartverket");
                    inspireDataset.ThemeGroupId = AddTheme(data.KeywordsNationalTheme != null && data.KeywordsNationalTheme.Count > 0 ? data.KeywordsNationalTheme[0].KeywordValue.Value : "Annen");

                    inspireDataset.InspireThemes = GetInspireThemes(data.KeywordsInspire);

                    if (data.ServiceUuid != null)
                        inspireDataset.UuidService = data.ServiceUuid;
                    if (data.ServiceDistributionUrlForDataset != null)
                        inspireDataset.WmsUrl = data.ServiceDistributionUrlForDataset;

                    if (data.DistributionDetails != null)
                        inspireDataset.DistributionUrl = data.DistributionDetails.URL;

                    if (data.UnitsOfDistribution != null)
                        inspireDataset.DistributionArea = data.UnitsOfDistribution.Value;

                    var distributionFormat = data.DistributionFormat;
                    if (distributionFormat != null)
                    {
                        if (distributionFormat.Name != null)
                            inspireDataset.DistributionFormat = distributionFormat.Name.Value;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }

            return inspireDataset;
        }

        private ICollection<CodelistValue> GetInspireThemes(dynamic keywordsInspire)
        {
            var inspireTheams = new List<CodelistValue>();

            if (keywordsInspire != null)
            {
                foreach (var item in keywordsInspire)
                {
                    CodelistValue inspireTheme = GetInspireThemeAsCodelistValue(item.KeywordValue.ToString());
                    if (inspireTheme != null)
                    {
                        inspireTheams.Add(inspireTheme);
                    }
                }
            }
            return inspireTheams;
        }

        private Guid? GetInspireThemeId(string code)
        {
            var queryResultsRegisterItem = from o in _dbContext.CodelistValues
                                           where o.register.name == "Inspiretema" &&
                                           o.value == code
                                           select o.systemId;

            if (queryResultsRegisterItem.Any())
            {
                return queryResultsRegisterItem.FirstOrDefault();
            }
            return null;
        }

        private CodelistValue GetInspireThemeAsCodelistValue(string code)
        {
            var queryResultsRegisterItem = from o in _dbContext.CodelistValues
                                           where o.register.name == "Inspiretema" &&
                                           (o.value == code || o.name == code)
                                           select o;


            return queryResultsRegisterItem.FirstOrDefault();
        }

        public GeodatalovDataset FetchGeodatalovDatasetFromKartkatalogen(string uuid)
        {
            var geodatalovDataset = new GeodatalovDataset();
            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
            var c = new System.Net.WebClient { Encoding = System.Text.Encoding.UTF8 };
            try
            {
                var json = c.DownloadString(url);

                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    geodatalovDataset.Name = data.Title;
                    geodatalovDataset.Description = data.Abstract;
                    geodatalovDataset.PresentationRulesUrl = data.LegendDescriptionUrl;
                    geodatalovDataset.ProductSheetUrl = data.ProductSheetUrl;
                    geodatalovDataset.ProductSpecificationUrl = data.ProductSpecificationUrl;
                    geodatalovDataset.SpecificUsage = data.SpecificUsage;
                    geodatalovDataset.Uuid = data.Uuid;
                    geodatalovDataset.MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + geodatalovDataset.Uuid;
                    var thumbnails = data.Thumbnails;
                    if (thumbnails != null && thumbnails.Count > 0)
                    {
                        geodatalovDataset.DatasetThumbnail = thumbnails[0].URL.Value;
                    }

                    geodatalovDataset.OwnerId = mapOrganizationNameToId(
                        data.ContactOwner != null && data.ContactOwner.Organization != null
                            ? data.ContactOwner.Organization.Value
                            : "Kartverket");
                    geodatalovDataset.ThemeGroupId =
                        AddTheme(data.KeywordsNationalTheme != null && data.KeywordsNationalTheme.Count > 0
                            ? data.KeywordsNationalTheme[0].KeywordValue.Value
                            : "Annen");

                    if (data.ServiceUuid != null) geodatalovDataset.UuidService = data.ServiceUuid;
                    if (data.ServiceDistributionUrlForDataset != null)
                        geodatalovDataset.WmsUrl = data.ServiceDistributionUrlForDataset;

                    if (data.DistributionDetails != null)
                        geodatalovDataset.DistributionUrl = data.DistributionDetails.URL;

                    if (data.UnitsOfDistribution != null)
                        geodatalovDataset.DistributionArea = data.UnitsOfDistribution.Value;

                    var distributionFormat = data.DistributionFormat;
                    if (distributionFormat != null)
                    {
                        if (distributionFormat.Name != null)
                            geodatalovDataset.DistributionFormat = distributionFormat.Name.Value;
                    }

                    foreach (var keyword in data.KeywordsNationalInitiative)
                    {
                        if (keyword.KeywordValue == "Det offentlige kartgrunnlaget")
                        {
                            geodatalovDataset.Dok = true;
                        }
                        if (keyword.KeywordValue == "geodataloven")
                        {
                            geodatalovDataset.Geodatalov = true;
                        }
                        if (keyword.KeywordValue == "Norge digitalt")
                        {
                            geodatalovDataset.NationalDataset = true;
                        }
                        if (keyword.KeywordValue == "Inspire")
                        {
                            geodatalovDataset.InspireTheme = true;
                        }
                        if (keyword.KeywordValue == "arealplanerPBL")
                        {
                            geodatalovDataset.Plan = true;
                        }
                        if (keyword.KeywordValue == "Mareano")
                        {
                            geodatalovDataset.Mareano = true;
                        }
                        if (keyword.KeywordValue == "ØkologiskGrunnkart" || keyword.KeywordValue == "Økologisk grunnkart")
                        {
                            geodatalovDataset.EcologicalBaseMap = true;
                        }
                        if (keyword.KeywordValue == "modellbaserteVegprosjekter")
                        {
                            geodatalovDataset.ModellbaserteVegprosjekter = true;
                        }
                    }

                    if (geodatalovDataset.NationalDataset)
                    {
                        foreach (var theme in data.KeywordsTheme)
                        {
                            string keywordValue = theme.KeywordValue;
                            if (!string.IsNullOrEmpty(keywordValue) && (keywordValue.ToLower() == "høydedata" || keywordValue.ToLower() == "flyfoto") )
                                if (!geodatalovDataset.Geodatalov)
                                    return null;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }

            return geodatalovDataset;
        }

        public MareanoDataset FetchMareanoDatasetFromKartkatalogen(string uuid)
        {
            var mareanoDataset = new MareanoDataset();
            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
            var c = new System.Net.WebClient { Encoding = System.Text.Encoding.UTF8 };
            try
            {
                var json = c.DownloadString(url);

                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    mareanoDataset.Name = data.Title;
                    mareanoDataset.Description = data.Abstract;
                    mareanoDataset.PresentationRulesUrl = data.LegendDescriptionUrl;
                    mareanoDataset.ProductSheetUrl = data.ProductSheetUrl;
                    mareanoDataset.ProductSpecificationUrl = data.ProductSpecificationUrl;
                    mareanoDataset.SpecificUsage = data.SpecificUsage;
                    mareanoDataset.Uuid = data.Uuid;
                    mareanoDataset.MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + mareanoDataset.Uuid;
                    var thumbnails = data.Thumbnails;
                    if (thumbnails != null && thumbnails.Count > 0)
                    {
                        mareanoDataset.DatasetThumbnail = thumbnails[0].URL.Value;
                    }

                    mareanoDataset.OwnerId = mapOrganizationNameToId(
                        data.ContactOwner != null && data.ContactOwner.Organization != null
                            ? data.ContactOwner.Organization.Value
                            : "Kartverket");
                    mareanoDataset.ThemeGroupId =
                        AddTheme(data.KeywordsNationalTheme != null && data.KeywordsNationalTheme.Count > 0
                            ? data.KeywordsNationalTheme[0].KeywordValue.Value
                            : "Annen");

                    if (data.ServiceUuid != null) mareanoDataset.UuidService = data.ServiceUuid;
                    if (data.ServiceDistributionUrlForDataset != null)
                        mareanoDataset.WmsUrl = data.ServiceDistributionUrlForDataset;

                    if (data.DistributionDetails != null)
                        mareanoDataset.DistributionUrl = data.DistributionDetails.URL;

                    if (data.UnitsOfDistribution != null)
                        mareanoDataset.DistributionArea = data.UnitsOfDistribution.Value;

                    var distributionFormat = data.DistributionFormat;
                    if (distributionFormat != null)
                    {
                        if (distributionFormat.Name != null)
                            mareanoDataset.DistributionFormat = distributionFormat.Name.Value;
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }

            return mareanoDataset;
        }

        public FairDataset FetchFairDatasetFromKartkatalogen(string uuid)
        {
            var fairDataset = new FairDataset();
            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
            var c = new System.Net.WebClient { Encoding = System.Text.Encoding.UTF8 };
            try
            {
                var json = c.DownloadString(url);

                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    fairDataset.Name = data.Title;
                    fairDataset.Description = data.Abstract;
                    fairDataset.PresentationRulesUrl = data.LegendDescriptionUrl;
                    fairDataset.ProductSheetUrl = data.ProductSheetUrl;
                    fairDataset.ProductSpecificationUrl = data.ProductSpecificationUrl;
                    fairDataset.SpecificUsage = data.SpecificUsage;
                    fairDataset.Uuid = data.Uuid;
                    fairDataset.MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + fairDataset.Uuid;
                    var thumbnails = data.Thumbnails;
                    if (thumbnails != null && thumbnails.Count > 0)
                    {
                        fairDataset.DatasetThumbnail = thumbnails[0].URL.Value;
                    }

                    fairDataset.OwnerId = mapOrganizationNameToId(
                        data.ContactOwner != null && data.ContactOwner.Organization != null
                            ? data.ContactOwner.Organization.Value
                            : "Kartverket");
                    fairDataset.ThemeGroupId =
                        AddTheme(data.KeywordsNationalTheme != null && data.KeywordsNationalTheme.Count > 0
                            ? data.KeywordsNationalTheme[0].KeywordValue.Value
                            : "Annen");

                    if (data.ServiceUuid != null) fairDataset.UuidService = data.ServiceUuid;
                    if (data.ServiceDistributionUrlForDataset != null)
                        fairDataset.WmsUrl = data.ServiceDistributionUrlForDataset;

                    if (data.DistributionDetails != null)
                        fairDataset.DistributionUrl = data.DistributionDetails.URL;

                    if (data.UnitsOfDistribution != null)
                        fairDataset.DistributionArea = data.UnitsOfDistribution.Value;

                    var distributionFormat = data.DistributionFormat;
                    if (distributionFormat != null)
                    {
                        if (distributionFormat.Name != null)
                            fairDataset.DistributionFormat = distributionFormat.Name.Value;
                    }

                    fairDataset.FairDatasetTypes = new List<FairDatasetType>();

                    if (data.KeywordsNationalInitiative != null)
                    {
                        foreach (var keyword in data.KeywordsNationalInitiative)
                        {
                            if (keyword.KeywordValue == "Det offentlige kartgrunnlaget")
                            {
                                fairDataset.FairDatasetTypes.Add(new FairDatasetType { Label = "DOK", Description = "Det offentlige kartgrunnlaget" });
                            }
                            else if (keyword.KeywordValue == "Mareano")
                            {
                                fairDataset.FairDatasetTypes.Add(new FairDatasetType { Label = "Mareano", Description = "Mareano" });
                            }
                            else if (keyword.KeywordValue == "MarineGrunnkart")
                            {
                                fairDataset.FairDatasetTypes.Add(new FairDatasetType { Label = "MarineGrunnkart", Description = "Marine grunnkart" });
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }

            return fairDataset;
        }

        public SearchResultsType SearchMetadata(string searchString)
        {
            GeoNorge g = new GeoNorge("", "", WebConfigurationManager.AppSettings["GeoNetworkUrl"]);
            var filters = new object[]
                   {
                        new PropertyIsLikeType
                        {
                            escapeChar = "\\",
                            singleChar = "_",
                            wildCard = "%",
                            PropertyName = new PropertyNameType {Text = new[] {"srv:title"}},
                            Literal = new LiteralType {Text = new[] {searchString}}
                    }
                   };


            var filterNames = new ItemsChoiceType23[]
            {
                        ItemsChoiceType23.PropertyIsLike,
            };

            var result = g.SearchWithFilters(filters, filterNames, 1, 200, true);
            return result;
        }

        private Guid mapOrganizationNameToId(string orgname)
        {
            var queryResultsRegisterItem = from o in _dbContext.Organizations
                                           where o.name == orgname
                                           select o.systemId;

            Guid ID = queryResultsRegisterItem.FirstOrDefault();
            if (ID == Guid.Empty)
            {
                ID = Organization.GetDefaultOrganizationId();
            }

            return ID;
        }

        public string UpdateDatasetsWithMetadata()
        {
            var queryResultsRegisterItem = from d in _dbContext.Datasets
                                           where !string.IsNullOrEmpty(d.Uuid)
                                           select d;

            var datasets = queryResultsRegisterItem.ToList();

            foreach (var dataset in datasets)
            {
                UpdateDatasetWithMetadata(dataset, dataset.Uuid, false);
                _dbContext.Entry(dataset).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges();
            }

            return "updated";
        }

        public List<MetadataItemViewModel> SearchMetadataFromKartkatalogen(string searchString)
        {
            var searchResult = SearchMetadata(searchString);
            var result = new List<MetadataItemViewModel>();

            if (searchResult != null && searchResult.numberOfRecordsMatched != "0")
            {
                result.AddRange(searchResult.Items.Select(t => new MetadataItemViewModel
                {
                    Uuid = ((www.opengis.net.DCMIRecordType)(t)).Items[0].Text[0],
                    Title = ((www.opengis.net.DCMIRecordType)(t)).Items[2].Text[0]
                }));
            }
            return result;
        }

        public InspireDataService FetchInspireDataServiceFromKartkatalogen(string uuid)
        {
            var inspireDataService = new InspireDataService();
            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
            var c = new System.Net.WebClient { Encoding = System.Text.Encoding.UTF8 };
            try
            {
                var json = c.DownloadString(url);

                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    inspireDataService.Name = data.Title;
                    inspireDataService.Description = data.Abstract;
                    inspireDataService.Uuid = data.Uuid;

                    inspireDataService.OwnerId = mapOrganizationNameToId(
                        data.ContactOwner != null && data.ContactOwner.Organization != null
                            ? data.ContactOwner.Organization.Value
                            : "Kartverket");

                    inspireDataService.InspireThemes = GetInspireThemes(data.KeywordsInspire);

                    inspireDataService.Url = data.DistributionUrl;
                    inspireDataService.GetServiceType(data.ServiceType.ToString());

                    inspireDataService.InspireDataType = data.DistributionDetails.ProtocolName;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }

            return inspireDataService;

        }

        private string GetInspireTheme(string code)
        {
            var queryResultsRegisterItem = from o in _dbContext.CodelistValues
                                           where o.register.name == "Inspiretema" &&
                                           o.value == code
                                           select o.name;

            return queryResultsRegisterItem.FirstOrDefault();

        }

        private bool IsNetworkService(string serviceType)
        {
            return serviceType == "view" || serviceType == "download";
        }
    }
}