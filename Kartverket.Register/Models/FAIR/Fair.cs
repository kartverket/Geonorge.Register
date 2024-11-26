using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.FAIR
{
    public class Fair
    {

        public bool F1_a_Criteria { get; set; } = true;
        public bool F2_a_Criteria { get; set; } = false;
        public bool F2_b_Criteria { get; set; } = false;
        public bool F2_c_Criteria { get; set; } = false;
        public bool F2_d_Criteria { get; set; } = false;
        public bool F2_e_Criteria { get; set; } = false;
        public bool F3_a_Criteria { get; set; } = false;
        public bool F4_a_Criteria { get; set; } = true;
        public int FindableStatusPerCent { get; set; }
        public FAIRDelivery FindableStatus { get; set; }

        public bool A1_a_Criteria { get; set; } = false;
        public bool A1_b_Criteria { get; set; } = false;
        public bool A1_c_Criteria { get; set; } = false;
        public bool A1_d_Criteria { get; set; } = false;
        public bool A1_e_Criteria { get; set; } = false;
        public bool A1_f_Criteria { get; set; } = true;
        public bool A2_a_Criteria { get; set; } = false;
        public int AccesibleStatusPerCent { get; set; }
        public FAIRDelivery AccesibleStatus { get; set; }

        public bool I1_a_Criteria { get; set; } = true;
        public bool I1_b_Criteria { get; set; } = false;
        //public bool? I1_c_Criteria { get; set; } = null; //Moved to I3_a_Criteria
        public bool I2_a_Criteria { get; set; } = false;
        public bool I2_b_Criteria { get; set; } = false;
        public bool? I3_a_Criteria { get; set; } = null;
        public bool? I3_b_Criteria { get; set; } = null;
        public bool? I3_c_Criteria { get; set; } = null;

        public int InteroperableStatusPerCent { get; set; }
        public FAIRDelivery InteroperableStatus { get; set; }

        public bool R1_a_Criteria { get; set; } = false;
        public bool R1_b_Criteria { get; set; } = false;
        public bool R2_a_Criteria { get; set; } = false;
        public bool R2_b_Criteria { get; set; } = false;
        public bool R2_c_Criteria { get; set; } = false;
        public bool R2_d_Criteria { get; set; } = false;
        public bool R2_e_Criteria { get; set; } = false;
        public bool R2_f_Criteria { get; set; } = false;
        public bool R2_g_Criteria { get; set; } = false;
        public bool R2_h_Criteria { get; set; } = false;
        public bool R3_a_Criteria { get; set; } = true;
        public bool R3_b_Criteria { get; set; } = false; 
        public int ReUseableStatusPerCent { get; set; }
        public FAIRDelivery ReUseableStatus { get; internal set; }

        public int FAIRStatusPerCent { get; internal set; }
        public FAIRDelivery FAIRStatus { get; internal set; }
    }
}