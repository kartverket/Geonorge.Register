using System.Collections.Generic;
using System.Web.Configuration;
using Kartverket.Register.Models.FAIR;
using Resources;

namespace Kartverket.Register.Models.Api
{
    /// <summary>
    /// FAIR assessment of a dataset with the same details as shown on the FAIR dataset page
    /// </summary>
    public class FAIR
    {
        public string FairStatus { get; set; }
        public double FAIRStatusPerCent { get; set; }
        public string FindableStatus { get; set; }
        public double FindableStatusPerCent { get; set; }
        public string AccesibleStatus { get; set; }
        public double AccesibleStatusPerCent { get; set; }
        public string InteroperableStatus { get; set; }
        public double InteroperableStatusPerCent { get; set; }
        public string ReUseableStatus { get; set; }
        public double ReUseableStatusPerCent { get; set; }
        public string DetailsPage { get; set; }

        /// <summary>
        /// Explanation of how the FAIR rating is given
        /// </summary>
        public FairRating Rating { get; set; }

        /// <summary>
        /// The four FAIR principles with criteria, in presentation order
        /// </summary>
        public List<FairPrinciple> Principles { get; set; }

        public FAIR()
        {
        }

        public FAIR(FairDataset fairDataset)
        {
            FairStatus = fairDataset.FAIRStatus.StatusId;
            FAIRStatusPerCent = fairDataset.FAIRStatusPerCent;

            FindableStatus = fairDataset.FindableStatus.StatusId;
            FindableStatusPerCent = fairDataset.FindableStatusPerCent;

            AccesibleStatus = fairDataset.AccesibleStatus.StatusId;
            AccesibleStatusPerCent = fairDataset.AccesibleStatusPerCent;

            InteroperableStatus = fairDataset.InteroperableStatus.StatusId;
            InteroperableStatusPerCent = fairDataset.InteroperableStatusPerCent;

            ReUseableStatus = fairDataset.ReUseableStatus.StatusId;
            ReUseableStatusPerCent = fairDataset.ReUseableStatusPerCent;

            DetailsPage = WebConfigurationManager.AppSettings["RegistryUrl"] + fairDataset.Register.seoname + "/" +
                          fairDataset.Seoname + "/" + fairDataset.SystemId + "#fair";

            Rating = CreateRating();
            Principles = CreatePrinciples(fairDataset);
        }

        private static FairRating CreateRating()
        {
            return new FairRating
            {
                Label = FairDataSet.RatingLabel,
                Description = FairDataSet.RatingDescription,
                Levels = new List<FairRatingLevel>
                {
                    new FairRatingLevel(FAIRDelivery.Good, FairDataSet.RatingGood),
                    new FairRatingLevel(FAIRDelivery.Satisfactory, FairDataSet.RatingSatisfactory),
                    new FairRatingLevel(FAIRDelivery.Useable, FairDataSet.RatingEmoprovements),
                    new FairRatingLevel(FAIRDelivery.Deficient, FairDataSet.RatingBad)
                }
            };
        }

        private static List<FairPrinciple> CreatePrinciples(FairDataset dataset)
        {
            return new List<FairPrinciple>
            {
                Findable(dataset),
                Accesible(dataset),
                Interoperable(dataset),
                ReUseable(dataset)
            };
        }

        private static FairPrinciple Findable(FairDataset dataset)
        {
            return new FairPrinciple
            {
                Code = "F",
                Label = FairDataSet.Findable_Label,
                Description = FairDataSet.Findable_Description,
                Status = dataset.FindableStatus.StatusId,
                StatusPerCent = dataset.FindableStatusPerCent,
                CriteriaGroups = new List<FairCriteriaGroup>
                {
                    Group("F1", FairDataSet.F1_Label,
                        Criterion("F1_a", FairDataSet.F1_a_Criteria, dataset.F1_a_Criteria)),
                    Group("F2", FairDataSet.F2_Label,
                        Criterion("F2_a", FairDataSet.F2_a_Criteria, dataset.F2_a_Criteria),
                        Criterion("F2_b", FairDataSet.F2_b_Criteria, dataset.F2_b_Criteria),
                        Criterion("F2_c", FairDataSet.F2_c_Criteria, dataset.F2_c_Criteria),
                        Criterion("F2_d", FairDataSet.F2_d_Criteria, dataset.F2_d_Criteria),
                        Criterion("F2_e", FairDataSet.F2_e_Criteria, dataset.F2_e_Criteria)),
                    Group("F3", FairDataSet.F3_Label,
                        Criterion("F3_a", FairDataSet.F3_a_Criteria, dataset.F3_a_Criteria)),
                    Group("F4", FairDataSet.F4_Label,
                        Criterion("F4_a", FairDataSet.F4_a_Criteria, dataset.F4_a_Criteria))
                }
            };
        }

        private static FairPrinciple Accesible(FairDataset dataset)
        {
            return new FairPrinciple
            {
                Code = "A",
                Label = FairDataSet.Accesible_Label,
                Description = FairDataSet.Accesible_Description,
                Status = dataset.AccesibleStatus.StatusId,
                StatusPerCent = dataset.AccesibleStatusPerCent,
                CriteriaGroups = new List<FairCriteriaGroup>
                {
                    Group("A1", FairDataSet.A1_Label,
                        Criterion("A1_a", FairDataSet.A1_a_Criteria, dataset.A1_a_Criteria),
                        Criterion("A1_b", FairDataSet.A1_b_Criteria, dataset.A1_b_Criteria),
                        Criterion("A1_c", FairDataSet.A1_c_Criteria, dataset.A1_c_Criteria),
                        Criterion("A1_d", FairDataSet.A1_d_Criteria, dataset.A1_d_Criteria),
                        Criterion("A1_e", FairDataSet.A1_e_Criteria, dataset.A1_e_Criteria),
                        Criterion("A1_f", FairDataSet.A1_f_Criteria, dataset.A1_f_Criteria)),
                    //A2 has no criteria, only a label, like on the details page
                    Group("A2", FairDataSet.A2_Label)
                }
            };
        }

