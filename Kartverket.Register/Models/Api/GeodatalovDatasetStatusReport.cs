using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Models.Api
{
    public class GeodatalovDatasetStatusReport
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public int NumberOfDatasets { get; set; }
        public DateTime Date { get; set; }

        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }
        public NumberOfStatuses NumberOfItemsWithProductSpecification { get; set; }
        public NumberOfStatuses NumberOfItemsWithSosiRequirements { get; set; }
        public NumberOfStatuses NumberOfItemsWithGmlRequirements { get; set; }
        public NumberOfStatuses NumberOfItemsWithWms { get; set; }
        public NumberOfStatuses NumberOfItemsWithWfs { get; set; }
        public NumberOfStatuses NumberOfItemsWithAtomFeed { get; set; }
        public NumberOfStatuses NumberOfItemsWithCommon { get; set; }
        public int NumberOfItemsWithInspireTheme { get; set; }
        public int NumberOfItemsWithoutInspireTheme { get; set; }
        public int NumberOfItemsWithDok { get; set; }
        public int NumberOfItemsWithoutDok { get; set; }
        public int NumberOfItemsWithNationalDatasets { get; set; }
        public int NumberOfItemsWithoutNationalDatasets { get; set; }
        public int NumberOfItemsWithPlan { get; set; }
        public int NumberOfItemsWithoutPlan { get; set; }
        public int NumberOfItemsWithGeodatalov { get; set; }
        public int NumberOfItemsWithoutGeodatalov { get; set; }



        public GeodatalovDatasetStatusReport(StatusReport statusReport)
        {
            Date = statusReport.Date;
            NumberOfDatasets = statusReport.NumberOfIems();

            NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfGeodatalovDatasetsWithMetadata(Good),
                statusReport.NumberOfGeodatalovDatasetsWithMetadata(Useable),
                statusReport.NumberOfGeodatalovDatasetsWithMetadata(Deficient),
                statusReport.NumberOfGeodatalovDatasetsWithMetadata(Notset));

            NumberOfItemsWithProductSpecification = new NumberOfStatuses(
                statusReport.NumberOfGeodatalovDatasetsWithProductSpecification(Good),
                statusReport.NumberOfGeodatalovDatasetsWithProductSpecification(Useable),
                statusReport.NumberOfGeodatalovDatasetsWithProductSpecification(Deficient),
                statusReport.NumberOfGeodatalovDatasetsWithProductSpecification(Notset));

            NumberOfItemsWithSosiRequirements = new NumberOfStatuses(
                statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Good),
                statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Useable),
                statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Deficient),
                statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements(Notset));

            NumberOfItemsWithGmlRequirements = new NumberOfStatuses(
                statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Good),
                statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Useable),
                statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Deficient),
                statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements(Notset));

            NumberOfItemsWithWms = new NumberOfStatuses(
                statusReport.NumberOfGeodatalovDatasetsWithWms(Good),
                statusReport.NumberOfGeodatalovDatasetsWithWms(Useable),
                statusReport.NumberOfGeodatalovDatasetsWithWms(Deficient),
                statusReport.NumberOfGeodatalovDatasetsWithWms(Notset));

            NumberOfItemsWithWfs = new NumberOfStatuses(
                statusReport.NumberOfGeodatalovDatasetsWithWfs(Good),
                statusReport.NumberOfGeodatalovDatasetsWithWfs(Useable),
                statusReport.NumberOfGeodatalovDatasetsWithWfs(Deficient),
                statusReport.NumberOfGeodatalovDatasetsWithWfs(Notset));

            NumberOfItemsWithAtomFeed = new NumberOfStatuses(
                statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Good),
                statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Useable),
                statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Deficient),
                statusReport.NumberOfGeodatalovDatasetsWithAtomFeed(Notset));

            NumberOfItemsWithCommon = new NumberOfStatuses(
                statusReport.NumberOfItemsWithCommon(Good),
                statusReport.NumberOfItemsWithCommon(Useable),
                statusReport.NumberOfItemsWithCommon(Deficient),
                statusReport.NumberOfItemsWithCommon(Notset));

            NumberOfItemsWithInspireTheme = statusReport.NumberOfGeodatalovDatasetsWithInspireTheme();
            NumberOfItemsWithoutInspireTheme = NumberOfDatasets - NumberOfItemsWithInspireTheme;

            NumberOfItemsWithDok = statusReport.NumberOfGeodatalovDatasetsWithDok();
            NumberOfItemsWithoutDok = NumberOfDatasets - NumberOfItemsWithDok;

            NumberOfItemsWithNationalDatasets = statusReport.NumberOfGeodatalovDatasetsWithNationalDataset();
            NumberOfItemsWithoutNationalDatasets = NumberOfDatasets - NumberOfItemsWithNationalDatasets;

            NumberOfItemsWithPlan = statusReport.NumberOfGeodatalovDatasetsWithPlan();
            NumberOfItemsWithoutPlan = NumberOfDatasets - NumberOfItemsWithPlan;

            NumberOfItemsWithGeodatalov = statusReport.NumberOfGeodatalovDatasetsWithGeodatalov();
            NumberOfItemsWithoutGeodatalov = NumberOfDatasets - NumberOfItemsWithGeodatalov;
        }
    }
}