using System.Linq;
using GeoNorgeAPI;
using Kartverket.Register.Models;
using www.opengis.net;
using System.Web.Configuration;
using Kartverket.Register.Helpers;
using System;

namespace Kartverket.DOK.Service
{
    public class MetadataService
    {
        public void UpdateDatasetWithMetadata(Dataset dataset, string uuid, Dataset originalDataset, bool dontUpdateDescription)
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
                dataset.datasetownerId = originalDataset.datasetownerId;
                dataset.datasetowner = originalDataset.datasetowner;
                dataset.ThemeGroupId = originalDataset.ThemeGroupId;
                dataset.WmsUrl = originalDataset.WmsUrl;
                dataset.registerId = originalDataset.registerId;
                dataset.dokStatusDateAccepted = originalDataset.dokStatusDateAccepted;

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

                    if(accessConstraint == "restricted" && distributionDetails.Protocol != null
                        && distributionDetails.Protocol =="GEONORGE:OFFLINE" )
                        {
                        dataset.restricted = true;
                        }
                }

                if (metadata.DistributionFormat != null)
                {
                    dataset.DistributionFormat = metadata.DistributionFormat.Name;

                }
            }
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
    }
}