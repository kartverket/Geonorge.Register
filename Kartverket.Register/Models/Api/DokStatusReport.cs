using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models.ViewModels;
using Resources;

namespace Kartverket.Register.Models.Api
{
    public class DokStatusReport
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public DokStatusReport(Models.StatusReport statusReport)
        {
            if (statusReport != null)
            {
                Date = statusReport.Date;
                NumberOfDatasets = statusReport.NumberOfIems();

                NumberOfItemsWithMetadata = new NumberOfStatuses(statusReport.NumberOfItemsWithMetadata(Good),
                    statusReport.NumberOfItemsWithMetadata(Useable),
                    statusReport.NumberOfItemsWithMetadata(Deficient),
                    statusReport.NumberOfItemsWithMetadata(Notset));

                NumberOfItemsWithProductsheet = new NumberOfStatuses(statusReport.NumberOfItemsWithProductsheet(Good),
                    statusReport.NumberOfItemsWithProductsheet(Useable),
                    statusReport.NumberOfItemsWithProductsheet(Deficient),
                    statusReport.NumberOfItemsWithProductsheet(Notset));

                NumberOfItemsWithPresentationRules = new NumberOfStatuses(statusReport.NumberOfItemsWithPresentationRules(Good),
                    statusReport.NumberOfItemsWithPresentationRules(Useable),
                    statusReport.NumberOfItemsWithPresentationRules(Deficient),
                    statusReport.NumberOfItemsWithPresentationRules(Notset));

                NumberOfItemsWithProductSpecification = new NumberOfStatuses(
                    statusReport.NumberOfItemsWithPresentationRules(Good),
                    statusReport.NumberOfItemsWithProductSpecification(Useable),
                    statusReport.NumberOfItemsWithProductSpecification(Deficient),
                    statusReport.NumberOfItemsWithProductSpecification(Notset));

                NumberOfItemsWithWms = new NumberOfStatuses(statusReport.NumberOfItemsWithWms(Good),
                    statusReport.NumberOfItemsWithWms(Useable),
                    statusReport.NumberOfItemsWithWms(Deficient),
                    statusReport.NumberOfItemsWithWms(Notset));

                NumberOfItemsWithWfs = new NumberOfStatuses(statusReport.NumberOfItemsWithWfs(Good),
                    statusReport.NumberOfItemsWithWfs(Useable),
                    statusReport.NumberOfItemsWithWfs(Deficient),
                    statusReport.NumberOfItemsWithWfs(Notset));

                NumberOfItemsWithSosiRequirements = new NumberOfStatuses(
                    statusReport.NumberOfItemsWithSosiRequirements(Good),
                    statusReport.NumberOfItemsWithSosiRequirements(Useable),
                    statusReport.NumberOfItemsWithSosiRequirements(Deficient),
                    statusReport.NumberOfItemsWithSosiRequirements(Notset));

                NumberOfItemsWithGmlRequirements = new NumberOfStatuses(
                    statusReport.NumberOfItemsWithGmlRequirements(Good),
                    statusReport.NumberOfItemsWithGmlRequirements(Useable),
                    statusReport.NumberOfItemsWithGmlRequirements(Deficient),
                    statusReport.NumberOfItemsWithGmlRequirements(Notset));

                NumberOfItemsWithAtomFeed = new NumberOfStatuses(statusReport.NumberOfItemsWithAtomFeed(Good),
                    statusReport.NumberOfItemsWithAtomFeed(Useable),
                    statusReport.NumberOfItemsWithAtomFeed(Deficient),
                    statusReport.NumberOfItemsWithAtomFeed(Notset));

                NumberOfItemsWithDistribution = new NumberOfStatuses(statusReport.NumberOfItemsWithDistribution(Good),
                    statusReport.NumberOfItemsWithDistribution(Useable),
                    statusReport.NumberOfItemsWithDistribution(Deficient),
                    statusReport.NumberOfItemsWithDistribution(Notset));
            }
        }

        public int NumberOfDatasets { get; set; }
        public DateTime Date { get; set; }

        public NumberOfStatuses NumberOfItemsWithMetadata { get; set; }
        public NumberOfStatuses NumberOfItemsWithProductsheet { get; set; }
        public NumberOfStatuses NumberOfItemsWithPresentationRules { get; set; }
        public NumberOfStatuses NumberOfItemsWithProductSpecification { get; set; }
        public NumberOfStatuses NumberOfItemsWithWms { get; set; }
        public NumberOfStatuses NumberOfItemsWithWfs { get; set; }
        public NumberOfStatuses NumberOfItemsWithSosiRequirements { get; set; }
        public NumberOfStatuses NumberOfItemsWithGmlRequirements { get; set; }
        public NumberOfStatuses NumberOfItemsWithAtomFeed { get; set; }
        public NumberOfStatuses NumberOfItemsWithDistribution { get; set; }

    }
}