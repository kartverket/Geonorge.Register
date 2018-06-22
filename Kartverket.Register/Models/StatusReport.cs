using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class StatusReport
    {

        private const string Good = "good";
        private const string Dificent = "dificent";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public StatusReport()
        {
            Id = Guid.NewGuid();
            Date = DateTime.Now;
            StatusHistories = new List<StatusHistory>();
        }


        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<StatusHistory> StatusHistories { get; set; }


        public int NumberOfItemsWithMetadata(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.Metadata == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithProductsheet(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.ProductSheet == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithPresentationRules(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.PresentationRules == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithProductSpecification(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.ProductSpecification == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithWms(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.Wms == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithWfs(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.Wfs == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithSosiRequirements(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.SosiRequirements == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithGmlRequirements(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.GmlRequirements == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithAtomFeed(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.AtomFeed == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithDistribution(string status)
        {
            int number = 0;
            foreach (StatusHistory item in StatusHistories)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.AtomFeed == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

    }


}