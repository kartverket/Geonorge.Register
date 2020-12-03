using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Models.Api
{
    public class MareanoDatasetStatusReport
    {   //Todo check everything
        private const string Good = "good";
        private const string Satisfactory = "satisfactory";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public int NumberOfDatasets { get; set; }
        public DateTime Date { get; set; }

        public NumberOfStatuses NumberOfItemsWithFindable { get; set; }
        public NumberOfStatuses NumberOfItemsWithAccesible { get; set; }
        public NumberOfStatuses NumberOfItemsWithInteroperable { get; set; }
        public NumberOfStatuses NumberOfItemsWithReUseable { get; set; }
        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }
        public NumberOfStatuses NumberOfItemsWithProductSpecification { get; set; }
        public NumberOfStatuses NumberOfItemsWithSosiRequirements { get; set; }
        public NumberOfStatuses NumberOfItemsWithGmlRequirements { get; set; }
        public NumberOfStatuses NumberOfItemsWithWms { get; set; }
        public NumberOfStatuses NumberOfItemsWithWfs { get; set; }
        public NumberOfStatuses NumberOfItemsWithAtomFeed { get; set; }
        public NumberOfStatuses NumberOfItemsWithCommon { get; set; }



        public MareanoDatasetStatusReport(StatusReport statusReport)
        {
            Date = statusReport.Date;
            NumberOfDatasets = statusReport.NumberOfIems();


            NumberOfItemsWithFindable = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithFindable(Good),
                statusReport.NumberOfMareanoDatasetsWithFindable(Useable),
                statusReport.NumberOfMareanoDatasetsWithFindable(Deficient),
                statusReport.NumberOfMareanoDatasetsWithFindable(Notset),
                statusReport.NumberOfMareanoDatasetsWithFindable(Satisfactory)
                );

            NumberOfItemsWithAccesible = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithAccesible(Good),
                statusReport.NumberOfMareanoDatasetsWithAccesible(Useable),
                statusReport.NumberOfMareanoDatasetsWithAccesible(Deficient),
                statusReport.NumberOfMareanoDatasetsWithAccesible(Notset),
                statusReport.NumberOfMareanoDatasetsWithAccesible(Satisfactory)
                );

            NumberOfItemsWithInteroperable = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithInteroperable(Good),
                statusReport.NumberOfMareanoDatasetsWithInteroperable(Useable),
                statusReport.NumberOfMareanoDatasetsWithInteroperable(Deficient),
                statusReport.NumberOfMareanoDatasetsWithInteroperable(Notset),
                statusReport.NumberOfMareanoDatasetsWithInteroperable(Satisfactory)
                );

            NumberOfItemsWithReUseable = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithReUsable(Good),
                statusReport.NumberOfMareanoDatasetsWithReUsable(Useable),
                statusReport.NumberOfMareanoDatasetsWithReUsable(Deficient),
                statusReport.NumberOfMareanoDatasetsWithReUsable(Notset),
                statusReport.NumberOfMareanoDatasetsWithReUsable(Satisfactory)
                );

            NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfMareanoDatasetsWithMetadata(Good),
                statusReport.NumberOfMareanoDatasetsWithMetadata(Useable),
                statusReport.NumberOfMareanoDatasetsWithMetadata(Deficient),
                statusReport.NumberOfMareanoDatasetsWithMetadata(Notset),
                statusReport.NumberOfMareanoDatasetsWithMetadata(Satisfactory)
                );

            NumberOfItemsWithProductSpecification = new NumberOfStatuses(
                statusReport.NumberOfMareanoDatasetsWithProductSpecification(Good),
                statusReport.NumberOfMareanoDatasetsWithProductSpecification(Useable),
                statusReport.NumberOfMareanoDatasetsWithProductSpecification(Deficient),
                statusReport.NumberOfMareanoDatasetsWithProductSpecification(Notset),
                statusReport.NumberOfMareanoDatasetsWithProductSpecification(Satisfactory));

            NumberOfItemsWithSosiRequirements = new NumberOfStatuses(
                statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Good),
                statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Useable),
                statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Deficient),
                statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Notset),
                statusReport.NumberOfMareanoDatasetsWithSosiRequirements(Satisfactory));

            NumberOfItemsWithGmlRequirements = new NumberOfStatuses(
                statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Good),
                statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Useable),
                statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Deficient),
                statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Notset),
                statusReport.NumberOfMareanoDatasetsWithGmlRequirements(Satisfactory));

            NumberOfItemsWithWms = new NumberOfStatuses(
                statusReport.NumberOfMareanoDatasetsWithWms(Good),
                statusReport.NumberOfMareanoDatasetsWithWms(Useable),
                statusReport.NumberOfMareanoDatasetsWithWms(Deficient),
                statusReport.NumberOfMareanoDatasetsWithWms(Notset),
                statusReport.NumberOfMareanoDatasetsWithWms(Satisfactory));

            NumberOfItemsWithWfs = new NumberOfStatuses(
                statusReport.NumberOfMareanoDatasetsWithWfs(Good),
                statusReport.NumberOfMareanoDatasetsWithWfs(Useable),
                statusReport.NumberOfMareanoDatasetsWithWfs(Deficient),
                statusReport.NumberOfMareanoDatasetsWithWfs(Notset),
                statusReport.NumberOfMareanoDatasetsWithWfs(Satisfactory));

            NumberOfItemsWithAtomFeed = new NumberOfStatuses(
                statusReport.NumberOfMareanoDatasetsWithAtomFeed(Good),
                statusReport.NumberOfMareanoDatasetsWithAtomFeed(Useable),
                statusReport.NumberOfMareanoDatasetsWithAtomFeed(Deficient),
                statusReport.NumberOfMareanoDatasetsWithAtomFeed(Notset),
                statusReport.NumberOfMareanoDatasetsWithAtomFeed(Satisfactory)
                );

            NumberOfItemsWithCommon = new NumberOfStatuses(
                statusReport.NumberOfItemsWithCommon(Good),
                statusReport.NumberOfItemsWithCommon(Useable),
                statusReport.NumberOfItemsWithCommon(Deficient),
                statusReport.NumberOfItemsWithCommon(Notset),
                statusReport.NumberOfItemsWithCommon(Satisfactory));
        }
    }
}