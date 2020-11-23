using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.FAIR
{
    public static class FAIRConstants
    {
        public const int F1Weight = 25;
        public const int F2KeywordsWeight = 10;
        public const int F2TitleWeight = 5;
        public const int F2DescriptionWeight = 10;
        public const int F3DatasetIdWeight = 25;
        public const int F4Weight = 25;

        public const int A1_1MetadataWeight = 30;
        public const int A1_1DatasetWeight = 40;
        public const int A1_2Weight = 30;
        public const int A2Weight = 0;

        public const int I1MetadataWeight = 20;
        public const int I1DatasetWeight = 20;

        public const int I2NationalThemeWeight = 10;
        public const int I2TopicCategoryWeight = 10;
        public const int I3ConceptsWeight = 20;
        public const int I3ApplicationSchemaInformationWeight = 20;

        public const int R1_1Weight = 40;
        public const int R1_2ProcessHistoryWeight = 10;
        public const int R1_2MaintenanceAndUpdateFrequencyWeight = 5;
        public const int R1_2ProductSpesificationWeight = 10;
        public const int R1_2ResolutionScaleWeight = 5;
        public const int R1_2CoverageMapWeight = 10;
        public const int R1_2CoverageMapCompleteWeight = 10;

        public const int R1_3StandardsWeight = 15;
        public const int R1_3OpenFormatsWeight = 15;
    }
}