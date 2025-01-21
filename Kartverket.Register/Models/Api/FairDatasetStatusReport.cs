using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Models.Api
{
    public class FairDatasetStatusReport
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
        public NumberOfStatuses NumberOfItemsWithProductSheet { get; set; }
        public NumberOfStatuses NumberOfItemsWithPresentationRules { get; set; }
        public NumberOfStatuses NumberOfItemsWithSosiRequirements { get; set; }
        public NumberOfStatuses NumberOfItemsWithGmlRequirements { get; set; }
        public NumberOfStatuses NumberOfItemsWithWms { get; set; }
        public NumberOfStatuses NumberOfItemsWithWfs { get; set; }
        public NumberOfStatuses NumberOfItemsWithAtomFeed { get; set; }
        public NumberOfStatuses NumberOfItemsWithCommon { get; set; }
        public double PercentageOfDatasetsWithFindable { get; set; }
        public double PercentageOfDatasetsWithAccesible { get; set; }
        public double PercentageOfDatasetsWithInteroperable { get; set; }
        public double PercentageOfDatasetsWithReUseable { get; set; }

        public double PercentageOfDatasetsWithFindableGood { get; private set; }
        public double PercentageOfDatasetsWithFindableSatisfactory { get; private set; }
        public double PercentageOfDatasetsWithFindableUseable { get; private set; }
        public double PercentageOfDatasetsWithFindableDeficient { get; private set; }
        public double PercentageOfDatasetsWithAccesibleGood { get; private set; }
        public double PercentageOfDatasetsWithAccesibleSatisfactory { get; private set; }
        public double PercentageOfDatasetsWithAccesibleUseable { get; private set; }
        public double PercentageOfDatasetsWithAccesibleDeficient { get; private set; }
        public double PercentageOfDatasetsWithInteroperableGood { get; private set; }
        public double PercentageOfDatasetsWithInteroperableSatisfactory { get; private set; }
        public double PercentageOfDatasetsWithInteroperableUseable { get; private set; }
        public double PercentageOfDatasetsWithInteroperableDeficient { get; private set; }
        public double PercentageOfDatasetsWithReUseableGood { get; private set; }
        public double PercentageOfDatasetsWithReUseableSatisfactory { get; private set; }
        public double PercentageOfDatasetsWithReUseableUseable { get; private set; }
        public double PercentageOfDatasetsWithReUseableDeficient { get; private set; }

        public FairDatasetStatusReport(StatusReport statusReport)
        {
            Date = statusReport.Date;
            NumberOfDatasets = statusReport.NumberOfIems();


            NumberOfItemsWithFindable = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithFindable(Good),
                statusReport.NumberOfFairDatasetsWithFindable(Useable),
                statusReport.NumberOfFairDatasetsWithFindable(Deficient),
                statusReport.NumberOfFairDatasetsWithFindable(Notset),
                statusReport.NumberOfFairDatasetsWithFindable(Satisfactory)
                );

            NumberOfItemsWithAccesible = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithAccesible(Good),
                statusReport.NumberOfFairDatasetsWithAccesible(Useable),
                statusReport.NumberOfFairDatasetsWithAccesible(Deficient),
                statusReport.NumberOfFairDatasetsWithAccesible(Notset),
                statusReport.NumberOfFairDatasetsWithAccesible(Satisfactory)
                );

            NumberOfItemsWithInteroperable = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithInteroperable(Good),
                statusReport.NumberOfFairDatasetsWithInteroperable(Useable),
                statusReport.NumberOfFairDatasetsWithInteroperable(Deficient),
                statusReport.NumberOfFairDatasetsWithInteroperable(Notset),
                statusReport.NumberOfFairDatasetsWithInteroperable(Satisfactory)
                );

            NumberOfItemsWithReUseable = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithReUsable(Good),
                statusReport.NumberOfFairDatasetsWithReUsable(Useable),
                statusReport.NumberOfFairDatasetsWithReUsable(Deficient),
                statusReport.NumberOfFairDatasetsWithReUsable(Notset),
                statusReport.NumberOfFairDatasetsWithReUsable(Satisfactory)
                );

            NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfFairDatasetsWithMetadata(Good),
                statusReport.NumberOfFairDatasetsWithMetadata(Useable),
                statusReport.NumberOfFairDatasetsWithMetadata(Deficient),
                statusReport.NumberOfFairDatasetsWithMetadata(Notset),
                statusReport.NumberOfFairDatasetsWithMetadata(Satisfactory)
                );

            NumberOfItemsWithProductSpecification = new NumberOfStatuses(
                statusReport.NumberOfFairDatasetsWithProductSpecification(Good),
                statusReport.NumberOfFairDatasetsWithProductSpecification(Useable),
                statusReport.NumberOfFairDatasetsWithProductSpecification(Deficient),
                statusReport.NumberOfFairDatasetsWithProductSpecification(Notset),
                statusReport.NumberOfFairDatasetsWithProductSpecification(Satisfactory));

            NumberOfItemsWithProductSheet = new NumberOfStatuses(
                statusReport.NumberOfFairDatasetsWithProductSheet(Good),
                statusReport.NumberOfFairDatasetsWithProductSheet(Useable),
                statusReport.NumberOfFairDatasetsWithProductSheet(Deficient),
                statusReport.NumberOfFairDatasetsWithProductSheet(Notset),
                statusReport.NumberOfFairDatasetsWithProductSheet(Satisfactory));

            NumberOfItemsWithPresentationRules = new NumberOfStatuses(
                statusReport.NumberOfFairDatasetsWithPresentationRules(Good),
                statusReport.NumberOfFairDatasetsWithPresentationRules(Useable),
                statusReport.NumberOfFairDatasetsWithPresentationRules(Deficient),
                statusReport.NumberOfFairDatasetsWithPresentationRules(Notset),
                statusReport.NumberOfFairDatasetsWithPresentationRules(Satisfactory));

            NumberOfItemsWithSosiRequirements = new NumberOfStatuses(
                statusReport.NumberOfFairDatasetsWithSosiRequirements(Good),
                statusReport.NumberOfFairDatasetsWithSosiRequirements(Useable),
                statusReport.NumberOfFairDatasetsWithSosiRequirements(Deficient),
                statusReport.NumberOfFairDatasetsWithSosiRequirements(Notset),
                statusReport.NumberOfFairDatasetsWithSosiRequirements(Satisfactory));

            NumberOfItemsWithGmlRequirements = new NumberOfStatuses(
                statusReport.NumberOfFairDatasetsWithGmlRequirements(Good),
                statusReport.NumberOfFairDatasetsWithGmlRequirements(Useable),
                statusReport.NumberOfFairDatasetsWithGmlRequirements(Deficient),
                statusReport.NumberOfFairDatasetsWithGmlRequirements(Notset),
                statusReport.NumberOfFairDatasetsWithGmlRequirements(Satisfactory));

            NumberOfItemsWithWms = new NumberOfStatuses(
                statusReport.NumberOfFairDatasetsWithWms(Good),
                statusReport.NumberOfFairDatasetsWithWms(Useable),
                statusReport.NumberOfFairDatasetsWithWms(Deficient),
                statusReport.NumberOfFairDatasetsWithWms(Notset),
                statusReport.NumberOfFairDatasetsWithWms(Satisfactory));

            NumberOfItemsWithWfs = new NumberOfStatuses(
                statusReport.NumberOfFairDatasetsWithWfs(Good),
                statusReport.NumberOfFairDatasetsWithWfs(Useable),
                statusReport.NumberOfFairDatasetsWithWfs(Deficient),
                statusReport.NumberOfFairDatasetsWithWfs(Notset),
                statusReport.NumberOfFairDatasetsWithWfs(Satisfactory));

            NumberOfItemsWithAtomFeed = new NumberOfStatuses(
                statusReport.NumberOfFairDatasetsWithAtomFeed(Good),
                statusReport.NumberOfFairDatasetsWithAtomFeed(Useable),
                statusReport.NumberOfFairDatasetsWithAtomFeed(Deficient),
                statusReport.NumberOfFairDatasetsWithAtomFeed(Notset),
                statusReport.NumberOfFairDatasetsWithAtomFeed(Satisfactory)
                );

            NumberOfItemsWithCommon = new NumberOfStatuses(
                statusReport.NumberOfItemsWithCommon(Good),
                statusReport.NumberOfItemsWithCommon(Useable),
                statusReport.NumberOfItemsWithCommon(Deficient),
                statusReport.NumberOfItemsWithCommon(Notset),
                statusReport.NumberOfItemsWithCommon(Satisfactory));


            PercentageOfDatasetsWithFindable = (int) statusReport.FairDatasetsFindablePercent();
            PercentageOfDatasetsWithAccesible = (int)statusReport.FairDatasetsAccessiblePercent();
            PercentageOfDatasetsWithInteroperable = (int)statusReport.FairDatasetsInteroperablePercent();
            PercentageOfDatasetsWithReUseable = (int) statusReport.FairDatasetsReuseablePercent();

            var numberOfItems = statusReport.NumberOfIems();

            PercentageOfDatasetsWithFindableGood = Percent(NumberOfItemsWithFindable.Good, numberOfItems);
            PercentageOfDatasetsWithFindableSatisfactory = Percent(NumberOfItemsWithFindable.Satisfactory, numberOfItems);
            PercentageOfDatasetsWithFindableUseable = Percent(NumberOfItemsWithFindable.Useable, numberOfItems);
            PercentageOfDatasetsWithFindableDeficient = Percent(NumberOfItemsWithFindable.Deficient, numberOfItems);

            PercentageOfDatasetsWithAccesibleGood = Percent(NumberOfItemsWithAccesible.Good, numberOfItems);
            PercentageOfDatasetsWithAccesibleSatisfactory = Percent(NumberOfItemsWithAccesible.Satisfactory, numberOfItems);
            PercentageOfDatasetsWithAccesibleUseable = Percent(NumberOfItemsWithAccesible.Useable, numberOfItems);
            PercentageOfDatasetsWithAccesibleDeficient = Percent(NumberOfItemsWithAccesible.Deficient, numberOfItems);

            PercentageOfDatasetsWithInteroperableGood = Percent(NumberOfItemsWithInteroperable.Good, numberOfItems);
            PercentageOfDatasetsWithInteroperableSatisfactory = Percent(NumberOfItemsWithInteroperable.Satisfactory, numberOfItems);
            PercentageOfDatasetsWithInteroperableUseable = Percent(NumberOfItemsWithInteroperable.Useable, numberOfItems);
            PercentageOfDatasetsWithInteroperableDeficient = Percent(NumberOfItemsWithInteroperable.Deficient, numberOfItems);

            PercentageOfDatasetsWithReUseableGood = Percent(NumberOfItemsWithReUseable.Good, numberOfItems);
            PercentageOfDatasetsWithReUseableSatisfactory = Percent(NumberOfItemsWithReUseable.Satisfactory, numberOfItems);
            PercentageOfDatasetsWithReUseableUseable = Percent(NumberOfItemsWithReUseable.Useable, numberOfItems);
            PercentageOfDatasetsWithReUseableDeficient = Percent(NumberOfItemsWithReUseable.Deficient, numberOfItems);
        }

        public double Percent(int numberOf, int numberOfItems)
        {
            return HtmlHelperExtensions.Percent(numberOf, numberOfItems);
        }
    }
}