        private static FairPrinciple Interoperable(FairDataset dataset)
        {
            return new FairPrinciple
            {
                Code = "I",
                Label = FairDataSet.Interoperable_Label,
                Description = FairDataSet.Interoperable_Description,
                Status = dataset.InteroperableStatus.StatusId,
                StatusPerCent = dataset.InteroperableStatusPerCent,
                CriteriaGroups = new List<FairCriteriaGroup>
                {
                    Group("I1", FairDataSet.I1_Label,
                        Criterion("I1_a", FairDataSet.I1_a_Criteria, dataset.I1_a_Criteria),
                        Criterion("I1_b", FairDataSet.I1_b_Criteria, dataset.I1_b_Criteria)),
                    Group("I2", FairDataSet.I2_Label,
                        Criterion("I2_a", FairDataSet.I2_a_Criteria, dataset.I2_a_Criteria),
                        Criterion("I2_b", FairDataSet.I2_b_Criteria, dataset.I2_b_Criteria)),
                    Group("I3", FairDataSet.I3_Label,
                        Criterion("I3_a", FairDataSet.I3_a_Criteria, dataset.I3_a_Criteria),
                        Criterion("I3_b", FairDataSet.I3_b_Criteria, dataset.I3_b_Criteria),
                        Criterion("I3_c", FairDataSet.I3_c_Criteria, dataset.I3_c_Criteria))
                }
            };
        }

        private static FairPrinciple ReUseable(FairDataset dataset)
        {
            return new FairPrinciple
            {
                Code = "R",
                Label = FairDataSet.ReUseable_Label,
                Description = FairDataSet.ReUseable_Description,
                Status = dataset.ReUseableStatus.StatusId,
                StatusPerCent = dataset.ReUseableStatusPerCent,
                CriteriaGroups = new List<FairCriteriaGroup>
                {
                    Group("R1", FairDataSet.R1_Label,
                        Criterion("R1_a", FairDataSet.R1_a_Criteria, dataset.R1_a_Criteria),
                        Criterion("R1_b", FairDataSet.R1_b_Criteria, dataset.R1_b_Criteria)),
                    Group("R2", FairDataSet.R2_Label,
                        Criterion("R2_a", FairDataSet.R2_a_Criteria, dataset.R2_a_Criteria),
                        Criterion("R2_b", FairDataSet.R2_b_Criteria, dataset.R2_b_Criteria),
                        Criterion("R2_c", FairDataSet.R2_c_Criteria, dataset.R2_c_Criteria),
                        Criterion("R2_d", FairDataSet.R2_d_Criteria, dataset.R2_d_Criteria),
                        Criterion("R2_e", FairDataSet.R2_e_Criteria, dataset.R2_e_Criteria),
                        Criterion("R2_f", FairDataSet.R2_f_Criteria, dataset.R2_f_Criteria),
                        Criterion("R2_g", FairDataSet.R2_g_Criteria, dataset.R2_g_Criteria),
                        Criterion("R2_h", FairDataSet.R2_h_Criteria, dataset.R2_h_Criteria),
                        Criterion("R2_i", FairDataSet.R2_i_Criteria, dataset.R2_i_Criteria)),
                    Group("R3", FairDataSet.R3_Label,
                        Criterion("R3_a", FairDataSet.R3_a_Criteria, dataset.R3_a_Criteria),
                        Criterion("R3_b", FairDataSet.R3_b_Criteria, dataset.R3_b_Criteria))
                }
            };
        }

        private static FairCriteriaGroup Group(string code, string label, params FairCriterion[] criteria)
        {
            return new FairCriteriaGroup
            {
                Code = code,
                Label = label,
                Criteria = new List<FairCriterion>(criteria)
            };
        }

        private static FairCriterion Criterion(string code, string description, bool? fulfilled)
        {
            return new FairCriterion
            {
                Code = code,
                Description = description,
                Fulfilled = fulfilled
            };
        }
    }

    public class FairRating
    {
        public string Label { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Rating levels from best to worst
        /// </summary>
        public List<FairRatingLevel> Levels { get; set; }
    }

    public class FairRatingLevel
    {
        /// <summary>
        /// good, satisfactory, useable or deficient
        /// </summary>
        public string Status { get; set; }
        public string Description { get; set; }

        public FairRatingLevel()
        {
        }

        public FairRatingLevel(string status, string description)
        {
            Status = status;
            Description = description;
        }
    }

    public class FairPrinciple
    {
        /// <summary>
        /// F, A, I or R
        /// </summary>
        public string Code { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// good, satisfactory, useable, deficient or notset
        /// </summary>
        public string Status { get; set; }
        public double StatusPerCent { get; set; }
        public List<FairCriteriaGroup> CriteriaGroups { get; set; }
    }

    public class FairCriteriaGroup
    {
        /// <summary>
        /// For example F1
        /// </summary>
        public string Code { get; set; }
        public string Label { get; set; }
        public List<FairCriterion> Criteria { get; set; }
    }

    public class FairCriterion
    {
        /// <summary>
        /// For example F1_a
        /// </summary>
        public string Code { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// True when fulfilled, false when not fulfilled and null when not evaluated
        /// </summary>
        public bool? Fulfilled { get; set; }
    }
}
