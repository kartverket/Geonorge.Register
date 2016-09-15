using System.Linq;
using GeoNorgeAPI;
using Kartverket.Register.Models;
using www.opengis.net;
using System.Web.Configuration;
using Kartverket.Register.Helpers;
using System;
using System.Collections.Generic;

namespace Kartverket.DOK.Service
{
    public class MetadataService
    {
        public Dataset UpdateDatasetWithMetadata(Dataset dataset, string uuid, Dataset originalDataset, bool dontUpdateDescription)
        {
            SimpleMetadata metadata = FetchMetadata(uuid);
            if (metadata != null)
            {
                dataset.Uuid = uuid;
                dataset.name = metadata.Title;
                dataset.seoname = RegisterUrls.MakeSeoFriendlyString(dataset.name);
                dataset.description = metadata.Abstract;
                dataset.MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + uuid;
                dataset.PresentationRulesUrl = metadata.LegendDescriptionUrl;
                dataset.ProductSheetUrl = metadata.ProductSheetUrl;
                dataset.ProductSpecificationUrl = metadata.ProductSpecificationUrl;
                dataset.datasetthumbnail = FetchThumbnailUrl(metadata);
                dataset.register = originalDataset.register;

                dataset.dokStatusId = originalDataset.dokStatusId;
                dataset.datasetownerId = mapOrganizationNameToId(metadata.ContactOwner != null && metadata.ContactOwner.Organization != null ? metadata.ContactOwner.Organization : "");
                dataset.datasetowner = originalDataset.datasetowner;
                List<SimpleKeyword> keywordsDok = SimpleKeyword.Filter(metadata.Keywords, null, SimpleKeyword.THESAURUS_NATIONAL_THEME);
                dataset.ThemeGroupId = AddTheme(keywordsDok != null && keywordsDok.Count > 0 ? keywordsDok.First().Keyword : "Annen");
                dataset.WmsUrl = originalDataset.WmsUrl;
                dataset.registerId = originalDataset.registerId;
                dataset.dokStatusDateAccepted = originalDataset.dokStatusDateAccepted;
                dataset.Notes = originalDataset.Notes;
                dataset.SpecificUsage = metadata.SpecificUsage;
                dataset.submitterId = originalDataset.submitterId;
                dataset.submitter = originalDataset.submitter;
                dataset.DatasetType = originalDataset.DatasetType;
                if (dontUpdateDescription) dataset.description = originalDataset.description;

                dataset.restricted = false;
                string accessConstraint = "";
                SimpleConstraints constraints = metadata.Constraints;
                if (!string.IsNullOrEmpty(constraints.AccessConstraints))
                    accessConstraint = constraints.AccessConstraints;

                SimpleDistributionDetails distributionDetails = metadata.DistributionDetails;
                if (distributionDetails != null)
                {
                    if (distributionDetails.Protocol != null
                        && distributionDetails.Protocol.ToLower().Contains("wms"))
                    {
                        dataset.WmsUrl = distributionDetails.URL;
                    }
                    else
                    {
                        dataset.DistributionUrl = distributionDetails.URL;

                    }
                    dataset.DistributionArea = distributionDetails.UnitsOfDistribution;

                    if(accessConstraint == "restricted" && distributionDetails.Protocol != null)
                        {
                        dataset.restricted = true;
                        }
                }

                if (metadata.DistributionFormat != null)
                {
                    dataset.DistributionFormat = metadata.DistributionFormat.Name;

                }
            }

            return dataset;
        }

        private string AddTheme(string theme)
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegisterItem = from o in db.DOKThemes
                                           where o.value == theme
                                           select o.value;

            if (queryResultsRegisterItem.ToList().Count == 0) { 
                db.DOKThemes.Add(new DOKTheme { description = theme, value = theme });
                db.SaveChanges();
            }

            db.Dispose();

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
            GeoNorge g = new GeoNorge("", "", WebConfigurationManager.AppSettings["GeoNetworkUrl"]);
            MD_Metadata_Type metadata = g.GetRecordByUuid(uuid);
            return metadata != null ? new SimpleMetadata(metadata) : null;
        }

        public SearchResultsType SearchMetadata(string searchString)
        {
            GeoNorge g = new GeoNorge("", "", WebConfigurationManager.AppSettings["GeoNetworkUrl"]);
            var filters = new object[]
                   {
                    new BinaryLogicOpType()
                    {
                       Items = new object[]
                        {
                        new PropertyIsLikeType
                        {
                            escapeChar = "\\",
                            singleChar = "_",
                            wildCard = "%",
                            PropertyName = new PropertyNameType {Text = new[] {"srv:title"}},
                            Literal = new LiteralType {Text = new[] {searchString}}
                        },
                        new PropertyIsLikeType
                            {
                                PropertyName = new PropertyNameType {Text = new[] {"srv:type"}},
                                Literal = new LiteralType {Text = new[] {"dataset"}}
                            }
                       },
                       ItemsElementName = new ItemsChoiceType22[]
                        {
                            ItemsChoiceType22.PropertyIsLike, ItemsChoiceType22.PropertyIsLike,
                        }
                    }
                   };


            var filterNames = new ItemsChoiceType23[]
            {
                        ItemsChoiceType23.And
            };

            var result = g.SearchWithFilters(filters, filterNames, 1, 200, true);
            return result;
        }

        private Guid mapOrganizationNameToId(string orgname)
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegisterItem = from o in db.Organizations
                                           where o.name == orgname 
                                           select o.systemId;

            Guid ID = queryResultsRegisterItem.FirstOrDefault();

            db.Dispose();

            return ID;
        }

        public string UpdateDatasetsWithMetadata()
        {
            RegisterDbContext db = new RegisterDbContext();

            var queryResultsRegisterItem = from d in db.Datasets
                                           where !string.IsNullOrEmpty(d.Uuid)
                                           select d;

            var datasets = queryResultsRegisterItem.ToList();

            foreach (var dataset in datasets)
            {
                UpdateDatasetWithMetadata(dataset, dataset.Uuid, dataset, false);
                db.Entry(dataset).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            db.Dispose();

            return "updated";
        }
    }
}